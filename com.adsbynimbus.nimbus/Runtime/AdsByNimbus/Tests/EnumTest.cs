using System;
using NUnit.Framework;

namespace Tests {
	public class EnumTest {
		[TestCase(AdSize.banner, 320, 50,
			TestName = "Check that Banner returns a 320x50 tuple")]
		[TestCase(AdSize.mrec, 300, 250,
			TestName = "Check that mrec returns a 300x250 tuple")]
		public void TestIabEnumSizes(AdSize size, int expectedWidth, int expectedHeight) {
			var (width, height) = size.ToWidthAndHeight();
			Assert.AreEqual(expectedWidth, width);
			Assert.AreEqual(expectedHeight, height);
		}
	}
}