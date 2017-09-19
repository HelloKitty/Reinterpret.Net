using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		public static byte[] Reinterpret<TConvertType>(this TConvertType value)
			where TConvertType : struct
		{
			if(TypeIntrospector<TConvertType>.IsPrimitive)
				return PrimitiveReinterpretCasts.ReinterpretToBytes(value);

			//At this point it's likely to be a custom struct which must be marshalled
			return ReinterpretFromCustomStruct(value);
		}

		private unsafe static byte[] ReinterpretFromCustomStruct<TConvertType>(TConvertType value) 
			where TConvertType : struct
		{
			//TODO: Cache result of Marshal sizeof. If it even works
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf];

			MarshalValueToByteArray(value, bytes, 0);

			return bytes;
		}

		private static unsafe void MarshalValueToByteArray<TConvertType>(TConvertType value, byte[] bytes, int offset) 
			where TConvertType : struct
		{
			fixed (byte* bPtr = &bytes[offset])
			{
				//TODO: Should we delete for any reasons? Should we expose the ability?
				Marshal.StructureToPtr(value, (IntPtr)bPtr, false);
			}
		}

		/// <summary>
		/// Reinterprets the provided <see cref="value"/> array to the byte representation
		/// of the array.
		/// </summary>
		/// <typeparam name="TConvertType">The element type of the array.</typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public unsafe static byte[] Reinterpret<TConvertType>(this TConvertType[] values)
			where TConvertType : struct
		{
			if(values == null) throw new ArgumentNullException(nameof(values));
			if(values.Length == 0) return new byte[0];

			if(TypeIntrospector<TConvertType>.IsPrimitive)
				return values.ToArray().ToByteArrayPerm();

			return ReinterpretFromCustomStructArray(values);
		}

		private static unsafe byte[] ReinterpretFromCustomStructArray<TConvertType>(TConvertType[] values) 
			where TConvertType : struct
		{
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf * values.Length];

			for(int i = 0; i < values.Length; i++)
			{
				MarshalValueToByteArray(values[i], bytes, i * MarshalSizeOf<TConvertType>.SizeOf);
			}

			return bytes;
		}

		private static byte[] ReinterpretFromPrimitive<TConvertType>(TConvertType value) 
			where TConvertType : struct
		{
			Type convertType = typeof(TConvertType);

			//.NET does not support Type switch cases
			if(convertType == typeof(int))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((int)(object)value);
			}
			else if(convertType == typeof(Int64))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((long)(object)value);
			}
			else if(convertType == typeof(float))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((float)(object)value);
			}
			else if(convertType == typeof(double))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((double)(object)value);
			}
			else if(convertType == typeof(short))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((short)(object)value);
			}
			else if(convertType == typeof(ushort))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((ushort)(object)value);
			}
			else if(convertType == typeof(UInt32))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((uint)(object)value);
			}
			else if(convertType == typeof(UInt64))
			{
				//TODO: Can we avoid this boxing somehow?
				return PrimitiveReinterpretCasts.ReinterpretToBytes((ulong)(object)value);
			}
			else if(convertType == typeof(byte))
			{
				byte castedValue = (byte)(object)value;
				return new byte[] { castedValue };
			}
			else if(convertType == typeof(sbyte))
			{
				return PrimitiveReinterpretCasts.ReinterpretToBytes((sbyte)(object)value);
			}
			else if(convertType == typeof(bool))
			{
				bool castedValue = (bool) (object) value;
				return new byte[] {castedValue ? (byte)1 : (byte)0 };
			}
			else if(convertType == typeof(char))
			{
				return PrimitiveReinterpretCasts.ReinterpretToBytes((char)(object)value);
			}
			else
			{
				throw new NotImplementedException();
			}
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
