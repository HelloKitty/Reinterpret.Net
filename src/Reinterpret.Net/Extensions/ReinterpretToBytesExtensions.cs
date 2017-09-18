using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Extension methods that cast to <see cref="byte"/>s from
	/// the specified target Type.
	/// </summary
	public static class ReinterpretToBytesExtensions
	{
		public static byte[] Reinterpret<TConvertType>(this TConvertType value)
			where TConvertType : struct
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Reinterprets the provided <see cref="value"/>
		/// </summary>
		/// <typeparam name="TConvertType"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static byte[] Reinterpret<TConvertType>(this string value)
		{
			throw new NotImplementedException();
		}
	}
}
