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
			where TConvertType : unmanaged
		{
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
		public static void Reinterpret<TConvertType>(this TConvertType value, Span<byte> bytes, int start = 0)
			where TConvertType : unmanaged
		{
			ReinterpretFromPrimitive(value, bytes, start);
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
			where TConvertType : unmanaged
		{
			//Don't check if null. It's a lot faster not to
			if(values.Length == 0) return Array.Empty<byte>();

			//BlockCopy is slightly faster if we have to reallocate
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf * values.Length];

			Reinterpret(values, bytes, 0);

			return bytes;
		}

		/// <summary>
		/// Reinterprets the provided <see cref="values"/> array to the byte representation
		/// of the array.
		/// </summary>
		/// <typeparam name="TConvertType">The element type of the array.</typeparam>
		/// <param name="values">The array to reinterpret.</param>
		/// <param name="bytes">Buffer to write.</param>
		/// <param name="start">Offset.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Reinterpret<TConvertType>(this TConvertType[] values, Span<byte> bytes, int start = 0)
			where TConvertType : unmanaged
		{
			// AI suggested a checked section, unlikely to overflow though so I refused
			uint byteSize = (uint)(MarshalSizeOf<TConvertType>.SizeOf * values.Length);

			if (start < 0 || (start + byteSize) > bytes.Length)
				ThrowBufferTooSmallException();

			fixed (TConvertType* ptr = values)
			{
				byte* destPtr = (byte*) ptr;
				Unsafe.CopyBlockUnaligned(ref bytes[start], ref destPtr[0], byteSize);
			}
		}

		private static void ThrowBufferTooSmallException() 
		{
			throw new ArgumentOutOfRangeException($"Buffer is too small to contain the reinterpreted data.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] ReinterpretFromPrimitive<TConvertType>(TConvertType value) 
			where TConvertType : unmanaged
		{
			byte[] bytes = new byte[MarshalSizeOf<TConvertType>.SizeOf];
			ReinterpretFromPrimitive(value, bytes);
			return bytes;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ReinterpretFromPrimitive<TConvertType>(TConvertType value, Span<byte> bytes, int start = 0)
			where TConvertType : unmanaged
		{
			if (bytes.Length - start < MarshalSizeOf<TConvertType>.SizeOf)
				ThrowHelpers.ThrowBufferTooSmall<TConvertType>(bytes.Length - start);

			Unsafe.As<byte, TConvertType>(ref bytes[start]) = value;
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided UTF16 string into its byte representation.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The byte represenation of the UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe byte[] Reinterpret(this string value)
		{
			var span = new Span<byte>(new byte[Encoding.Unicode.GetByteCount(value)]);
			Reinterpret(value, span);
			return span.ToArray();
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided UTF16 string into its byte representation.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="encoding">Optional encoding (default is unicode)</param>
		/// <returns>The byte represenation of the UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Reinterpret(this string value, Span<byte> buffer, int offset = 0, Encoding encoding = null)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));
			if(String.IsNullOrEmpty(value)) return;

			buffer = buffer.Slice(offset);

			fixed(char* chars = value)
			fixed (byte* bytes = &buffer.GetPinnableReference())
			{
				if (encoding != null)
					encoding.GetBytes(chars, value.Length, bytes, buffer.Length);
				else
					Encoding.Unicode.GetBytes(chars, value.Length, bytes, buffer.Length);
			}
		}

		//TODO: Can we access the underlying char array as UTF16 without copying? unions produce ASCII encoded array
		/// <summary>
		/// Reinterprets the provided UTF16 string into its byte representation.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="encoding">Optional encoding, default is Unicode.</param>
		/// <returns>The byte represenation of the UTF16 string.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Reinterpret(this string value, Encoding encoding)
		{
			var span = new Span<byte>(new byte[encoding.GetByteCount(value)]);
			Reinterpret(value, span, 0, encoding);
			return span.ToArray();
		}
	}
}
