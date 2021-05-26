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

			//Thanks Fabian =3
			ulong widenedValue = Unsafe.SizeOf<TFrom>() switch
			{
				1 => Unsafe.As<TFrom, byte>(ref value),
				2 => Unsafe.As<TFrom, ushort>(ref value),
				4 => Unsafe.As<TFrom, uint>(ref value),
				_ => Unsafe.As<TFrom, ulong>(ref value)
			};

			return Unsafe.As<ulong, TTo>(ref widenedValue);
		}
	}
}
