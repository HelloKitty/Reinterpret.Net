using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reinterpret.Net
{
	internal static class ThrowHelpers
	{
		//Seperate method to avoid inlining exception
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void ThrowOnlyPrimitivesException<TConvertType>()
		{
			throw new InvalidOperationException($"Cannot reinterpret Type: {typeof(TConvertType).Name} because only primitive types are supported.");
		}
	}
}
