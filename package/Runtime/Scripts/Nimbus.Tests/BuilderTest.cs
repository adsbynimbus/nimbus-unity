using Newtonsoft.Json;
using Nimbus.Internal.RequestBuilder;
using NUnit.Framework;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;


namespace Nimbus.Tests {
	public class BuilderTest {
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

		[Test]
		public void TestRewardedVideo() {
			const string expected = "{\"device\":{\"connectiontype\":0,\"devicetype\":2,\"dnt\":0,\"h\":480,\"ifa\":\"00000000-0000-0000-0000-000000000000\",\"lmt\":0,\"os\":\"Unity Editor\",\"osv\":\"0.0.2\",\"ua\":\"UnityPlayer/2020.3.34f1 personal (UnityWebRequest/1.0, libcurl/7.52.0-DEV)\",\"w\":640},\"ext\":{\"session_id\":\"foo-session\"},\"format\":{\"h\":480,\"w\":640},\"imp\":[{\"ext\":{\"position\":\"foobar\"},\"instl\":1,\"secure\":1,\"video\":{\"ext\":{\"is_rewarded\":1},\"h\":480,\"mimes\":[\"application/x-mpegurl\",\"video/mp4\",\"video/3gpp\",\"video/x-flv\"],\"pos\":7,\"protocols\":[2,3,5,6],\"skip\":0,\"startdelay\":0,\"w\":640,\"bidfloor\":1.0,\"companionad\":[{\"h\":320,\"vcm\":1,\"w\":480}]}}],\"test\":1}";
			
			var bidRequest = NimbusRtbBidRequestHelper.ForVideoInterstitialAd("foobar");
			bidRequest.
				AttemptToShowVideoEndCard().
				SetSessionId("foo-session").
				SetDevice(new Device {
					Ua = "UnityPlayer/2020.3.34f1 personal (UnityWebRequest/1.0, libcurl/7.52.0-DEV)",
					Os = "Unity Editor",
					DeviceType = DeviceType.PersonalComputer,
					Osv = Application.version,
					H = Screen.height,
					W = Screen.width,
					ConnectionType = ConnectionType.Unknown,
					Ifa = "00000000-0000-0000-0000-000000000000"
				}).
				SetVideoFloor(1.00f).
				SetRewardedVideoFlag().
				SetTest(true);
			
			
			var body = JsonConvert.SerializeObject(bidRequest);
			Assert.AreEqual(expected, body);
		}
		
	}
}