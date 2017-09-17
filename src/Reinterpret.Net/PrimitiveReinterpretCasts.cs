using System;
using System.Collections.Generic;
using System.Text;

namespace Reinterpret.Net
{
	/// <summary>
	/// Internal methods for reinterpreting involving primitive types.
	/// </summary>
	internal static class PrimitiveReinterpretCasts
	{
		internal unsafe static int ReinterpretToInt32(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((int*)bytePtr);
		}

		internal unsafe static uint ReinterpretToUInt32(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((uint*)bytePtr);
		}
	}
}
