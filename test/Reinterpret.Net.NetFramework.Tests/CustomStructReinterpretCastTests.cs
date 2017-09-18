using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reinterpret.Net.NetFramework.Tests
{
	[TestFixture]
	public class CustomStructReinterpretCastTests
	{
		[Test]
		[TestCase('h', 5)]
		[TestCase('j', int.MinValue)]
		[TestCase((char)0, int.MaxValue)]
		public static void Test_Can_Reinterpret_Simple_ValueOnlyStruct(char charValue, int intValue)
		{
			//arrange
			TestCustomStruct1 testStructValue = new TestCustomStruct1() {c = charValue, i = intValue};
			byte[] buffer = new byte[Marshal.SizeOf<TestCustomStruct1>()];
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TestCustomStruct1>());

			//act
			Marshal.StructureToPtr(testStructValue, ptr, false);
			Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf<TestCustomStruct1>());
			TestCustomStruct1 resultStructValue = buffer.Reinterpret<TestCustomStruct1>();

			//assert
			Assert.NotNull(buffer);

			Assert.AreEqual(testStructValue.i, resultStructValue.i);
			Assert.AreEqual(testStructValue.c, resultStructValue.c);
		}

		[Test]
		[TestCase('h', new int[]{5})]
		[TestCase('j', new int[] { int.MinValue, 5, 6, 7})]
		[TestCase((char)0, new int[] { int.MinValue, 99, 5533})]
		public unsafe static void Test_Can_Reinterpret_CustomStruct_Containing_Array(char charValue, int[] intValues)
		{
			//arrange
			TestCustomStruct2 testStructValue = new TestCustomStruct2() { c = charValue, i = intValues };

			byte[] buffer = new byte[Marshal.SizeOf<TestCustomStruct2>()];
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TestCustomStruct2>());

			//act
			Marshal.StructureToPtr(testStructValue, ptr, false);
			Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf<TestCustomStruct2>());
			TestCustomStruct2 resultStructValue = buffer.Reinterpret<TestCustomStruct2>();

			//assert
			Assert.NotNull(buffer);

			for(int i = 0; i < intValues.Length; i++)
				Assert.AreEqual(intValues[i], resultStructValue.i[i]);
			Assert.AreEqual(testStructValue.c, resultStructValue.c);
		}

		[Test]
		[TestCase('h', "Test")]
		[TestCase('j', "hello")]
		[TestCase((char)0, @"lololodfsidnfsindf8*H*ASDH*ASDHd")]
		public unsafe static void Test_Can_Reinterpret_CustomStruct_Containing_String(char charValue, string stringValue)
		{
			//arrange
			TestCustomStruct3 testStructValue = new TestCustomStruct3() { c = charValue, s = stringValue };

			byte[] buffer = new byte[Marshal.SizeOf<TestCustomStruct2>()];
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TestCustomStruct2>());

			//act
			Marshal.StructureToPtr(testStructValue, ptr, false);
			Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf<TestCustomStruct3>());
			TestCustomStruct3 resultStructValue = buffer.Reinterpret<TestCustomStruct3>();

			//assert
			Assert.NotNull(buffer);
			Assert.NotNull(resultStructValue.s);
			Assert.AreEqual(stringValue, resultStructValue.s);
			Assert.AreEqual(testStructValue.c, resultStructValue.c);
		}

		[Test]
		[TestCase('h', "Test")]
		[TestCase('j', "hello")]
		[TestCase((char)0, @"lololodfsidnfsindf8*H*ASDH*ASDHd")]
		public unsafe static void Test_Can_Reinterpret_CustomStruct_Containing_RefType(char charValue, string stringValue)
		{
			//arrange
			TestCustomStruct4 testStructValue = new TestCustomStruct4() { c = charValue, refType = new TestRefType() { s = stringValue}};

			byte[] buffer = new byte[Marshal.SizeOf<TestCustomStruct2>()];
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TestCustomStruct2>());

			//act
			Marshal.StructureToPtr(testStructValue, ptr, false);
			Marshal.Copy(ptr, buffer, 0, Marshal.SizeOf<TestCustomStruct4>());
			TestCustomStruct4 resultStructValue = buffer.Reinterpret<TestCustomStruct4>();

			//assert
			Assert.NotNull(buffer);
			Assert.NotNull(resultStructValue.refType);
			Assert.AreEqual(stringValue, resultStructValue.refType.s);
			Assert.AreEqual(testStructValue.c, resultStructValue.c);
		}

		public class TestRefType
		{
			public string s;
		}

		public struct TestCustomStruct1
		{
			public int i;
			public char c;
		}

		public unsafe struct TestCustomStruct2
		{
			public int[] i;

			public char c;
		}

		public unsafe struct TestCustomStruct3
		{
			public string s;

			public char c;
		}

		public unsafe struct TestCustomStruct4
		{
			public TestRefType refType;

			public char c;
		}
	}
}
