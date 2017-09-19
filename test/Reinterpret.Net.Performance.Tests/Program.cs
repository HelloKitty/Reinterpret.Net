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

		static void Main(string[] args)
		{
			Console.ReadKey();
			byte[] bytes = null;
			Stopwatch watch = new Stopwatch();
			BitConverterInt32ToBytesTest(bytes, watch);
			watch.Reset();

			ReinterpretInt32ToBytesTest(watch);
			watch.Reset();

			BitConverterToInt32FromBytesTest(bytes, watch);
			watch.Reset();

			ReinterpretInt32FromBytesTest(watch);

			Console.ReadKey();
		}

		private static void ReinterpretInt32ToBytesTest(Stopwatch watch)
		{
			//prewarm
			5.Reinterpret();
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
				5.Reinterpret()[0] = 6;
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

		private static void BitConverterInt32ToBytesTest(byte[] bytes, Stopwatch watch)
		{
			//prewarm
			bytes = BitConverter.GetBytes(5);
			PauseGC();
			watch.Start();
			for(int i = 0; i < TenMillion; i++)
			{
				bytes = BitConverter.GetBytes(5);
				bytes[0] = 6;
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
