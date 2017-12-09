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
		where TTypeToTest : struct, IComparable, IComparable<TTypeToTest>, IEquatable<TTypeToTest>
	{
		public static IEnumerable<TTypeToTest> ValuesToTest { get; }

		static PrimitiveGenericReinterpretTest()
		{
			int sizeOfValue = Marshal.SizeOf(typeof(TTypeToTest));

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
			byte[] buffer = new byte[255];

			for(int i = 0; i < bytes.Length; i++)
				buffer[i + 25] = bytes[i];

			//act
			TTypeToTest value = buffer.Reinterpret<TTypeToTest>(25);

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
			Assert.AreEqual(valueToTest.GetType(), value.GetType(), $"Result from reinterpret cast was not the same Type.");
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public void TestDoestCrashIfInvalidByteArrayLengthForType(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = BitConverter.GetBytes((dynamic)valueToTest);
			bytes = bytes.Skip(1).ToArray();

			//assert
			Assert.DoesNotThrow(() => bytes.Reinterpret<TTypeToTest>());
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public void TestCanReinterpretToTypeTypeWithExistingBuffer(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = BitConverter.GetBytes((dynamic)valueToTest);

			//act
			TTypeToTest value = bytes.Reinterpret<TTypeToTest>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
			Assert.AreEqual(valueToTest.GetType(), value.GetType(), $"Result from reinterpret cast was not the same Type.");
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public unsafe void TestCanReinterpretToTypeTypeWithExistingArrayPointer(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = BitConverter.GetBytes((dynamic)valueToTest);
			byte* bytesAtackAlloc = stackalloc byte[bytes.Length];

			for(int i = 0; i < bytes.Length; i++)
				bytesAtackAlloc[i] = bytes[i];

			//act
			TTypeToTest value = ((IntPtr)bytesAtackAlloc).Reinterpret<TTypeToTest>();

			//assert
			Assert.AreEqual(valueToTest, value, $"Result from reinterpret cast was not the same.");
			Assert.AreEqual(valueToTest.GetType(), value.GetType(), $"Result from reinterpret cast was not the same Type.");
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public void TestCanReinterpretValueToBytes(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = null;

			//For testing purposes we can't use GetBytes on the byte type
			if(typeof(TTypeToTest) == typeof(byte))
				bytes = new byte[] { (byte)(object)valueToTest };
			else if(typeof(TTypeToTest) == typeof(sbyte))
				unchecked
				{
					bytes = new byte[] { (byte)(sbyte)(object)valueToTest };
				}
			else
				bytes = BitConverter.GetBytes((dynamic)valueToTest);
			

			//act
			byte[] result = valueToTest.Reinterpret();

			//assert
			for(int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(bytes[i], result[i], $"Result index: {i} from reinterpret cast was not the same.");

			Assert.IsTrue(result.GetType() == typeof(byte[]));
		}

		[Test]
		[TestCaseSource(nameof(ValuesToTest))]
		public void TestCanReinterpretValueIntoBytes(TTypeToTest valueToTest)
		{
			//arrange
			//We abuse the DLR so that we can keep this generic
			byte[] bytes = null;
			byte[] resultBytes = new byte[255];

			//For testing purposes we can't use GetBytes on the byte type
			if(typeof(TTypeToTest) == typeof(byte))
				bytes = new byte[] { (byte)(object)valueToTest };
			else if(typeof(TTypeToTest) == typeof(sbyte))
				unchecked
				{
					bytes = new byte[] { (byte)(sbyte)(object)valueToTest };
				}
			else
				bytes = BitConverter.GetBytes((dynamic)valueToTest);


			//act
			valueToTest.Reinterpret(resultBytes, 5);

			//assert
			for(int i = 0; i < bytes.Length; i++)
				Assert.AreEqual(bytes[i], resultBytes[i + 5], $"Result index: {i} from reinterpret cast was not the same.");

			Assert.IsTrue(resultBytes.GetType() == typeof(byte[]));
		}

		//This tests the TTypeToTest[] reinterpetability
		[Test]
		public void TestCanReinterpretToArrayType()
		{
			//arrange
			byte[] realBytes = ValuesToTest
				.SelectMany(v => typeof(TTypeToTest) == typeof(byte) || typeof(TTypeToTest) == typeof(sbyte) ? new byte[]{(byte)(dynamic)v} : (byte[])BitConverter.GetBytes((dynamic)v))
				.ToArray();

			//act
			TTypeToTest[] result = realBytes.ReinterpretToArray<TTypeToTest>();

			//assert
			TTypeToTest[] expectedResult = ValuesToTest.ToArray();
			Assert.AreEqual(expectedResult.Length, result.Length, $"Calculated invalid Length for Type: {typeof(TTypeToTest).Name}");

			for(int i = 0; i < expectedResult.Length; i++)
				Assert.AreEqual(expectedResult[i], result[i]);

			Assert.IsTrue(result.GetType() == typeof(TTypeToTest[]));
		}

		[Test]
		public void TestThrowsOnReinterpretToArrayTypeIfSourceBytesAreInvalidLengthForElementType()
		{
			//These will always be valid
			if(typeof(TTypeToTest) == typeof(byte) || typeof(TTypeToTest) == typeof(bool) || typeof(TTypeToTest) == typeof(sbyte))
				return;

			//arrange
			byte[] realBytes = ValuesToTest
				.SelectMany(v => typeof(TTypeToTest) == typeof(byte) || typeof(TTypeToTest) == typeof(sbyte) ? new byte[] { (byte)(dynamic)v } : (byte[])BitConverter.GetBytes((dynamic)v))
				.ToArray();

			IList<byte> byteList = realBytes.ToList();
			byteList.Add(0);

			realBytes = byteList.ToArray();

			//act
			Assert.Throws<InvalidOperationException>(() => realBytes.ReinterpretToArray<TTypeToTest>(), $"Expected invalid length for Type: {typeof(TTypeToTest).Name} but didn't throw");
		}

		//This tests the TTypeToTest[] reinterpetability
		[Test]
		public void TestCanReinterpretToArrayTypeWithIntactTypeIntrospection()
		{
			//arrange
			byte[] realBytes = ValuesToTest
				.SelectMany(v => typeof(TTypeToTest) == typeof(byte) || typeof(TTypeToTest) == typeof(sbyte) ? new byte[] { (byte)(dynamic)v } : (byte[])BitConverter.GetBytes((dynamic)v))
				.ToArray();

			//act
			TTypeToTest[] result = realBytes.ReinterpretToArray<TTypeToTest>();

			//assert
			TTypeToTest[] expectedResult = ValuesToTest.ToArray();

			Assert.AreEqual(expectedResult.GetType(), result.GetType());
			Assert.AreEqual(expectedResult.GetType().GetElementType(), result.GetType().GetElementType());
			Assert.AreEqual(expectedResult.GetType().Name, result.GetType().Name);
			Assert.AreEqual(expectedResult.GetType().UnderlyingSystemType, result.GetType().UnderlyingSystemType);
			Assert.AreEqual(expectedResult.ToArray().GetType(), result.ToArray().GetType());
			Assert.AreEqual(expectedResult.ToList().GetType(), result.ToList().GetType());
			Assert.AreEqual(expectedResult.ToList().Count(), result.ToList().Count());
			Assert.AreEqual(expectedResult.ToList().Where(t => true).ToArray().GetType(), result.ToList().Where(t => true).ToArray().GetType());
		}

		//This tests the TTypeToTest[] to bytes reinterpetability
		[Test]
		public void TestCanReinterpretFromArrayType()
		{
			//arrange
			byte[] realBytes = ValuesToTest
				.SelectMany(v => typeof(TTypeToTest) == typeof(byte) || typeof(TTypeToTest) == typeof(sbyte) ? new byte[] { (byte)(dynamic)v } : (byte[])BitConverter.GetBytes((dynamic)v))
				.ToArray();

			//act
			byte[] result = realBytes.ReinterpretToArray<TTypeToTest>()
				.Reinterpret();

			//assert
			Assert.AreEqual(realBytes.Length, realBytes.Length, $"Calculated invalid Length for Type: {typeof(TTypeToTest).Name}");

			for(int i = 0; i < realBytes.Length; i++)
				Assert.AreEqual(realBytes[i], result[i]);
		}

		//Tests to see that empty arrays are handleable
		[Test]
		public void TestCanReinterpretFromEmptyArrayType()
		{
			//arrange
			byte[] realBytes = new byte[0];

			//act
			byte[] result = realBytes.ReinterpretToArray<TTypeToTest>()
				.Reinterpret();

			//assert
			Assert.AreEqual(realBytes.Length, realBytes.Length, $"Calculated invalid Length for Type: {typeof(TTypeToTest).Name}");

			for(int i = 0; i < realBytes.Length; i++)
				Assert.AreEqual(realBytes[i], result[i]);
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
