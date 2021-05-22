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
			if (MarshalSizeOf<TFrom>.SizeOf >= MarshalSizeOf<TTo>.SizeOf)
				return Unsafe.As<TFrom, TTo>(ref value);

			//TTo is larger bit size so we must carefully reinterpret to avoid out of bounds memory references.
			byte[] buffer = ArrayPool<byte>.Shared.Rent(MarshalSizeOf<TTo>.SizeOf);
			try
			{
				//Since the buffer is non-zero by default and Reinterpret TFrom value to bytes can leave higher index
				//bytes non-zero it could cause the Reinterpret to TTo to fail so we can just directly set
				//the entire buffer to the default value of TTo
				Unsafe.As<byte, TTo>(ref buffer[0]) = default;

				value.Reinterpret(buffer);
				return buffer.Reinterpret<TTo>();
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
	}
}
