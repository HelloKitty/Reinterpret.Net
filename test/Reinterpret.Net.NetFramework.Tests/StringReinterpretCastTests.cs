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
		public void Test_Can_Reinterpret_To_UTF16_String(string value)
		{
			//arrange
			byte[] bytes = Encoding.Unicode.GetBytes(value);

			//act
			string result = bytes.ReinterpretToString();

			//assert
			Assert.AreEqual(value, result, $"Expected: {value} Result: {result}");
		}

		[Test]
		[TestCase("test")]
		[TestCase("")]
		[TestCase(" ")]
		[TestCase("\t")]
		[TestCase("Heloooooooooooooooooooooooo")]
		[TestCase(@"asd89asd98ahsd893598nioasop)*(AJ*(D(*SHDF(H#%*(H*F(Snsf")]
		public void Test_Can_Reinterpret_From_UTF16_String(string value)
		{
			//arrange
			byte[] bytes = Encoding.Unicode.GetBytes(value);

			//act
			byte[] result = value.Reinterpret();

			//assert
			Assert.AreEqual(bytes.Length, result.Length);
			for(int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(bytes[i], result[i], $"Expected: {bytes[i]} Result: {result[i]}");
		}
	}
}
