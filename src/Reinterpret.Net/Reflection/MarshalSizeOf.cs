using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Reinterpret.Net
{
	/// <summary>
	/// Static helper that computed and caches the <see cref="Marshal.SizeOf"/> computation
	/// for a specified <typeparamref name="TType"/>.
	/// </summary>
	/// <typeparam name="TType">The structure type to determine the size of.</typeparam>
	internal static class MarshalSizeOf<TType>
		where TType : struct
	{
		/// <summary>
		/// Indicates the size of the <typeparamref name="TType"/> struct.
		/// </summary>
		internal static int SizeOf { get; } = typeof(TType) == typeof(char) ? 2 : Marshal.SizeOf(typeof(TType));
	}
}
