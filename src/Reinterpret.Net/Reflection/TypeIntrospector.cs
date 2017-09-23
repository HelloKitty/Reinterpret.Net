using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reinterpret.Net
{
	internal static class TypeIntrospector<TType>
	{
		/// <summary>
		/// Indicates if the type <typeparamref name="TType"/> is a primitive type.
		/// </summary>
		public static bool IsPrimitive { get; } =
#if !NETSTANDARD1_1
		typeof(TType).IsPrimitive;
#else
		typeof(TType).GetTypeInfo().IsPrimitive;
#endif
	}
}
