using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace Reinterpret.Net.NetFramework.Tests
{
	[TestFixture]
	public class StringReinterpretCastTests
	{
		[Test]
		[TestCase("test")]
		[TestCase("")]
		[TestCase(" ")]
		[TestCase("\t")]
		[TestCase("Heloooooooooooooooooooooooo")]
		[TestCase(@"asd89asd98ahsd893598nioasop)*(AJ*(D(*SHDF(H#%*(H*F(Snsf")]
		public void Test_Can_Reinterpret_To_ASCII_String(string value)
		{
			//arrange
			byte[] bytes = Encoding.ASCII.GetBytes(value);

			//act
			string result = bytes.Reinterpret(Encoding.ASCII);

			//assert
			Assert.AreEqual(value, result, $"Expected: {value} Result: {result}");
		}
	}
}
