using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Reinterpret.Net.Performance.Tests
{
	class Program
	{
		public static int TenMillion { get; } = 10000000;

		public static int OneHundredThousand { get; } = TenMillion / 100;

		public static int OneThousand { get; } = OneHundredThousand / 100;

		public static int[] testIntArray;

		public static int[][] multipleArrays;

		public static byte[][] MultipleByteStringArray;

		public static string TestString = @"inADUInsafd8hasdf98hdsf89h*(HASD*(HS*(DFbns98dbhsd98dbdgf98bdguisbng98sdng98nSFN*()S(*nSF98nsf89ns89fn*(*(NAMIOPSDOPDSFOP<DX<O";

		static void Main(string[] args)
		{
			try
			{
				testIntArray = new int[7000];

				for(int i = 0; i < testIntArray.Length; i++)
					testIntArray[i] = i;

				multipleArrays = new int[OneThousand][];

				for(int i = 0; i < multipleArrays.Length; i++)
					multipleArrays[i] = testIntArray;

				MultipleByteStringArray = new byte[TenMillion][];
				byte[] stringBytes = TestString.Reinterpret();
				for(int i = 0; i < MultipleByteStringArray.Length; i++)
					MultipleByteStringArray[i] = stringBytes;

				Console.ReadKey();
				byte[] bytes = null;
				Stopwatch watch = new Stopwatch();
				BitConverterInt32ToBytesTest(bytes, watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretInt32ToBytesTest(watch);
				watch.Reset();

				Console.ReadKey();

				BitConverterToInt32FromBytesTest(bytes, watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretInt32FromBytesTest(watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretInt32ArrayToBytes(watch);
				watch.Reset();

				Console.ReadKey();

				BlockCopyInt32ArrayToBytes(watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretInt32ArrayToBytesWithDestroyArray(watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretStringToBytes(watch);
				watch.Reset();

				Console.ReadKey();

				EncodingUnicodeStringToBytes(watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretBytesToString(watch);
				watch.Reset();

				Console.ReadKey();

				EncodingUnicodeBytesToString(watch);
				watch.Reset();

				Console.ReadKey();

				ReinterpretBytesToStringDestroy(watch);
				watch.Reset();

				Console.ReadKey();
				
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				Console.ReadKey();
				throw;
			}
		}

		private static void ReinterpretInt32ToBytesTest(Stopwatch watch)
		{
			//prewarm
			5.Reinterpret();
			int testVal = 5;
			watch.Start();

			for(int i = 0; i < TenMillion; i++)
				testVal.Reinterpret();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret Int32 to Bytes: {watch.Elapsed}");
		}

		private static void ReinterpretInt32FromBytesTest(Stopwatch watch)
		{
			//prewarm
			byte[] bytes = 5.Reinterpret();
			int val = bytes.Reinterpret<int>();
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				val = bytes.Reinterpret<int>();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret Bytes to Int32: {watch.Elapsed}");
		}

		private static void ReinterpretInt32ArrayToBytes(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < OneThousand; i++)
				testIntArray.Reinterpret();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret Int32[] to bytes: {watch.Elapsed}");
		}

		private static void ReinterpretInt32ArrayToBytesWithDestroyArray(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < multipleArrays.Length; i++)
				multipleArrays[i].ReinterpretWithoutPreserving();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret (DESTROY) Int32[] to bytes: {watch.Elapsed}");
		}

		private static void ReinterpretStringToBytes(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				TestString.Reinterpret();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret string to bytes: {watch.Elapsed}");
		}

		private static void ReinterpretBytesToString(Stopwatch watch)
		{
			//prewarm
			byte[] bytes = TestString.Reinterpret();
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				bytes.ReinterpretToString();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Reinterpret bytes to string: {watch.Elapsed}");
		}

		private static void ReinterpretBytesToStringDestroy(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				MultipleByteStringArray[i].ReinterpretToStringWithoutPreserving();

			watch.Stop();

			Console.WriteLine($"Reinterpret (DESTROY) bytes to string: {watch.Elapsed}");
			ResumeGC();
		}

		private static void BitConverterInt32ToBytesTest(byte[] bytes, Stopwatch watch)
		{
			//prewarm
			bytes = BitConverter.GetBytes(5);
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
			{
				bytes = BitConverter.GetBytes(5);
			}
			watch.Stop();
			ResumeGC();

			Console.WriteLine($"BitConverter Int32 to Bytes: {watch.Elapsed}");
		}

		private static void BitConverterToInt32FromBytesTest(byte[] bytes, Stopwatch watch)
		{
			//prewarm
			bytes = BitConverter.GetBytes(5);
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
			{
				int val = BitConverter.ToInt32(bytes, 0);
			}
			watch.Stop();
			ResumeGC();

			Console.WriteLine($"BitConverter Bytes to Int32: {watch.Elapsed}");
		}

		private static void BlockCopyInt32ArrayToBytes(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < OneThousand; i++)
			{
				byte[] result = new byte[testIntArray.Length * sizeof(int)];
				Buffer.BlockCopy(testIntArray, 0, result, 0, result.Length);
			}

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"BlockCopy Int32[] to bytes: {watch.Elapsed}");
		}

		private static void EncodingUnicodeStringToBytes(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				Encoding.Unicode.GetBytes(TestString);

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Encoding.GetBytes string to bytes: {watch.Elapsed}");
		}

		private static void EncodingUnicodeBytesToString(Stopwatch watch)
		{
			//prewarm
			byte[] bytes = Encoding.Unicode.GetBytes(TestString);
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				Encoding.Unicode.GetString(bytes);

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"Encoding.GetString bytes to string: {watch.Elapsed}");
		}

		public static void PauseGC()
		{
			//if(!GC.TryStartNoGCRegion(int.MaxValue / 10000000))
			//	throw new InvalidOperationException("Failed to pause GC. Must be able to pause for testing");
		}

		public static void ResumeGC()
		{
			GC.Collect();
		}

		// Converts an int into an array of bytes with length 
		// four.
		[System.Security.SecuritySafeCritical]  // auto-generated
		public unsafe static byte[] GetBytes(int value)
		{
			byte[] bytes = new byte[4];
			fixed (byte* b = bytes)
				*((int*)b) = value;
			return bytes;
		}
	}
}
