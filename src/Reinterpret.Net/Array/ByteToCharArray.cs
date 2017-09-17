using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	//Based on hack from: http://stackoverflow.com/questions/619041/what-is-the-fastest-way-to-convert-a-float-to-a-byte
	internal static unsafe class ByteToCharArray
	{
		private static readonly UIntPtr CHAR_ARRAY_TYPE;

		static ByteToCharArray()
		{
			fixed (void* pChars = new char[1])
			{
				CHAR_ARRAY_TYPE = ArrayMemoryHack.GetHeaderPointer(pChars)->type;
			}
		}

		public static char[] ToCharArrayPerm(this byte[] bytes)
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
		}	
	}
}
