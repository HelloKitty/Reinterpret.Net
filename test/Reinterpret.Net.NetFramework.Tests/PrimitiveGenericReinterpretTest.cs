using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Reinterpret.Net.NetFramework.Tests
{
	[TestFixture]
	public abstract class PrimitiveGenericReinterpretTest<TTypeToTest>
		where TTypeToTest : struct, IComparable, IFormattable, IComparable<TTypeToTest>, IEquatable<TTypeToTest>
	{
		public static IEnumerable<TTypeToTest> ValuesToTest { get; }

		static PrimitiveGenericReinterpretTest()
		{
			int sizeOfValue = Marshal.SizeOf<TTypeToTest>();

			ValuesToTest = GetPowerOfTwoRange(GetMaxValue(), 0, sizeOfValue * 8)
				.Concat(GetPowerOfTwoRange(GetMinValue(), 0, sizeOfValue * 8))
				.Select(l => (TTypeToTest)Convert.ChangeType(l, typeof(TTypeToTest)))
				.ToArray();
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public void TestCanReinterpretToTypeType(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = BitConverter.GetBytes((dynamic)valueToTest);

			//act
			TTypeToTest value = bytes.Reinterpret<TTypeToTest>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
		}

		private static Int64 GetMaxValue()
		{
			Type convertType = typeof(TTypeToTest);

			FieldInfo field = convertType.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public);

			return ConvertToType<Int64>(field.GetRawConstantValue());
		}

		private static Int64 GetMinValue()
		{
			Type convertType = typeof(TTypeToTest);

			FieldInfo field = convertType.GetField("MinValue", BindingFlags.Static | BindingFlags.Public);

			return ConvertToType<Int64>(field.GetRawConstantValue());
		}

		private static TTypeToTest ConvertToType(object value)
		{
			return (TTypeToTest) Convert.ChangeType(value, typeof(TTypeToTest));
		}

		private static TTypeToConvertTo ConvertToType<TTypeToConvertTo>(object value)
		{
			return (TTypeToConvertTo)Convert.ChangeType(value, typeof(TTypeToConvertTo));
		}

		protected static IEnumerable<Int64> GetPowerOfTwoRange(Int64 mainValue, int start, int stop)
		{
			return Enumerable.Range(start, stop)
				.Select(i => mainValue / (Int64)Math.Pow(2, i));
		}
	}
}
