using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Extension methods that cast from <see cref="byte"/>s to
	/// the specified target Type.
	/// </summary>
	public static class ReInterpretFromBytesExtensions
	{
		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> in a similar fashion to C++
		/// reinterpret_cast by casting the byte chunk into the specified generic type
		/// <typeparamref name="TConvetType"/>.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <returns>The resultant of the cast operation.</returns>
		public static unsafe TConvertType Reinterpret<TConvertType>(this byte[] bytes)
			where TConvertType : struct
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));


#if NETSTANDARD1_0
			TypeInfo convertTypeInfo = typeof(TConvertType).GetTypeInfo();
#else
			Type convertTypeInfo = typeof(TConvertType);
#endif

			if(convertTypeInfo.IsPrimitive)
				return ReinterpretPrimitive<TConvertType>(bytes);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> in a similar fashion to C++
		/// reinterpret_cast by casting the byte chunk to a <see cref="string"/> of the specified
		/// <see cref="Encoding"/>.
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="allowDestroyByteArray ">Indicates if the provided <see cref="bytes"/> array can be modified or 
		/// changed/destroyed in the process of casting. Indicates that it can will yield higher performance results but the
		/// byte array must never be touched or used again. </param>
		/// <returns>The resultant of the cast operation.</returns>
		public static unsafe string Reinterpret(this byte[] bytes, Encoding encoding, bool allowDestroyByteArray = false)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(encoding == null) throw new ArgumentNullException(nameof(encoding));
			if(bytes.Length == 0) return "";

			//If it's unicode we can just reinterpret the byte array as a 2byte char array
			if(encoding == Encoding.Unicode)
			{
				return bytes.ReinterpretToString();
			}

#if !NETSTANDARD1_0
			//TODO: This isn't fast, it's at best as slow as usual for most C# devs.
			return encoding.GetString(bytes);
#else
			//TODO: This isn't fast, it's at best as slow as usual for most C# devs.
			return encoding.GetString(bytes, 0, bytes.Length);
#endif
		}

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> in a similar fashion to C++
		/// reinterpret_cast by casting the byte chunk to a <see cref="string"/> using Unicode
		/// encoding (2byte char).
		/// </summary>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="allowDestroyByteArray ">Indicates if the provided <see cref="bytes"/> array can be modified or 
		/// changed/destroyed in the process of casting. Indicates that it can will yield higher performance results but the
		/// byte array must never be touched or used again. </param>
		/// <returns>The resultant of the cast operation.</returns>
		public static unsafe string ReinterpretToString(this byte[] bytes, bool allowDestroyByteArray = false)
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) return "";

			//The caller may want to reuse the byte array so we check if they will allow us to destroy it
			char[] chars = allowDestroyByteArray ? bytes.ToCharArrayPerm() : bytes.ToArray().ToCharArrayPerm();
			return new string(chars);
		}

		private static unsafe TConvertType ReinterpretPrimitive<TConvertType>(byte[] bytes)
			where TConvertType : struct
		{
			//For performance we don't recheck the parameters.

			Type convertType = typeof(TConvertType);

			//.NET does not support Type switch cases
			if(convertType == typeof(int))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToInt32(bytes);
			}
			else if(convertType == typeof(Int64))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToInt64(bytes);
			}
			else if(convertType == typeof(float))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToFloat(bytes);
			}
			else if(convertType == typeof(double))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToDouble(bytes);
			}
			else if(convertType == typeof(short))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToInt16(bytes);
			}
			else if(convertType == typeof(ushort))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToUInt16(bytes);
			}
			else if(convertType == typeof(UInt32))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToUInt32(bytes);
			}
			else if(convertType == typeof(UInt64))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToUInt64(bytes);
			}
			else if(convertType == typeof(byte))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)bytes[0];
			}
			else if(convertType == typeof(sbyte))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)PrimitiveReinterpretCasts.ReinterpretToSByte(bytes);
			}
			else if(convertType == typeof(bool))
			{
				//TODO: Can we avoid this boxing somehow?
				return (TConvertType)(object)bytes[0];
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
