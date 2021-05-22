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
	public sealed class Int32ReinterpretCastTests : PrimitiveGenericReinterpretTest<Int32>
	{
		public Int32ReinterpretCastTests()
		{
			
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public static void TestEncounterExceptionReinterpretingIntoTooSmallBuffer(int value)
		{
			byte[] buffer = new byte[2];
			byte[] buffer2 = new byte[4];
			Assert.Throws<InvalidOperationException>(() => value.Reinterpret(buffer));
			Assert.Throws<InvalidOperationException>(() => value.Reinterpret(buffer2, 1));
		}
	}
}
