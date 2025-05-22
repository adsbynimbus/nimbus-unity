using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
#if UNITY_IOS && NIMBUS_ENABLE_APS
using Nimbus.Internal.Interceptor.ThirdPartyDemand.APS;
#endif
using NUnit.Framework;
using OpenRTB.Request;

namespace Nimbus.Tests {
	public class InterceptorTest {
		const string ParseData = "{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}";

		[Test]
		public void TestApsInterceptor() {
			#if UNITY_IOS && NIMBUS_ENABLE_APS
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new Tuple<BidRequest, IInterceptor>(
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
									Aps = new JArray{
										JObject.Parse(ParseData),
									}
								}
							}
						}
					}, new ApsAndroid("foo_app_id",
						new[] {
							new ApsSlotData {
								SlotId = "rewarded_video_slot",
								APSAdUnitType = APSAdUnitType.RewardedVideo,
							},
							new ApsSlotData {
								SlotId = "interstitial_slot",
								APSAdUnitType = APSAdUnitType.InterstitialDisplay,
							},
							new ApsSlotData {
								SlotId = "banner_slot",
								APSAdUnitType = APSAdUnitType.Display300X250,
							}
						})
				),

				new Tuple<BidRequest, IInterceptor>(
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
									Aps = new JArray() {
										JObject.Parse(ParseData),
									}
								}
							}
						}
					}, new ApsIOS("foo_app_id",
						new[] {
							new ApsSlotData {
								SlotId = "rewarded_video_slot",
								APSAdUnitType = APSAdUnitType.RewardedVideo,
							},
							new ApsSlotData {
								SlotId = "interstitial_slot",
								APSAdUnitType = APSAdUnitType.InterstitialDisplay,
							},
							new ApsSlotData {
								SlotId = "banner_slot",
								APSAdUnitType = APSAdUnitType.Display300X250,
							}
						})
				),
			};

			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var got = new BidRequestDelta();
				var data =
					"[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				if (interceptor.GetType() == typeof(ApsIOS))
				{
					got = ((ApsIOS) interceptor).ModifyRequest(expectedBidResponse, data);
				}
				if (interceptor.GetType() == typeof(ApsAndroid))
				{
					got = ((ApsAndroid) interceptor).ModifyRequest(expectedBidResponse, data);
				}
				got.impressionExtension.Position = "test";
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.impressionExtension);
				Assert.AreEqual(wantBody, gotBody);
			}
		#endif
		}
		[Test]
		public void TestSkaAdNetworkInterceptor() {
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new Tuple<BidRequest, IInterceptor>(
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt {
									Position = "test",
									Skadn = new Skadn {
										SkadnetIds = new[] {
											"id1.skadnetwork",
											"id2.skadnetwork",
											"id3.skadnetwork",
											"id4.skadnetwork",
										},
										Version = "2.0"
									}
								}
							}
						}
					}, new SkAdNetworkIOS(MockData.PlistDataRaw())
				),
			};

			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var got = new BidRequestDelta();
				if (interceptor.GetType() == typeof(SkAdNetworkIOS))
				{
					got = ((SkAdNetworkIOS) interceptor).ModifyRequest(expectedBidResponse, "");
				}
				got.impressionExtension.Position = "test";
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.impressionExtension);
				Assert.AreEqual(wantBody, gotBody);
			}
		}


		[Test]
		public void TestMultipleInterceptor() {
			var interceptors = new IInterceptor[] {
				new SkAdNetworkIOS(MockData.PlistDataRaw()),
				new ApsAndroid("foo_app_id",
					new[] {
						new ApsSlotData {
							SlotId = "rewarded_video_slot",
							APSAdUnitType = APSAdUnitType.RewardedVideo,
						},
						new ApsSlotData {
							SlotId = "interstitial_slot",
							APSAdUnitType = APSAdUnitType.InterstitialDisplay,
						},
						new ApsSlotData {
							SlotId = "banner_slot",
							APSAdUnitType = APSAdUnitType.Display300X250,
						}
					})
			};


			var bidRequest = new BidRequest {
				Imp = new[] {
					new Imp {
						Ext = new ImpExt() {
							Position = "test",
						}
					}
				}
			};
			var bidRequestDeltas = new BidRequestDelta[interceptors.Length];
			var i = 0;
			foreach (var interceptor in interceptors) {
				var data = "";
				if (interceptor is ApsAndroid) {
					data =
						"[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				}
				var got = new BidRequestDelta();
				#if UNITY_IOS
					if (interceptor.GetType() == typeof(ApsIOS))
					{
						got = ((ApsIOS) interceptor).ModifyRequest(bidRequest, data);
					}
				#endif
				if (interceptor.GetType() == typeof(ApsAndroid))
				{
					got = ((ApsAndroid) interceptor).ModifyRequest(bidRequest, data);
				}
				if (interceptor.GetType() == typeof(SkAdNetworkIOS))
				{
					got = ((SkAdNetworkIOS) interceptor).ModifyRequest(bidRequest, "");
				}
				bidRequestDeltas[i] = got;
				i++;
			}
			
			var gotBidRequest = BidRequestDeltaManager.ApplyDeltas(bidRequestDeltas, bidRequest);

			var wantBody = JsonConvert.SerializeObject(new ImpExt() {
				Position = "test",
				Skadn = new Skadn {
					SkadnetIds = new[] {
						"id1.skadnetwork",
						"id2.skadnetwork",
						"id3.skadnetwork",
						"id4.skadnetwork",
					},
					Version = "2.0"
				},
				Aps = new JArray{
					JObject.Parse(ParseData)
				}
			});
			var gotBody = JsonConvert.SerializeObject(gotBidRequest.Imp[0].Ext);
			Assert.AreEqual(wantBody, gotBody);
		}
	}


	internal static class MockData {
		public static string PlistDataRaw() {
			return System.IO.File.ReadAllText(
				"../com.adsbynimbus.nimbus/Runtime/Scripts/Nimbus.Tests/TestData/MockPlistFile.json");
		}
	}
}