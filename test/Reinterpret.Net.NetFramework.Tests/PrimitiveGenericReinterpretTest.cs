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
				.Select(l => ConvertToType<TTypeToTest>(l))
				.ToArray();

			ValuesToTest = ValuesToTest
				.Concat(GetPowerOfTwoRange(GetMinValue(), 0, sizeOfValue * 8)
				.Select(l => ConvertToType<TTypeToTest>(l)));
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

		private static UInt64 GetMaxValue()
		{
			Type convertType = typeof(TTypeToTest);

			FieldInfo field = convertType.GetField("MaxValue", BindingFlags.Static | BindingFlags.Public);

			//This can fail if the min/max value on the primitive is not standard/floatingpoint
			try
			{
				return ConvertToType<UInt64>(field.GetRawConstantValue());
			}
			catch(Exception e)
			{
				byte[] bytes = BitConverter.GetBytes((dynamic)field.GetRawConstantValue());
				bytes = Enumerable.Repeat((byte)0, 8 - bytes.Length).Concat(bytes).ToArray();
				return BitConverter.ToUInt64(bytes, 0);
			}
		}

		private static Int64 GetMinValue()
		{
			Type convertType = typeof(TTypeToTest);

			FieldInfo field = convertType.GetField("MinValue", BindingFlags.Static | BindingFlags.Public);

			//This can fail if the min/max value on the primitive is not standard/floatingpoint
			try
			{
				return ConvertToType<Int64>(field.GetRawConstantValue());
			}
			catch(Exception e)
			{
				byte[] bytes = BitConverter.GetBytes((dynamic)field.GetRawConstantValue());
				bytes = Enumerable.Repeat((byte) 0, 8 - bytes.Length).Concat(bytes).ToArray();
				return BitConverter.ToInt64(bytes, 0);
			}
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

		protected static IEnumerable<UInt64> GetPowerOfTwoRange(UInt64 mainValue, int start, int stop)
		{
			return Enumerable.Range(start, stop)
				.Select(i => mainValue / (UInt64)Math.Pow(2, i));
		}
	}
}
