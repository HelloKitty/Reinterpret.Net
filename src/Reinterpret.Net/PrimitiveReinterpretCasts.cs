using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

//This is a nuget package that is for unsafe .NET operations
//It only supports newer versions of .NET though
//The newer versions of dotnet utilize the Unsafe type seen in the new BitConverter https://github.com/dotnet/coreclr/blob/master/src/mscorlib/shared/System/BitConverter.cs
//In other versions of .NET the BitConverter will be much slower and our fallback pointer hacking will be quick by comparision
using System.Runtime.CompilerServices;

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

		internal unsafe static short ReinterpretToInt16(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((short*)bytePtr);
		}

		internal unsafe static ushort ReinterpretToUInt16(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((ushort*)bytePtr);
		}

		internal unsafe static float ReinterpretToFloat(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((float*)bytePtr);
		}

		internal unsafe static double ReinterpretToDouble(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((double*)bytePtr);
		}

		internal unsafe static Int64 ReinterpretToInt64(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((Int64*)bytePtr);
		}

		internal unsafe static UInt64 ReinterpretToUInt64(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((UInt64*)bytePtr);
		}

		internal unsafe static sbyte ReinterpretToSByte(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((sbyte*)bytePtr);
		}

		internal unsafe static char ReinterpretToChar(byte[] bytes)
		{
			//For performance we don't recheck the parameters.
			//fix address; See this link for information on this memory hack: http://stackoverflow.com/questions/2036718/fastest-way-of-reading-and-writing-binary
			fixed (byte* bytePtr = &bytes[0])
				return *((char*)bytePtr);
		}

		internal unsafe static byte[] ReinterpretToBytes<TType>(TType value)
			where TType : struct
		{
			byte[] bytes = new byte[MarshalSizeOf<TType>.SizeOf];

			Unsafe.As<byte, TType>(ref bytes[0]) = value;

			return bytes;
		}
	}
}
