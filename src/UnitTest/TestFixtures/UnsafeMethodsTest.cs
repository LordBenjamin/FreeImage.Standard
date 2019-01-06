using FreeImageAPI;
using NUnit.Framework;

namespace UnitTest.TestFixtures
{
	public unsafe class UnsafeMethodsTest
	{
		[Test]
		public void CompareMemory_Match()
		{
			uint expectedCount = 10;
			byte[] bytes1 = new byte[expectedCount];
			byte[] bytes2 = (byte[])bytes1.Clone();

			uint actualCount;

			fixed(byte* b1 = bytes1)
			fixed(byte* b2 = bytes2) {
				actualCount = UnsafeHelpers.CompareMemory(b1, b2, (uint)bytes1.Length);
			}

			Assert.AreEqual(expectedCount, actualCount); ;
		}
		[Test]
		public void CompareMemory_Different()
		{
			uint expectedCount = 6;

			byte[] bytes1 = new byte[10];
			byte[] bytes2 = (byte[])bytes1.Clone();
			bytes2[expectedCount] = 1;

			uint actualCount;

			fixed (byte* b1 = bytes1)
			fixed (byte* b2 = bytes2)
			{
				actualCount = UnsafeHelpers.CompareMemory(b1, b2, (uint)bytes1.Length);
			}

			Assert.AreEqual(expectedCount, actualCount); ;
		}
	}
}
