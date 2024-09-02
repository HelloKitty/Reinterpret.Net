using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reinterpret.Net
{
	public static class ReInterpretToPrimitiveExtensions
	{
		/// <summary>
		/// Safely reinterprets the provided value to the requested type with an identical memory representation.
		/// This method will protect against bitsize safety issues.
		/// </summary>
		/// <typeparam name="TFrom">The original type.</typeparam>
		/// <typeparam name="TTo">The target type.</typeparam>
		/// <param name="value">The value to reinterpret/cast.</param>
		/// <returns>The reinterpreted result of the provided value as the new type.</returns>
		public static TTo Reinterpret<TFrom, TTo>(ref this TFrom value)
			where TFrom : unmanaged
			where TTo : unmanaged
		{
			//We cannot support currently larger than 8-byte primitives.
			if (MarshalSizeOf<TFrom>.SizeOf > ReinterpretConstants.MAX_REINTERPRET_PRIMITIVE_BYTE_SIZE || MarshalSizeOf<TTo>.SizeOf > ReinterpretConstants.MAX_REINTERPRET_PRIMITIVE_BYTE_SIZE)
				throw new InvalidOperationException($"{typeof(TFrom).Name} to {typeof(TTo).Name} is not supported for primitive reinterpret. Byte size is too large.");

			// Allocate a buffer to safely store the data
			Span<byte> buffer = stackalloc byte[ReinterpretConstants.MAX_REINTERPRET_PRIMITIVE_BYTE_SIZE];

			// Copy the value into the buffer with proper alignment considerations
			Unsafe.WriteUnaligned(ref buffer[0], value);

			// Read the value from the buffer as the target type
			return Unsafe.ReadUnaligned<TTo>(ref buffer[0]);
		}
	}
}
