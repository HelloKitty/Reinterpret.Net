using System;
using System.Collections.Generic;
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
			where TConvertType : struct, IComparable, IFormattable, IComparable<TConvertType>, IEquatable<TConvertType>
		{
			if(bytes == null) throw new ArgumentNullException(nameof(bytes));
			if(bytes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));

			TypeInfo convertTypeInfo = typeof(TConvertType).GetTypeInfo();

			if(convertTypeInfo.IsPrimitive)
				return ReinterpretPrimitive<TConvertType>(bytes);

			throw new NotImplementedException();
		}

		private static unsafe TConvertType ReinterpretPrimitive<TConvertType>(byte[] bytes)
			where TConvertType : struct, IComparable, IFormattable, IComparable<TConvertType>, IEquatable<TConvertType>
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
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
