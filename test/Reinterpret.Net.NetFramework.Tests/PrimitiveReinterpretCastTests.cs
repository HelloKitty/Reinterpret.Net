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
		private static IEnumerable<int> int32sToTest = Enumerable.Range(-10, 10)
			.ToArray();

		[Test]
		[TestCaseSource(nameof(int32sToTest))]
		[TestCase(int.MaxValue)]
		[TestCase(int.MinValue)]
		[TestCase(int.MaxValue / 2 + 1)]
		[TestCase(int.MinValue / 2 - 1)]
		[TestCase(int.MaxValue / 4 + 1)]
		[TestCase(int.MinValue / 4 - 1)]
		[TestCase(int.MaxValue / 8 + 1)]
		[TestCase(int.MinValue / 8 - 1)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(-1)]
		public static void TestCanReinterpretToInt32(int valueToTest)
		{
			//arrange
			byte[] bytes = BitConverter.GetBytes(valueToTest);

			//act
			int value = bytes.Reinterpret<int>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
		}
	}
}
