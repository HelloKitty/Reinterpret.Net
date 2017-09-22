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
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic value type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The converted value.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static unsafe TConvertType Reinterpret<TConvertType>(this byte[] bytes)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			//Originally we null and length checked the bytes. This caused performance issues on .NET Core for some reason
			//Removing them increased the speed by almost an order of magnitude.
			//We shouldn't really handhold the user trying to reinterpet things into other things
			//If they're using this library then they should KNOW they shouldn't mess around and anything could happen
			//We already sacrfice safety for performance. An order of magnitude performance increase is a no brainer here.

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretPrimitive<TConvertType>(bytes);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic value type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="start">The starting position to read the <typeparamref name="TConvertType"/> value from.</param>
		/// <returns>The converted value.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static unsafe TConvertType Reinterpret<TConvertType>(this byte[] bytes, int start)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretPrimitive<TConvertType>(bytes, start);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to the specified generic array of value types.
		/// </summary>
		/// <typeparam name="TConvertType">The element type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The array of converted values.</returns>
		public static unsafe TConvertType[] ReinterpretToArray<TConvertType>(this byte[] bytes)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			//Don't check nullness for perf. Callers shouldn't give us null arrays
			if(bytes.Length == 0) return new TConvertType[0];

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretPrimitiveArray<TConvertType>(bytes);
		}

		private static TConvertType[] ReinterpretPrimitiveArray<TConvertType>(byte[] bytes)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			//If someone happens to ask for the byte representation of bytes
			if(typeof(TConvertType) == typeof(byte))
				return bytes as TConvertType[];

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			//BlockCopy is slightly faster if we have to reallocate
			TConvertType[] convertedValues = new TConvertType[unchecked(bytes.Length / MarshalSizeOf<TConvertType>.SizeOf)];

			Buffer.BlockCopy(bytes, 0, convertedValues, 0, bytes.Length);

			return convertedValues;
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> to a C# standard UTF16 encoded (2 byte char) string.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The converted UTF16 string.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static unsafe string ReinterpretToString(this byte[] bytes)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) return "";

			//The caller may want to reuse the byte array so we check if they will allow us to destroy it
			char[] chars = bytes.ToArray().ToConvertedArrayPerm<char>();
			return Unsafe.As<char[], string>(ref chars);
		}

		/// <summary>
		/// High performance: unsafely Reinterprets the provided <see cref="bytes"/> 
		/// to a C# standard UTF16 encoded (2 byte char) string.
		/// The original bytes array will be left in an INVALID state and should be considered destroyed.
		/// </summary>
		/// <param name="bytes">The bytes chunk. (Invalid after calling)</param>
		/// <returns>The converted UTF16 string.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static unsafe string ReinterpretToStringWithoutPreserving(this byte[] bytes)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) return "";

			//this will destroy the original byte array
			char[] chars = bytes.ToConvertedArrayPerm<char>();
			return Unsafe.As<char[], string>(ref chars);
		}

		/// <summary>
		/// Reinterprets the <see cref="bytes"/> to the specified primitive type.
		/// </summary>
		/// <typeparam name="TConvertType">The type to convert to.</typeparam>
		/// <param name="bytes">The bytes to convert from.</param>
		/// <param name="position">The position to begin reading from.</param>
		/// <returns>The converted value.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		private static unsafe TConvertType ReinterpretPrimitive<TConvertType>(byte[] bytes, int position = 0)
			where TConvertType : struct
		{
			return Unsafe.ReadUnaligned<TConvertType>(ref bytes[position]);
		}
	}
}
