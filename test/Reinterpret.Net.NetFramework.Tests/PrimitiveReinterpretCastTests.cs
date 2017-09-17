using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Reinterpret.Net.NetFramework.Tests
{
	[TestFixture]
	public static class PrimitiveReinterpretCastTests
	{
		private static IEnumerable<int> int32sToTest = GetPowerOfTwoRange(int.MaxValue, 0, sizeof(int) * 8)
			.Concat(GetPowerOfTwoRange(int.MinValue, 0, sizeof(int) * 8))
			.Select(l => (int) l);


		[Test]
		[TestCaseSource(nameof(int32sToTest))]
		public static void TestCanReinterpretToInt32(int valueToTest)
		{

			//arrange
			byte[] bytes = BitConverter.GetBytes(valueToTest);

			//act
			int value = bytes.Reinterpret<int>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
		}

		private static IEnumerable<Int64> GetPowerOfTwoRange(Int64 mainValue, int start, int stop)
		{
			return Enumerable.Range(start, stop)
				.Select(i => mainValue / (Int64) Math.Pow(2, i));
		}
	}
}
