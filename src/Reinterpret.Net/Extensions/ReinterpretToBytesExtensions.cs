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
	/// Extension methods that cast to <see cref="byte"/>s from
	/// the specified target Type.
	/// </summary
	public static class ReinterpretToBytesExtensions
	{
		/// <summary>
		/// Reinterprets the provided <see cref="value"/> value to the C# standard
		/// byte array representation.
		/// </summary>
		/// <typeparam name="TConvertType">The type of the value.</typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static byte[] Reinterpret<TConvertType>(this TConvertType value)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretFromPrimitive(value);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="value"/> array to the byte representation
		/// of the array.
		/// </summary>
		/// <typeparam name="TConvertType">The element type of the array.</typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe static byte[] Reinterpret<TConvertType>(this TConvertType[] values)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			//Don't check if null. It's a lot faster not to
			if(values.Length == 0) return new byte[0];

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return values.ToArray().ToByteArrayPerm();
		}

		/// <summary>
		/// High performance but unsafe version that reinterprets the provided <see cref="value"/> array to the byte representation.
		/// WARNING: This version will NOT leave the <see cref="values"/> array intact. It will be
		/// left in an invalid state.
		/// </summary>
		/// <typeparam name="TConvertType">The element type of the array.</typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public unsafe static byte[] ReinterpretWithoutPreserving<TConvertType>(this TConvertType[] values)
			where TConvertType : struct, IComparable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			//Don't check if null. It's a lot faster not to
			if(values.Length == 0) return new byte[0];

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return values.ToByteArrayPerm();
		}

#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		private static byte[] ReinterpretFromPrimitive<TConvertType>(TConvertType value) 
			where TConvertType : struct
		{
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf];

			Unsafe.As<byte, TConvertType>(ref bytes[0]) = value;

			return bytes;
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided <see cref="value"/>
		/// </summary>
		/// <typeparam name="TConvertType"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] ReinterpretFromString(this string value)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));
			if(String.IsNullOrEmpty(value)) return new byte[0];

			return value.ToCharArray().ToByteArrayPerm();
		}
	}
}
