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
#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

#if NET451 || NET46 || NETSTANDARD1_1 || NETSTANDARD2_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public static byte[] ToByteArrayPerm<TConvertType>(this TConvertType[] values)
			where TConvertType : struct
		{
			byte[] bytes = Unsafe.As<TConvertType[], byte[]>(ref values);

			//This needs to be called to set the array size and type.
			//However type information in the Type header will be invalid at this point
			PointerHelper.SetTypeAndSize(values, TypeIntrospector<byte>.ArrayTypeHeader, (UIntPtr)unchecked(values.Length * MarshalSizeOf<TConvertType>.SizeOf));

			return bytes;
		}
	}
}
