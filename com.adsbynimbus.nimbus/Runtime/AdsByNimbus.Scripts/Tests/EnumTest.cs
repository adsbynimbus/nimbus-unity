using System;
using NUnit.Framework;

namespace Tests {
	public class EnumTest {
		[TestCase(IabSupportedAdSizes.Banner, 320, 50,
			TestName = "Check that Banner returns a 320x50 tuple")]
		[TestCase(IabSupportedAdSizes.FullScreenPortrait, 320, 480,
			TestName = "Check that FullScreenPortrait returns a 320x480 tuple")]
		public void TestIabEnumSizes(IabSupportedAdSizes size, int expectedWidth, int expectedHeight) {
			var (width, height) = size.ToWidthAndHeight();
			Assert.AreEqual(expectedWidth, width);
			Assert.AreEqual(expectedHeight, height);
		}
	}
}