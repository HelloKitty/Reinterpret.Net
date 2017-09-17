using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	internal unsafe static class ArrayMemoryHack
	{
		[StructLayout(LayoutKind.Explicit)]
		internal struct Union
		{
			[FieldOffset(0)] public byte[] bytes;
			[FieldOffset(0)] public char[] chars;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct ArrayHeader
		{
			public UIntPtr type;
			public UIntPtr length;
		}

		internal static UIntPtr BYTE_ARRAY_TYPE { get; }

		static ArrayMemoryHack()
		{
			fixed (void* pBytes = new byte[1])
			{
				BYTE_ARRAY_TYPE = GetHeaderPointer(pBytes)->type;
			}
		}

		internal static ArrayHeader* GetHeaderPointer(void* pBytes)
		{
			return (ArrayHeader*)pBytes - 1;
		}

		public static void ConvertByteTypeToTargetType(this byte[] bytes, UIntPtr targetTypeTypePointer, int sizeOfTargetType)
		{
			if(sizeOfTargetType <= 0) throw new ArgumentOutOfRangeException(nameof(sizeOfTargetType));

			fixed (void* pArray = bytes)
			{
				var pHeader = GetHeaderPointer(pArray);
				pHeader->type = targetTypeTypePointer;
				pHeader->length = (UIntPtr)(bytes.Length / sizeOfTargetType);
			}
		}

		public static void ConvertOtherTypeToByteType(void* arrayPointer, int lengthOfCollection, int sizeOfTargetType)
		{
			var pHeader = ArrayMemoryHack.GetHeaderPointer(arrayPointer);
			pHeader->type = ArrayMemoryHack.BYTE_ARRAY_TYPE;
			pHeader->length = (UIntPtr)(lengthOfCollection / sizeOfTargetType);
		}
	}
}
