using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	//Based on hack from: http://stackoverflow.com/questions/619041/what-is-the-fastest-way-to-convert-a-float-to-a-byte
	internal static unsafe class ArrayHackByteConversionExtensions
	{
		/// <summary>
		/// Permantely mutate the Type information and state of the provided <see cref="bytes"/>
		/// This method will leave the orignal array in an invalid state.
		/// </summary>
		/// <typeparam name="TConvertedType">The type to convert to.</typeparam>
		/// <param name="bytes">The bytes to be permantely converted.</param>
		/// <returns>A reference to the converted array.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static TConvertedType[] ToConvertedArrayPerm<TConvertedType>(this byte[] bytes)
			where TConvertedType : struct
		{
			//This is the part that converts the array from bytes to the convertedtype array
			//However type information in the Type header will be invalid at this point
			TConvertedType[] results = Unsafe.As<byte[], TConvertedType[]>(ref bytes);

			//This needs to be called to set the array size and type.
			PointerHelper.SetTypeAndSize(results, TypeIntrospector<TConvertedType>.ArrayTypeHeader, (UIntPtr)unchecked(bytes.Length / MarshalSizeOf<TConvertedType>.SizeOf));

			return results;
		}

		/// <summary>
		/// Permantely mutate the Type information and state of the provided <see cref="values"/> to become a byte array.
		/// This method will leave the orignal array in an invalid state.
		/// </summary>
		/// <typeparam name="TCurrentType">The element type of the array being converted from.</typeparam>
		/// <param name="values">The array to be permantely converted to bytes.</param>
		/// <returns>A reference to the byte representation of the array.</returns>
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#else
		[MethodImpl(256)]
#endif
		public static byte[] ToByteArrayPerm<TCurrentType>(this TCurrentType[] values)
			where TCurrentType : struct
		{
			byte[] bytes = Unsafe.As<TCurrentType[], byte[]>(ref values);

			//This needs to be called to set the array size and type.
			//However type information in the Type header will be invalid at this point
			PointerHelper.SetTypeAndSize(values, TypeIntrospector<byte>.ArrayTypeHeader, (UIntPtr)unchecked(values.Length * MarshalSizeOf<TCurrentType>.SizeOf));

			return bytes;
		}
	}
}
