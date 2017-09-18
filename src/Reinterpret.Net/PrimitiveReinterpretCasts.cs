using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

		#region TOBYTES
		//TO BYTES
		internal unsafe static byte[] ReinterpretToBytes(int value)
		{
			byte[] bytes = new byte[sizeof(int)];

			fixed(byte* bPtr = &bytes[0])
				*((int*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(uint value)
		{
			byte[] bytes = new byte[sizeof(uint)];

			fixed (byte* bPtr = &bytes[0])
				*((uint*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(short value)
		{
			byte[] bytes = new byte[sizeof(short)];

			fixed (byte* bPtr = &bytes[0])
				*((short*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(ushort value)
		{
			byte[] bytes = new byte[sizeof(ushort)];

			fixed (byte* bPtr = &bytes[0])
				*((ushort*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(float value)
		{
			byte[] bytes = new byte[sizeof(float)];

			fixed (byte* bPtr = &bytes[0])
				*((float*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(double value)
		{
			byte[] bytes = new byte[sizeof(double)];

			fixed (byte* bPtr = &bytes[0])
				*((double*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(Int64 value)
		{
			byte[] bytes = new byte[sizeof(Int64)];

			fixed (byte* bPtr = &bytes[0])
				*((Int64*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(UInt64 value)
		{
			byte[] bytes = new byte[sizeof(UInt64)];

			fixed (byte* bPtr = &bytes[0])
				*((UInt64*)bPtr) = value;

			return bytes;
		}

		internal unsafe static byte[] ReinterpretToBytes(sbyte value)
		{
			byte[] bytes = new byte[sizeof(sbyte)];

			fixed (byte* bPtr = &bytes[0])
				*((sbyte*)bPtr) = value;

			return bytes;
		}
		#endregion
	}
}
