using System;
using Newtonsoft.Json;
using Nimbus.Internal.RequestBuilder;
using NUnit.Framework;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;

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


		[Test]
		public void TestBuilder() {
			var bidRequest = NimbusRtbBidRequestHelper.ForHybridInterstitialAd("test_position");
			bidRequest.SetAppBundle("com.foo");
			bidRequest.SetDevice(new Device {
				Ua = "UnityPlayer/2020.3.34f1 personal (UnityWebRequest/1.0, libcurl/7.52.0-DEV)",
				Os = "Unity Editor",
				DeviceType = DeviceType.PersonalComputer,
				Osv = Application.version,
				H = Screen.height,
				W = Screen.width,
				ConnectionType = ConnectionType.Unknown,
				Ifa = "00000000-0000-0000-0000-000000000000"
			});
			bidRequest.SetSessionId("foobar");

			void TestSerialize() {
				JsonConvert.SerializeObject(bidRequest);
			}
			Assert.DoesNotThrow(TestSerialize);
		}
	}
}