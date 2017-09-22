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

		//Repro: https://github.com/HelloKitty/Reinterpret.Net/issues/1
		//Crash was being caused due to invalid length of bytes array. Espected to be a complete array for UTF16 string.
		[Test]
		public void Test_Crash_From_MismatchedSize_Of_Source_To_Destination_ArrayType_Repro()
		{
			object[] a = new object[1000];

			for(int i = 0; i < a.Length; i++)
			{
				byte[] b = new byte[i * sizeof(char)];
				string s = b.ReinterpretToString();
				a[i] = s;
				GC.Collect();
			}
		}
	}
}
