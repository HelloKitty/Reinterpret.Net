using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Static helper that computed and caches sizeof computation
	/// for a specified <typeparamref name="TType"/>.
	/// </summary>
	/// <typeparam name="TType">The structure type to determine the size of.</typeparam>
	internal static class MarshalSizeOf<TType>
		where TType : struct
	{
		/// <summary>
		/// Indicates the size of the <typeparamref name="TType"/> struct.
		/// </summary>
		internal static int SizeOf { get; } = Unsafe.SizeOf<TType>();
	}
}
