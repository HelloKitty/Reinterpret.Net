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
		{
			var union = new ArrayMemoryHack.Union() { bytes = bytes };
			union.bytes.ConvertByteTypeToTargetType(ArrayMemoryHack.TypeToTypePointerDictionary[typeof(TConvertedType)], 
				ArrayMemoryHack.TypeSizeDictionary[typeof(TConvertedType)]);

			return union.GetTypedArray<TConvertedType>();
		}

		/*public static char[] ToCharArrayPerm(this byte[] bytes)
		{
			var union = new ArrayMemoryHack.Union() { bytes = bytes };
			union.bytes.ConvertByteTypeToTargetType(CHAR_ARRAY_TYPE, sizeof(char));
			return union.chars;
		}

		private static byte[] ToByteArrayPerm(this char[] charArray)
		{
			var union = new ArrayMemoryHack.Union() { chars = charArray };

			fixed (void* cArray = charArray)
			{
				ArrayMemoryHack.ConvertOtherTypeToByteType(cArray, charArray.Length, sizeof(char));
			}

			return union.bytes;
		}*/
	}
}
