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
	public sealed class BoolReinterpretCastTests
	{
		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void TestCanReinterpretToTypeType(bool valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = BitConverter.GetBytes((dynamic)valueToTest);

			//act
			bool value = bytes.Reinterpret<bool>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
		}
	}
}
