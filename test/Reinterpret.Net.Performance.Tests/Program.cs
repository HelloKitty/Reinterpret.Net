using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Reinterpret.Net.Performance.Tests
{
	class Program
	{
		public static int TenMillion { get; } = 10000000;

		public static int OneHundredThousand { get; } = TenMillion / 100;

		public static int OneThousand { get; } = OneHundredThousand / 100;

		public static int[] testIntArray = Enumerable.Range(0, 7000).ToArray();

		public static int[][] multipleArrays = Enumerable.Repeat(testIntArray, OneThousand)
			.Select(a => a.ToArray()).ToArray();

		static void Main(string[] args)
		{
			Console.ReadKey();
			byte[] bytes = null;
			Stopwatch watch = new Stopwatch();

			string testString = "helloteststring";
			byte[] testBytes = Encoding.Unicode.GetBytes(testString);

			watch.Start();
			for(int i = 0; i < OneHundredThousand; i++)
				Encoding.Unicode.GetString(testBytes);
			watch.Stop();
			Console.WriteLine($"Encoding: {watch.ElapsedMilliseconds}");
			
			watch.Reset();
			watch.Start();
			for(int i = 0; i < OneHundredThousand; i++)
				testBytes.ReinterpretToString();

			watch.Stop();
			Console.WriteLine($"Reinterpret: {watch.ElapsedMilliseconds}");
			BitConverterInt32ToBytesTest(bytes, watch);
			watch.Reset();

			ReinterpretInt32ToBytesTest(watch);
			watch.Reset();

			BitConverterToInt32FromBytesTest(bytes, watch);
			watch.Reset();

			ReinterpretInt32FromBytesTest(watch);
			watch.Reset();

			ReinterpretInt32ArrayToBytes(watch);
			watch.Reset();

			BitConverterInt32ArrayToBytes(watch);
			watch.Reset();

			BlockCopyInt32ArrayToBytes(watch);
			watch.Reset();


			Console.ReadKey();
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

		private static void BitConverterInt32ArrayToBytes(Stopwatch watch)
		{
			//prewarm
			PauseGC();
			watch.Start();
			for(int i = 0; i < OneThousand; i++)
				testIntArray.SelectMany(j => BitConverter.GetBytes(j)).ToArray();

			watch.Stop();
			ResumeGC();

			Console.WriteLine($"BitConverter (LINQ) Int32[] to bytes: {watch.Elapsed}");
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

		public static void PauseGC()
		{
			//if(!GC.TryStartNoGCRegion(int.MaxValue / 10000000))
			//	throw new InvalidOperationException("Failed to pause GC. Must be able to pause for testing");
		}

		public static void ResumeGC()
		{
			GC.Collect();
		}
	}
}
