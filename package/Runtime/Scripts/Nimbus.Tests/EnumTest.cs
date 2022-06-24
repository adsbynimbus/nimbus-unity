using System;
using Nimbus.Internal.RequestBuilder;
using NUnit.Framework;

namespace Nimbus.Tests {
	public class EnumTest {
		[TestCase(IabSupportedAdSizes.Banner320X50, 320, 50,
			TestName = "Check that Banner320X50 returns a 320x50 tuple")]
		[TestCase(IabSupportedAdSizes.FullScreenPortrait, 320, 480,
			TestName = "Check that FullScreenPortrait returns a 320x480 tuple")]
		public void TestIabEnumSizes(IabSupportedAdSizes size, int expectedWidth, int expectedHeight) {
			var (width, height) = size.ToWidthAndHeight();
			Assert.AreEqual(expectedWidth, width);
			Assert.AreEqual(expectedHeight, height);
		}
	}
}