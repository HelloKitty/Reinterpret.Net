using System;
using System.Collections.Generic;
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
#if NETSTANDARD1_0 || NETSTANDARD1_1
			TypeInfo convertTypeInfo = typeof(TConvertType).GetTypeInfo();
#else
			Type convertTypeInfo = typeof(TConvertType);
#endif
			if(convertTypeInfo.IsPrimitive)
				return ReinterpretFromPrimitive(value);

			throw new NotImplementedException();
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
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Reinterprets the provided <see cref="value"/>
		/// </summary>
		/// <typeparam name="TConvertType"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] Reinterpret(this string value)
		{
			throw new NotImplementedException();
		}
	}
}
