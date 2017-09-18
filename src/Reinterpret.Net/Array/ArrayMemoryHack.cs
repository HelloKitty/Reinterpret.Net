using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	internal unsafe static class ArrayMemoryHack
	{
		//TODO: We can't use IReadonlyDictionary because most versions doesn't support it. Can we crate our own interface?
		public static IDictionary<Type, UIntPtr> TypeToTypePointerDictionary { get; }

		public static IDictionary<Type, int> TypeSizeDictionary { get; }

		[StructLayout(LayoutKind.Explicit)]
		internal struct Union
		{
			//This is a union of all potential types that we can cast to
			[FieldOffset(0)] public byte[] bytes;
			[FieldOffset(0)] public sbyte[] sbytes;
			[FieldOffset(0)] public char[] chars;
			[FieldOffset(0)] public bool[] bools;
			[FieldOffset(0)] public int[] ints;
			[FieldOffset(0)] public uint[] uints;
			[FieldOffset(0)] public short[] shorts;
			[FieldOffset(0)] public ushort[] ushorts;
			[FieldOffset(0)] public long[] longs;
			[FieldOffset(0)] public ulong[] ulongs;
			[FieldOffset(0)] public float[] floats;
			[FieldOffset(0)] public double[] doubles;

			/// <summary>
			/// Gets the strongly typed reference to the array in the union whose Type matches
			/// the spcified generic type argument <typeparamref name="TConvertedType"/>.
			/// </summary>
			/// <typeparam name="TConvertedType">The type of the array to return.</typeparam>
			/// <returns>The reference to the array of the specified type.</returns>
			public TConvertedType[] GetTypedArray<TConvertedType>()
			{
				Type t = typeof(TConvertedType);

				if(t == typeof(byte))
					return bytes as TConvertedType[];
				else if(t == typeof(int))
					return ints as TConvertedType[];
				else if(t == typeof(float))
					return floats as TConvertedType[];
				else if(t == typeof(double))
					return doubles as TConvertedType[];
				else if(t == typeof(char))
					return chars as TConvertedType[];
				else if(t == typeof(bool))
					return bools as TConvertedType[];
				else if(t == typeof(short))
					return shorts as TConvertedType[];
				else if(t == typeof(uint))
					return uints as TConvertedType[];
				else if(t == typeof(long))
					return longs as TConvertedType[];
				else if(t == typeof(ulong))
					return ulongs as TConvertedType[];
				else if(t == typeof(sbyte))
					return sbytes as TConvertedType[];
				else if(t == typeof(ushort))
					return ushorts as TConvertedType[];
				else
					throw new NotImplementedException();
			}
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
			Dictionary<Type, UIntPtr> typePointerDictionary = new Dictionary<Type, UIntPtr>();
			TypeToTypePointerDictionary = typePointerDictionary;

			Dictionary<Type, int> typeSizeDictionary = new Dictionary<Type, int>();
			TypeSizeDictionary = typeSizeDictionary;

			//Below we prepare all the types we can handle.
			//We want to get their type UIntPtr and store in the publicly accessible map
			//for other array hacking classes to use.
			fixed (void* p = new byte[1])
			{
				typePointerDictionary[typeof(byte)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(byte)] = sizeof(byte);
			}

			fixed (void* p = new sbyte[1])
			{
				typePointerDictionary[typeof(sbyte)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(sbyte)] = sizeof(sbyte);
			}

			fixed (void* p = new char[1])
			{
				typePointerDictionary[typeof(char)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(char)] = sizeof(char);
			}

			fixed (void* p = new bool[1])
			{
				typePointerDictionary[typeof(bool)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(bool)] = sizeof(bool);
			}

			fixed (void* p = new int[1])
			{
				typePointerDictionary[typeof(int)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(int)] = sizeof(int);
			}

			fixed (void* p = new uint[1])
			{
				typePointerDictionary[typeof(uint)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(uint)] = sizeof(uint);
			}

			fixed (void* p = new short[1])
			{
				typePointerDictionary[typeof(short)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(short)] = sizeof(short);
			}

			fixed (void* p = new ushort[1])
			{
				typePointerDictionary[typeof(ushort)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(ushort)] = sizeof(ushort);
			}

			fixed (void* p = new float[1])
			{
				typePointerDictionary[typeof(float)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(float)] = sizeof(float);
			}

			fixed (void* p = new double[1])
			{
				typePointerDictionary[typeof(double)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(double)] = sizeof(double);
			}

			fixed (void* p = new long[1])
			{
				typePointerDictionary[typeof(long)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(long)] = sizeof(long);
			}

			fixed (void* p = new ulong[1])
			{
				typePointerDictionary[typeof(ulong)] = GetHeaderPointer(p)->type;
				typeSizeDictionary[typeof(ulong)] = sizeof(ulong);
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
