using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Extension methods that cast from <see cref="byte"/>s to
	/// the specified target Type.
	/// </summary>
	public static class ReInterpretFromBytesExtensions
	{
		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> pointer (in the form of an <see cref="IntPtr"/>
		/// to the specified generic value type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe TConvertType Reinterpret<TConvertType>(this IntPtr bytes)
			where TConvertType : unmanaged
		{
			//Originally we null and length checked the bytes. This caused performance issues on .NET Core for some reason
			//Removing them increased the speed by almost an order of magnitude.
			//We shouldn't really handhold the user trying to reinterpet things into other things
			//If they're using this library then they should KNOW they shouldn't mess around and anything could happen
			//We already sacrfice safety for performance. An order of magnitude performance increase is a no brainer here.
			return ReinterpretPrimitive<TConvertType>((byte*)bytes);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic value type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="start">The starting position to read the <typeparamref name="TConvertType"/> value from.</param>
		/// <returns>The converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe TConvertType Reinterpret<TConvertType>(this Span<byte> bytes, int start = 0)
			where TConvertType : unmanaged
		{
			return ReinterpretPrimitive<TConvertType>(bytes, start);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic value type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="start">The starting position to read the <typeparamref name="TConvertType"/> value from.</param>
		/// <returns>The converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe TConvertType Reinterpret<TConvertType>(this byte[] bytes, int start = 0)
			where TConvertType : unmanaged
		{
			return Reinterpret<TConvertType>(new Span<byte>(bytes), start);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic array of value types.
		/// </summary>
		/// <typeparam name="TConvertType">The element type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The array of converted values.</returns>
		public static TConvertType[] ReinterpretToArray<TConvertType>(this Span<byte> bytes)
			where TConvertType : unmanaged
		{
			//Don't check nullness for perf. Callers shouldn't give us null arrays
			if(bytes.Length == 0) return Array.Empty<TConvertType>();

			if(bytes.Length % MarshalSizeOf<TConvertType>.SizeOf != 0)
				ThrowHelpers.ThrowMismatchedArraySizeForElementType<char>();

			return ReinterpretPrimitiveArray<TConvertType>(bytes);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic array of value types.
		/// </summary>
		/// <typeparam name="TConvertType">The element type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The array of converted values.</returns>
		public static TConvertType[] ReinterpretToArray<TConvertType>(this byte[] bytes)
			where TConvertType : unmanaged
		{
			return ReinterpretToArray<TConvertType>(new Span<byte>(bytes));
		}

		private static unsafe TConvertType[] ReinterpretPrimitiveArray<TConvertType>(Span<byte> bytes)
			where TConvertType : unmanaged
		{
			//If someone happens to ask for the byte representation of bytes
			if (typeof(TConvertType) == typeof(byte))
				return bytes.ToArray() as TConvertType[];

			int elementSize = MarshalSizeOf<TConvertType>.SizeOf;
			if (elementSize == 0)
				throw new InvalidOperationException("Invalid type size");

			if (bytes.Length % elementSize != 0)
				throw new ArgumentException("Byte length is not a multiple of the element size");

			//BlockCopy is slightly faster if we have to reallocate
			TConvertType[] convertedValues = new TConvertType[unchecked(bytes.Length / elementSize)];

			fixed (TConvertType* dest = convertedValues)
			{
				byte* destPtr = (byte*) dest;
				Unsafe.CopyBlockUnaligned(ref destPtr[0], ref bytes[0], (uint)bytes.Length);
			}

			return convertedValues;
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to a C# standard UTF16 encoded (2 byte char) string.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="encoding">Optional encoding to use. (Default is Unicode)</param>
		/// <returns>The converted UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ReinterpretToString(this Span<byte> bytes, Encoding encoding = null)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) return "";

			//TODO: Is there a faster way to do this now that we can't reinterpret?
			fixed(byte* ptr = bytes)
				return encoding == null ? Encoding.Unicode.GetString(ptr, bytes.Length) : encoding.GetString(ptr, bytes.Length);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to a C# standard UTF16 encoded (2 byte char) string.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="start">The offset into the buffer.</param>
		/// <param name="encoding">Optional encoding to use. (Default is Unicode)</param>
		/// <returns>The converted UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ReinterpretToString(this Span<byte> bytes, int start, Encoding encoding = null)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) return "";

			//TODO: Is there a faster way to do this now that we can't reinterpret?
			fixed(byte* ptr = bytes.Slice(start))
				return encoding == null ? Encoding.Unicode.GetString(ptr, bytes.Length) : encoding.GetString(ptr, bytes.Length);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to a C# standard UTF16 encoded (2 byte char) string.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="encoding">Optional encoding to use. (Default is Unicode)</param>
		/// <returns>The converted UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ReinterpretToString(this byte[] bytes, Encoding encoding = null)
		{
			return ReinterpretToString(new Span<byte>(bytes), encoding);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to a C# standard UTF16 encoded (2 byte char) string.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="start">The offset into the buffer.</param>
		/// <param name="encoding">Optional encoding to use. (Default is Unicode)</param>
		/// <returns>The converted UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string ReinterpretToString(this byte[] bytes, int start, Encoding encoding = null)
		{
			return ReinterpretToString(new Span<byte>(bytes), start, encoding);
		}

		/// <summary>
		/// Reinterprets the <see cref="bytes"/> to the specified primitive type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to convert to.</typeparam>
		/// <param name="bytes">The bytes to convert from.</param>
		/// <param name="position">The position to begin reading from.</param>
		/// <returns>The converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TConvertType ReinterpretPrimitive<TConvertType>(Span<byte> bytes, int position = 0)
			where TConvertType : unmanaged
		{
			// AI suggested bounds checking
			if (position < 0 
			    || (position + MarshalSizeOf<TConvertType>.SizeOf) > bytes.Length)
				ThrowOutofBoundsException();

			return Unsafe.ReadUnaligned<TConvertType>(ref bytes[position]);
		}

		private static void ThrowOutofBoundsException() 
		{
			throw new ArgumentOutOfRangeException($"Position is out of bounds or does not leave enough space for the type.");
		}

		/// <summary>
		/// Reinterprets the <see cref="bytes"/> to the specified primitive type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to convert to.</typeparam>
		/// <param name="bytes">The bytes to convert from.</param>
		/// <returns>The converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe TConvertType ReinterpretPrimitive<TConvertType>(byte* bytes)
			where TConvertType : unmanaged
		{
			return Unsafe.ReadUnaligned<TConvertType>(bytes);
		}
	}
}
