using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
		/// <param name="allowDestroyByteArray ">Indicates if the provided <see cref="bytes"/> array can be modified or 
		/// changed/destroyed in the process of casting. Indicating true  can yield higher performance results but the
		/// byte array must never be touched or used again. This will only work for certain types of reinterpret casting.</param>
		/// <returns>The resultant of the cast operation.</returns>
		public static unsafe TConvertType Reinterpret<TConvertType>(this byte[] bytes, bool allowDestroyByteArray = false)
			where TConvertType : struct
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));

#if NETSTANDARD1_0 || NETSTANDARD1_1
			TypeInfo convertTypeInfo = typeof(TConvertType).GetTypeInfo();
#else
			Type convertTypeInfo = typeof(TConvertType);
#endif
			if(convertTypeInfo.IsPrimitive)
				return ReinterpretPrimitive<TConvertType>(bytes);

			//We know it's not a primitive so it's a struct, either custom or made by MS/.NET.

#if !NETSTANDARD1_0
			return ReinterpretCustomStruct<TConvertType>(bytes);
#else
			throw new NotSupportedException($"Reinterpreting byte[] to structs is not supported in netstandard1.0 because it lacks the required API.");
#endif
		}

		//This feature is unavailable on Netstandard1.0 because Runtime Interop Services are NOT available
		//Even as a supplemental nuget package it required netstandard1.1
#if !NETSTANDARD1_0
		private unsafe static TConvertType ReinterpretCustomStruct<TConvertType>(byte[] bytes) 
			where TConvertType : struct
		{
			fixed(byte* p = bytes)
			{
#if NET20 || NET30 || NET35 || NET40 || NETSTANDARD1_1
				return (TConvertType)Marshal.PtrToStructure((IntPtr)p, typeof(TConvertType));
#else
				return Marshal.PtrToStructure<TConvertType>((IntPtr)p);
#endif
			}
		}
#endif

		/// <summary>
		/// Reinterprets the provided <see cref="bytes"/> in a similar fashion to C++
		/// reinterpret_cast by casting the byte chunk into the specified generic type
		/// <typeparamref name="TConvetType"/>.
		/// </summary>
		/// <typeparam name="TConvertType">The type to reinterpret to.</typeparam>
		/// <param name="bytes">The bytes chunk.</param>
		/// <param name="allowDestroyByteArray ">Indicates if the provided <see cref="bytes"/> array can be modified or 
		/// changed/destroyed in the process of casting. Indicating true  can yield higher performance results but the
		/// byte array must never be touched or used again. This will only work for certain types of reinterpret casting.</param>
		/// <returns>The resultant of the cast operation.</returns>
		public static unsafe TConvertType[] ReinterpretToArray<TConvertType>(this byte[] bytes, bool allowDestroyByteArray = false)
			where TConvertType : struct
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));

#if NETSTANDARD1_0 || NETSTANDARD1_1
			TypeInfo convertTypeInfo = typeof(TConvertType).GetTypeInfo();
#else
			Type convertTypeInfo = typeof(TConvertType);
#endif
			//We can only handle primitive arrays
			if(convertTypeInfo.IsPrimitive)
				return ReinterpretPrimitiveArray<TConvertType>(bytes);

			throw new NotImplementedException();
		}

		private static TConvertType[] ReinterpretPrimitiveArray<TConvertType>(byte[] bytes, bool allowDestroyByteArray = false)
			where TConvertType : struct
		{
			//If someone happens to ask for the byte representation of bytes
			if(typeof(TConvertType) == typeof(byte))
				return bytes as TConvertType[];

			return allowDestroyByteArray ? bytes.ToConvertedArrayPerm<TConvertType>() : bytes.ToArray().ToConvertedArrayPerm<TConvertType>();
		}

		/// <summary>
		/// High performance reinterpret cast for the <see cref="bytes"/> converting
		/// the byte chunk to a <see cref="string"/> using Unicode encoding (2byte char).
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
			char[] chars = allowDestroyByteArray ? bytes.ToConvertedArrayPerm<char>() : bytes.ToArray().ToConvertedArrayPerm<char>();
			return new string(chars);
		}

		private static unsafe TConvertType ReinterpretPrimitive<TConvertType>(byte[] bytes)
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
				return (TConvertType)(object)(bytes[0] != 0);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
