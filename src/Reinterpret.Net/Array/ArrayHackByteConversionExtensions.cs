using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	//Based on hack from: http://stackoverflow.com/questions/619041/what-is-the-fastest-way-to-convert-a-float-to-a-byte
	internal static unsafe class ArrayHackByteConversionExtensions
	{
		public static TConvertedType[] ToConvertedArrayPerm<TConvertedType>(this byte[] bytes)
			where TConvertedType : struct
		{
			var union = new ArrayMemoryHack.Union() { bytes = bytes };
			union.bytes.ConvertByteTypeToTargetType(ArrayMemoryHack.TypeToTypePointerDictionary[typeof(TConvertedType)], MarshalSizeOf<TConvertedType>.SizeOf);

			return union.GetTypedArray<TConvertedType>();
		}

		public static byte[] ToByteArrayPerm<TConvertType>(this TConvertType[] values)
			where TConvertType : struct
		{
			var union = new ArrayMemoryHack.Union();
			union.SetTypedArray(values);

			union.bytes.ConvertTypeToByteType(values.Length, MarshalSizeOf<TConvertType>.SizeOf);

			return union.bytes;
		}
	}
}
