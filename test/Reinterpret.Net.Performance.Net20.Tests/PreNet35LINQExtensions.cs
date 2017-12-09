//This is from Microsoft's .NET Core "corefx"
//https://github.com/dotnet/corefx/blob/master/src/System.Linq/src/System/Linq/ToCollection.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

//To use Extension Methods in .NET 2.0 we must use the below hack explained by Jon Skeet
//This is an attribute that is missing in net20 that must be added to be available
//http://csharpindepth.com/Articles/Chapter1/Versions.aspx
namespace System.Runtime.CompilerServices
{
	[AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	public class ExtensionAttribute : Attribute
	{
	}
}

namespace System.Collections.Generic
{
	/// <summary>
	/// Internal helper functions for working with enumerables.
	/// </summary>
	internal static partial class EnumerableHelpers
	{
		/// <summary>
		/// Tries to get the count of the enumerable cheaply.
		/// </summary>
		/// <typeparam name="T">The element type of the source enumerable.</typeparam>
		/// <param name="source">The enumerable to count.</param>
		/// <param name="count">The count of the enumerable, if it could be obtained cheaply.</param>
		/// <returns><c>true</c> if the enumerable could be counted cheaply; otherwise, <c>false</c>.</returns>
		internal static bool TryGetCount<T>(IEnumerable<T> source, out int count)
		{
			Debug.Assert(source != null);

			if(source is ICollection<T> collection)
			{
				count = collection.Count;
				return true;
			}

			if(source is IIListProvider<T> provider)
			{
				return (count = provider.GetCount(onlyIfCheap: true)) >= 0;
			}

			count = -1;
			return false;
		}
	}

	/// <summary>
	/// Internal helper functions for working with enumerables.
	/// </summary>
	internal static partial class EnumerableHelpers
	{
		/// <summary>
		/// Copies items from an enumerable to an array.
		/// </summary>
		/// <typeparam name="T">The element type of the enumerable.</typeparam>
		/// <param name="source">The source enumerable.</param>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The index in the array to start copying to.</param>
		/// <param name="count">The number of items in the enumerable.</param>
		internal static void Copy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)
		{
			Debug.Assert(source != null);
			Debug.Assert(arrayIndex >= 0);
			Debug.Assert(count >= 0);
			Debug.Assert(array?.Length - arrayIndex >= count);

			if(source is ICollection<T> collection)
			{
				Debug.Assert(collection.Count == count);
				collection.CopyTo(array, arrayIndex);
				return;
			}

			IterativeCopy(source, array, arrayIndex, count);
		}

		/// <summary>
		/// Copies items from a non-collection enumerable to an array.
		/// </summary>
		/// <typeparam name="T">The element type of the enumerable.</typeparam>
		/// <param name="source">The source enumerable.</param>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The index in the array to start copying to.</param>
		/// <param name="count">The number of items in the enumerable.</param>
		internal static void IterativeCopy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)
		{
			Debug.Assert(source != null && !(source is ICollection<T>));
			Debug.Assert(arrayIndex >= 0);
			Debug.Assert(count >= 0);
			Debug.Assert(array?.Length - arrayIndex >= count);

			int endIndex = arrayIndex + count;
			foreach(T item in source)
			{
				array[arrayIndex++] = item;
			}

			Debug.Assert(arrayIndex == endIndex);
		}

		/// <summary>Converts an enumerable to an array.</summary>
		/// <param name="source">The enumerable to convert.</param>
		/// <returns>The resulting array.</returns>
		internal static T[] ToArray<T>(IEnumerable<T> source)
		{
			Debug.Assert(source != null);

			if(source is ICollection<T> collection)
			{
				int count = collection.Count;
				if(count == 0)
				{
					return new T[0];
				}

				var result = new T[count];
				collection.CopyTo(result, arrayIndex: 0);
				return result;
			}

			var builder = new LargeArrayBuilder<T>(initialize: true);
			builder.AddRange(source);
			return builder.ToArray();
		}

		/// <summary>Converts an enumerable to an array using the same logic as List{T}.</summary>
		/// <param name="source">The enumerable to convert.</param>
		/// <param name="length">The number of items stored in the resulting array, 0-indexed.</param>
		/// <returns>
		/// The resulting array.  The length of the array may be greater than <paramref name="length"/>,
		/// which is the actual number of elements in the array.
		/// </returns>
		internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
		{
			if(source is ICollection<T> ic)
			{
				int count = ic.Count;
				if(count != 0)
				{
					// Allocate an array of the desired size, then copy the elements into it. Note that this has the same
					// issue regarding concurrency as other existing collections like List<T>. If the collection size
					// concurrently changes between the array allocation and the CopyTo, we could end up either getting an
					// exception from overrunning the array (if the size went up) or we could end up not filling as many
					// items as 'count' suggests (if the size went down).  This is only an issue for concurrent collections
					// that implement ICollection<T>, which as of .NET 4.6 is just ConcurrentDictionary<TKey, TValue>.
					T[] arr = new T[count];
					ic.CopyTo(arr, 0);
					length = count;
					return arr;
				}
			}
			else
			{
				using(var en = source.GetEnumerator())
				{
					if(en.MoveNext())
					{
						const int DefaultCapacity = 4;
						T[] arr = new T[DefaultCapacity];
						arr[0] = en.Current;
						int count = 1;

						while(en.MoveNext())
						{
							if(count == arr.Length)
							{
								// MaxArrayLength is defined in Array.MaxArrayLength and in gchelpers in CoreCLR.
								// It represents the maximum number of elements that can be in an array where
								// the size of the element is greater than one byte; a separate, slightly larger constant,
								// is used when the size of the element is one.
								const int MaxArrayLength = 0x7FEFFFFF;

								// This is the same growth logic as in List<T>:
								// If the array is currently empty, we make it a default size.  Otherwise, we attempt to
								// double the size of the array.  Doubling will overflow once the size of the array reaches
								// 2^30, since doubling to 2^31 is 1 larger than Int32.MaxValue.  In that case, we instead
								// constrain the length to be MaxArrayLength (this overflow check works because of the
								// cast to uint).  Because a slightly larger constant is used when T is one byte in size, we
								// could then end up in a situation where arr.Length is MaxArrayLength or slightly larger, such
								// that we constrain newLength to be MaxArrayLength but the needed number of elements is actually
								// larger than that.  For that case, we then ensure that the newLength is large enough to hold
								// the desired capacity.  This does mean that in the very rare case where we've grown to such a
								// large size, each new element added after MaxArrayLength will end up doing a resize.
								int newLength = count << 1;
								if((uint)newLength > MaxArrayLength)
								{
									newLength = MaxArrayLength <= count ? count + 1 : MaxArrayLength;
								}

								Array.Resize(ref arr, newLength);
							}

							arr[count++] = en.Current;
						}

						length = count;
						return arr;
					}
				}
			}

			length = 0;
			return new T[0];
		}
	}

	/// <summary>
	/// Represents a position within a <see cref="LargeArrayBuilder{T}"/>.
	/// </summary>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	internal struct CopyPosition
	{
		/// <summary>
		/// Constructs a new <see cref="CopyPosition"/>.
		/// </summary>
		/// <param name="row">The index of the buffer to select.</param>
		/// <param name="column">The index within the buffer to select.</param>
		internal CopyPosition(int row, int column)
		{
			Debug.Assert(row >= 0);
			Debug.Assert(column >= 0);

			Row = row;
			Column = column;
		}

		/// <summary>
		/// Represents a position at the start of a <see cref="LargeArrayBuilder{T}"/>.
		/// </summary>
		public static CopyPosition Start => default(CopyPosition);

		/// <summary>
		/// The index of the buffer to select.
		/// </summary>
		internal int Row { get; }

		/// <summary>
		/// The index within the buffer to select.
		/// </summary>
		internal int Column { get; }

		/// <summary>
		/// If this position is at the end of the current buffer, returns the position
		/// at the start of the next buffer. Otherwise, returns this position.
		/// </summary>
		/// <param name="endColumn">The length of the current buffer.</param>
		public CopyPosition Normalize(int endColumn)
		{
			Debug.Assert(Column <= endColumn);

			return Column == endColumn ?
				new CopyPosition(Row + 1, 0) :
				this;
		}

		/// <summary>
		/// Gets a string suitable for display in the debugger.
		/// </summary>
		private string DebuggerDisplay => $"[{Row}, {Column}]";
	}

	/// <summary>
	/// Helper type for avoiding allocations while building arrays.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	internal struct ArrayBuilder<T>
	{
		private const int DefaultCapacity = 4;
		private const int MaxCoreClrArrayLength = 0x7fefffff; // For byte arrays the limit is slightly larger

		private T[] _array; // Starts out null, initialized on first Add.
		private int _count; // Number of items into _array we're using.

		/// <summary>
		/// Initializes the <see cref="ArrayBuilder{T}"/> with a specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity of the array to allocate.</param>
		public ArrayBuilder(int capacity) : this()
		{
			Debug.Assert(capacity >= 0);
			if(capacity > 0)
			{
				_array = new T[capacity];
			}
		}

		/// <summary>
		/// Gets the number of items this instance can store without re-allocating,
		/// or 0 if the backing array is <c>null</c>.
		/// </summary>
		public int Capacity => _array?.Length ?? 0;

		/// <summary>
		/// Gets the number of items in the array currently in use.
		/// </summary>
		public int Count => _count;

		/// <summary>
		/// Gets or sets the item at a certain index in the array.
		/// </summary>
		/// <param name="index">The index into the array.</param>
		public T this[int index]
		{
			get
			{
				Debug.Assert(index >= 0 && index < _count);
				return _array[index];
			}
			set
			{
				Debug.Assert(index >= 0 && index < _count);
				_array[index] = value;
			}
		}

		/// <summary>
		/// Adds an item to the backing array, resizing it if necessary.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(T item)
		{
			if(_count == Capacity)
			{
				EnsureCapacity(_count + 1);
			}

			UncheckedAdd(item);
		}

		/// <summary>
		/// Gets the first item in this builder.
		/// </summary>
		public T First()
		{
			Debug.Assert(_count > 0);
			return _array[0];
		}

		/// <summary>
		/// Gets the last item in this builder.
		/// </summary>
		public T Last()
		{
			Debug.Assert(_count > 0);
			return _array[_count - 1];
		}

		/// <summary>
		/// Creates an array from the contents of this builder.
		/// </summary>
		/// <remarks>
		/// Do not call this method twice on the same builder.
		/// </remarks>
		public T[] ToArray()
		{
			if(_count == 0)
			{
				return new T[0];
			}

			Debug.Assert(_array != null); // Nonzero _count should imply this

			T[] result = _array;
			if(_count < result.Length)
			{
				// Avoid a bit of overhead (method call, some branches, extra codegen)
				// which would be incurred by using Array.Resize
				result = new T[_count];
				Array.Copy(_array, 0, result, 0, _count);
			}

#if DEBUG
// Try to prevent callers from using the ArrayBuilder after ToArray, if _count != 0.
			_count = -1;
			_array = null;
#endif

			return result;
		}

		/// <summary>
		/// Adds an item to the backing array, without checking if there is room.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <remarks>
		/// Use this method if you know there is enough space in the <see cref="ArrayBuilder{T}"/>
		/// for another item, and you are writing performance-sensitive code.
		/// </remarks>
		public void UncheckedAdd(T item)
		{
			Debug.Assert(_count < Capacity);

			_array[_count++] = item;
		}

		private void EnsureCapacity(int minimum)
		{
			Debug.Assert(minimum > Capacity);

			int capacity = Capacity;
			int nextCapacity = capacity == 0 ? DefaultCapacity : 2 * capacity;

			if((uint)nextCapacity > (uint)MaxCoreClrArrayLength)
			{
				nextCapacity = Math.Max(capacity + 1, MaxCoreClrArrayLength);
			}

			nextCapacity = Math.Max(nextCapacity, minimum);

			T[] next = new T[nextCapacity];
			if(_count > 0)
			{
				Array.Copy(_array, 0, next, 0, _count);
			}
			_array = next;
		}
	}

	/// <summary>
	/// Helper type for building dynamically-sized arrays while minimizing allocations and copying.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	internal struct LargeArrayBuilder<T>
	{
		private const int StartingCapacity = 4;
		private const int ResizeLimit = 8;

		private readonly int _maxCapacity;  // The maximum capacity this builder can have.
		private T[] _first;                 // The first buffer we store items in. Resized until ResizeLimit.
		private ArrayBuilder<T[]> _buffers; // After ResizeLimit * 2, we store previous buffers we've filled out here.
		private T[] _current;               // Current buffer we're reading into. If _count <= ResizeLimit, this is _first.
		private int _index;                 // Index into the current buffer.
		private int _count;                 // Count of all of the items in this builder.

		/// <summary>
		/// Constructs a new builder.
		/// </summary>
		/// <param name="initialize">Pass <c>true</c>.</param>
		public LargeArrayBuilder(bool initialize)
			: this(maxCapacity: int.MaxValue)
		{
			// This is a workaround for C# not having parameterless struct constructors yet.
			// Once it gets them, replace this with a parameterless constructor.
			Debug.Assert(initialize);
		}

		/// <summary>
		/// Constructs a new builder with the specified maximum capacity.
		/// </summary>
		/// <param name="maxCapacity">The maximum capacity this builder can have.</param>
		/// <remarks>
		/// Do not add more than <paramref name="maxCapacity"/> items to this builder.
		/// </remarks>
		public LargeArrayBuilder(int maxCapacity)
			: this()
		{
			Debug.Assert(maxCapacity >= 0);

			_first = _current = new T[0];
			_maxCapacity = maxCapacity;
		}

		/// <summary>
		/// Gets the number of items added to the builder.
		/// </summary>
		public int Count => _count;

		/// <summary>
		/// Adds an item to this builder.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <remarks>
		/// Use <see cref="Add"/> if adding to the builder is a bottleneck for your use case.
		/// Otherwise, use <see cref="SlowAdd"/>.
		/// </remarks>
		public void Add(T item)
		{
			Debug.Assert(_maxCapacity > _count);

			if(_index == _current.Length)
			{
				AllocateBuffer();
			}

			_current[_index++] = item;
			_count++;
		}

		/// <summary>
		/// Adds a range of items to this builder.
		/// </summary>
		/// <param name="items">The sequence to add.</param>
		/// <remarks>
		/// It is the caller's responsibility to ensure that adding <paramref name="items"/>
		/// does not cause the builder to exceed its maximum capacity.
		/// </remarks>
		public void AddRange(IEnumerable<T> items)
		{
			Debug.Assert(items != null);

			using(IEnumerator<T> enumerator = items.GetEnumerator())
			{
				T[] destination = _current;
				int index = _index;

				// Continuously read in items from the enumerator, updating _count
				// and _index when we run out of space.

				while(enumerator.MoveNext())
				{
					if(index == destination.Length)
					{
						// No more space in this buffer. Resize.
						_count += index - _index;
						_index = index;
						AllocateBuffer();
						destination = _current;
						index = _index; // May have been reset to 0
					}

					destination[index++] = enumerator.Current;
				}

				// Final update to _count and _index.
				_count += index - _index;
				_index = index;
			}
		}

		/// <summary>
		/// Copies the contents of this builder to the specified array.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The index in <see cref="array"/> to start copying to.</param>
		/// <param name="count">The number of items to copy.</param>
		public void CopyTo(T[] array, int arrayIndex, int count)
		{
			Debug.Assert(arrayIndex >= 0);
			Debug.Assert(count >= 0 && count <= Count);
			Debug.Assert(array?.Length - arrayIndex >= count);

			for(int i = 0; count > 0; i++)
			{
				// Find the buffer we're copying from.
				T[] buffer = GetBuffer(index: i);

				// Copy until we satisfy count, or we reach the end of the buffer.
				int toCopy = Math.Min(count, buffer.Length);
				Array.Copy(buffer, 0, array, arrayIndex, toCopy);

				// Increment variables to that position.
				count -= toCopy;
				arrayIndex += toCopy;
			}
		}

		/// <summary>
		/// Copies the contents of this builder to the specified array.
		/// </summary>
		/// <param name="position">The position in this builder to start copying from.</param>
		/// <param name="array">The destination array.</param>
		/// <param name="arrayIndex">The index in <see cref="array"/> to start copying to.</param>
		/// <param name="count">The number of items to copy.</param>
		/// <returns>The position in this builder that was copied up to.</returns>
		public CopyPosition CopyTo(CopyPosition position, T[] array, int arrayIndex, int count)
		{
			Debug.Assert(array != null);
			Debug.Assert(arrayIndex >= 0);
			Debug.Assert(count > 0 && count <= Count);
			Debug.Assert(array.Length - arrayIndex >= count);

			// Go through each buffer, which contains one 'row' of items.
			// The index in each buffer is referred to as the 'column'.

			/*
			 * Visual representation:
			 * 
			 *       C0   C1   C2 ..  C31 ..   C63
			 * R0:  [0]  [1]  [2] .. [31]
			 * R1: [32] [33] [34] .. [63]
			 * R2: [64] [65] [66] .. [95] .. [127]
			 */

			int row = position.Row;
			int column = position.Column;

			T[] buffer = GetBuffer(row);
			int copied = CopyToCore(buffer, column);

			if(count == 0)
			{
				return new CopyPosition(row, column + copied).Normalize(buffer.Length);
			}

			do
			{
				buffer = GetBuffer(++row);
				copied = CopyToCore(buffer, 0);
			} while(count > 0);

			return new CopyPosition(row, copied).Normalize(buffer.Length);

			int CopyToCore(T[] sourceBuffer, int sourceIndex)
			{
				Debug.Assert(sourceBuffer.Length > sourceIndex);

				// Copy until we satisfy `count` or reach the end of the current buffer.
				int copyCount = Math.Min(sourceBuffer.Length - sourceIndex, count);
				Array.Copy(sourceBuffer, sourceIndex, array, arrayIndex, copyCount);

				arrayIndex += copyCount;
				count -= copyCount;

				return copyCount;
			}
		}

		/// <summary>
		/// Retrieves the buffer at the specified index.
		/// </summary>
		/// <param name="index">The index of the buffer.</param>
		public T[] GetBuffer(int index)
		{
			Debug.Assert(index >= 0 && index < _buffers.Count + 2);

			return index == 0 ? _first :
				index <= _buffers.Count ? _buffers[index - 1] :
					_current;
		}

		/// <summary>
		/// Adds an item to this builder.
		/// </summary>
		/// <param name="item">The item to add.</param>
		/// <remarks>
		/// Use <see cref="Add"/> if adding to the builder is a bottleneck for your use case.
		/// Otherwise, use <see cref="SlowAdd"/>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void SlowAdd(T item) => Add(item);

		/// <summary>
		/// Creates an array from the contents of this builder.
		/// </summary>
		public T[] ToArray()
		{
			if(TryMove(out T[] array))
			{
				// No resizing to do.
				return array;
			}

			array = new T[_count];
			CopyTo(array, 0, _count);
			return array;
		}

		/// <summary>
		/// Attempts to transfer this builder into an array without copying.
		/// </summary>
		/// <param name="array">The transferred array, if the operation succeeded.</param>
		/// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
		public bool TryMove(out T[] array)
		{
			array = _first;
			return _count == _first.Length;
		}

		private void AllocateBuffer()
		{
			// - On the first few adds, simply resize _first.
			// - When we pass ResizeLimit, allocate ResizeLimit elements for _current
			//   and start reading into _current. Set _index to 0.
			// - When _current runs out of space, add it to _buffers and repeat the
			//   above step, except with _current.Length * 2.
			// - Make sure we never pass _maxCapacity in all of the above steps.

			Debug.Assert((uint)_maxCapacity > (uint)_count);
			Debug.Assert(_index == _current.Length, $"{nameof(AllocateBuffer)} was called, but there's more space.");

			// If _count is int.MinValue, we want to go down the other path which will raise an exception.
			if((uint)_count < (uint)ResizeLimit)
			{
				// We haven't passed ResizeLimit. Resize _first, copying over the previous items.
				Debug.Assert(_current == _first && _count == _first.Length);

				int nextCapacity = Math.Min(_count == 0 ? StartingCapacity : _count * 2, _maxCapacity);

				_current = new T[nextCapacity];
				Array.Copy(_first, 0, _current, 0, _count);
				_first = _current;
			}
			else
			{
				Debug.Assert(_maxCapacity > ResizeLimit);
				Debug.Assert(_count == ResizeLimit ^ _current != _first);

				int nextCapacity;
				if(_count == ResizeLimit)
				{
					nextCapacity = ResizeLimit;
				}
				else
				{
					// Example scenario: Let's say _count == 64.
					// Then our buffers look like this: | 8 | 8 | 16 | 32 |
					// As you can see, our count will be just double the last buffer.
					// Now, say _maxCapacity is 100. We will find the right amount to allocate by
					// doing min(64, 100 - 64). The lhs represents double the last buffer,
					// the rhs the limit minus the amount we've already allocated.

					Debug.Assert(_count >= ResizeLimit * 2);
					Debug.Assert(_count == _current.Length * 2);

					_buffers.Add(_current);
					nextCapacity = Math.Min(_count, _maxCapacity - _count);
				}

				_current = new T[nextCapacity];
				_index = 0;
			}
		}
	}
}

namespace System.Linq
{
	internal static partial class Enumerable
	{
		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return source is IIListProvider<TSource> arrayProvider
				? arrayProvider.ToArray()
				: EnumerableHelpers.ToArray(source);
		}

		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			if(source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			return source is IIListProvider<TSource> listProvider ? listProvider.ToList() : new List<TSource>(source);
		}

	}

	/// <summary>
	/// An iterator that can produce an array or <see cref="List{TElement}"/> through an optimized path.
	/// </summary>
	internal interface IIListProvider<TElement> : IEnumerable<TElement>
	{
		/// <summary>
		/// Produce an array of the sequence through an optimized path.
		/// </summary>
		/// <returns>The array.</returns>
		TElement[] ToArray();

		/// <summary>
		/// Produce a <see cref="List{TElement}"/> of the sequence through an optimized path.
		/// </summary>
		/// <returns>The <see cref="List{TElement}"/>.</returns>
		List<TElement> ToList();

		/// <summary>
		/// Returns the count of elements in the sequence.
		/// </summary>
		/// <param name="onlyIfCheap">If true then the count should only be calculated if doing
		/// so is quick (sure or likely to be constant time), otherwise -1 should be returned.</param>
		/// <returns>The number of elements.</returns>
		int GetCount(bool onlyIfCheap);
	}
}