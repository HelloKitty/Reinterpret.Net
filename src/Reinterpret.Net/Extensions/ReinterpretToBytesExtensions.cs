using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Extension methods that cast to <see cref="byte"/>s from
	/// the specified target Type.
	/// </summary>
	public static class ReinterpretToBytesExtensions
	{
		/// <summary>
		/// Reinterprets the provided <see cref="value"/> to the byte array representation.
		/// </summary>
		/// <typeparam name="TConvertType">The type of the value.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <returns>The byte array representation of the value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Reinterpret<TConvertType>(this TConvertType value)
			where TConvertType : struct
		{
			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretFromPrimitive(value);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="value"/> to the byte array representation.
		/// </summary>
		/// <typeparam name="TConvertType">The type of the value.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <param name="bytes">The buffer to write/reinterpret into.</param>
		/// <param name="start">The position in the buffer to start writing into.</param>
		/// <returns>The passed in byte array for fluent chaining.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Reinterpret<TConvertType>(this TConvertType value, byte[] bytes, int start)
			where TConvertType : struct
		{
			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			return ReinterpretFromPrimitive(value, bytes, start);
		}

		/// <summary>
		/// Reinterprets the provided <see cref="values"/> array to the byte representation
		/// of the array.
		/// </summary>
		/// <typeparam name="TConvertType">The element type of the array.</typeparam>
		/// <param name="values">The array to reinterpret.</param>
		/// <returns>The byte represenation of the values.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe byte[] Reinterpret<TConvertType>(this TConvertType[] values)
			where TConvertType : struct
		{
			//Don't check if null. It's a lot faster not to
			if(values.Length == 0) return new byte[0];

			if(!TypeIntrospector<TConvertType>.IsPrimitive)
				ThrowHelpers.ThrowOnlyPrimitivesException<TConvertType>();

			//BlockCopy is slightly faster if we have to reallocate
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf * values.Length];

			Buffer.BlockCopy(values, 0, bytes, 0, MarshalSizeOf<TConvertType>.SizeOf * values.Length);

			return bytes;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] ReinterpretFromPrimitive<TConvertType>(TConvertType value) 
			where TConvertType : struct
		{
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf];

			return ReinterpretFromPrimitive(value, bytes, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] ReinterpretFromPrimitive<TConvertType>(TConvertType value, byte[] bytes, int start)
			where TConvertType : struct
		{
			Unsafe.As<byte, TConvertType>(ref bytes[start]) = value;

			return bytes;
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided UTF16 string into its byte representation.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The byte represenation of the UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Reinterpret(this string value)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));
			if(String.IsNullOrEmpty(value)) return new byte[0];

			return Encoding.Unicode.GetBytes(value.ToCharArray());
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided UTF16 string into its byte representation.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The byte represenation of the UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Reinterpret(this string value, Encoding encoding)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));
			if(encoding == null) throw new ArgumentNullException(nameof(encoding));
			if(String.IsNullOrEmpty(value)) return new byte[0];

			return encoding.GetBytes(value.ToCharArray());
		}
	}
}
