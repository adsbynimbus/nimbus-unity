using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
#if UNITY_IOS && NIMBUS_ENABLE_APS
using Nimbus.Internal.Interceptor.ThirdPartyDemand.APS;
#endif
using NUnit.Framework;
using OpenRTB.Request;

namespace Nimbus.Tests {
	public class InterceptorTest {
		const string ApsParseData = "{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}";

		[Test]
		public void TestAdMobInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_ADMOB
			const string admobData = "{\"admob_gde_signals\":\"admob_signal\"}";
				var table = new List<Tuple<BidRequest, IInterceptor>> {
					new (
						new BidRequest {
							Imp = new[] {
								new Imp {
									Ext = new ImpExt() {
										Position = "test",
									}
								}
							},
							User = new User
							{
								Ext = JObject.Parse(admobData),
							}
						}, new AdMobAndroid(
							new[] {
								new ThirdPartyAdUnit {
									AdUnitId = "rewarded_video_slot",
									AdUnitType = AdUnitType.Rewarded,
								},
								new ThirdPartyAdUnit {
									AdUnitId = "interstitial_slot",
									AdUnitType = AdUnitType.Interstitial,
								},
								new ThirdPartyAdUnit {
									AdUnitId = "banner_slot",
									AdUnitType = AdUnitType.Banner
								}
							})
					),

					new (
						new BidRequest {
							Imp = new[] {
								new Imp {
									Ext = new ImpExt() {
										Position = "test"
									}
								}
							},
							User = new User
							{
								Ext = JObject.Parse(admobData),
							}
						}, new AdMobIOS(
							new[] {
								new ThirdPartyAdUnit {
									AdUnitId = "rewarded_video_slot",
									AdUnitType = AdUnitType.Rewarded,
								},
								new ThirdPartyAdUnit {
									AdUnitId = "interstitial_slot",
									AdUnitType =  AdUnitType.Interstitial,
								},
								new ThirdPartyAdUnit {
									AdUnitId = "banner_slot",
									AdUnitType = AdUnitType.Banner,
								}
							})
					),
				};
				
				foreach (var tt in table) {
					var (expectedBidResponse, interceptor) = tt;
					// extensions are only added if the imp data has been initialized already
					var got = new BidRequestDelta();
					var data = "admob_signal";
					if (interceptor.GetType() == typeof(AdMobIOS))
					{
						got = ((AdMobIOS) interceptor).GetBidRequestDelta(data);
					}
					if (interceptor.GetType() == typeof(AdMobAndroid))
					{
						got = ((AdMobAndroid) interceptor).GetBidRequestDelta(data);
					}
					var wantBody = expectedBidResponse.User.Ext["admob_gde_signals"].ToString();
					var gotBody = got.simpleUserExt.Value;
					Assert.AreEqual(wantBody, gotBody);
				}
			#endif
		}
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
										JObject.Parse(ApsParseData),
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
										JObject.Parse(ApsParseData),
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
					got = ((ApsIOS) interceptor).GetBidRequestDelta(expectedBidResponse, data);
				}
				if (interceptor.GetType() == typeof(ApsAndroid))
				{
					got = ((ApsAndroid) interceptor).GetBidRequestDelta(expectedBidResponse, data);
				}
				got.impressionExtension.Position = "test";
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.impressionExtension);
				Assert.AreEqual(wantBody, gotBody);
			}
		#endif
		}
		
		[Test]
		public void TestMetaInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_META
			const string metaUserData = "{\"facebook_buyeruid\":\"meta_buyer_uid\"}";
				var table = new List<Tuple<BidRequest, IInterceptor>> {
					new (
						new BidRequest {
							Imp = new[] {
								new Imp {
									Ext = new ImpExt() {
										Position = "test",
										FacebookAppId = "meta_app_id",
										MetaTestAdType = "meta_test_ad_type",
									}
								}
							},
							User = new User
							{
								Ext = JObject.Parse(metaUserData),
							}
						}, new MetaAndroid(null,true, "meta_app_id")
					),

					new (
						new BidRequest {
							Imp = new[] {
								new Imp {
									Ext = new ImpExt() {
										Position = "test",
										FacebookAppId = "meta_app_id",
										MetaTestAdType = "meta_test_ad_type",
									}
								}
							},
							User = new User
							{
								Ext = JObject.Parse(metaUserData),
							}
						}, new MetaIOS("meta_app_id", true)
					),
				};
				
				foreach (var tt in table) {
					var (expectedBidResponse, interceptor) = tt;
					// extensions are only added if the imp data has been initialized already
					var gotDelta = new BidRequestDelta();
					var data = "meta_buyer_uid";
					if (interceptor.GetType() == typeof(MetaIOS))
					{
						gotDelta = ((MetaIOS) interceptor).GetBidRequestDelta(expectedBidResponse, data);
					}
					if (interceptor.GetType() == typeof(MetaAndroid))
					{
						gotDelta = ((MetaAndroid) interceptor).GetBidRequestDelta(expectedBidResponse, data);
					}
					var want = expectedBidResponse.User.Ext["facebook_buyeruid"].ToString();
					var got = gotDelta.simpleUserExt.Value;
					Assert.AreEqual(want, got);
					want = expectedBidResponse.Imp[0].Ext.FacebookAppId;
					got = gotDelta.impressionExtension.FacebookAppId;
					Assert.AreEqual(want, got);
				}
			#endif
		}
		
		[Test]
		public void TestMintegralInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_MINTEGRAL
			const string mintegralUserData = "{\"mintegral_sdk\": {\"buyeruid\": \"mintegral_buyer_uid\", " +
			                                 "\"sdkv\": \"mintegral_sdk_version\"}}";
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(mintegralUserData),
						}
					}, new MintegralAndroid(null, "mintegral_app_id", "mintegral_app_key", new[] {
						new ThirdPartyAdUnit {
							AdUnitId = "rewarded_video_slot",
							AdUnitType = AdUnitType.Rewarded,
						},
						new ThirdPartyAdUnit {
							AdUnitId = "interstitial_slot",
							AdUnitType =  AdUnitType.Interstitial,
						},
						new ThirdPartyAdUnit {
							AdUnitId = "banner_slot",
							AdUnitType = AdUnitType.Banner,
						}
					})
				),

				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test"
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(mintegralUserData),
						}
					}, new MintegralIOS("mintegral_app_id", "mintegral_app_key", new[] {
						new ThirdPartyAdUnit {
							AdUnitId = "rewarded_video_slot",
							AdUnitType = AdUnitType.Rewarded,
						},
						new ThirdPartyAdUnit {
							AdUnitId = "interstitial_slot",
							AdUnitType =  AdUnitType.Interstitial,
						},
						new ThirdPartyAdUnit {
							AdUnitId = "banner_slot",
							AdUnitType = AdUnitType.Banner,
						}
					})
				),
			};
				
			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var gotDelta = new BidRequestDelta();
				var data = "{\"buyeruid\": \"mintegral_buyer_uid\", " +
				           "\"sdkv\": \"mintegral_sdk_version\"}";
				if (interceptor.GetType() == typeof(MintegralIOS))
				{
					gotDelta = ((MintegralIOS) interceptor).GetBidRequestDelta(expectedBidResponse, data);
				}
				if (interceptor.GetType() == typeof(MintegralAndroid))
				{
					gotDelta = ((MintegralAndroid) interceptor).GetBidRequestDelta(JObject.Parse(data));
				}
				var want = expectedBidResponse.User.Ext["mintegral_sdk"]["buyeruid"];
				var got = gotDelta.complexUserExt.Value["buyeruid"];
				Assert.AreEqual(want, got);
				want = expectedBidResponse.User.Ext["mintegral_sdk"]["sdkv"];
				got = gotDelta.complexUserExt.Value["sdkv"];
				Assert.AreEqual(want, got);
			}
			#endif
		}
		
		[Test]
		public void TestMobileFuseInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_MOBILEFUSE
			const string mobileFuseUserData = "{\"mfx_buyerdata\": {\"v\": \"2\", \"mf_adapter\":\"nimbus\", " +
			                                 "\"sdk_version\": \"mobilefuse_sdk_version\"}}";
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(mobileFuseUserData),
						}
					}, new MobileFuseAndroid()
				),

				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test"
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(mobileFuseUserData),
						}
					}, new MobileFuseIOS()
				),
			};
				
			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var gotDelta = new BidRequestDelta();
				var data = "{\"v\": \"2\", \"mf_adapter\":\"nimbus\", " +
									"\"sdk_version\": \"mobilefuse_sdk_version\"}";
				if (interceptor.GetType() == typeof(MobileFuseIOS))
				{
					gotDelta = ((MobileFuseIOS) interceptor).GetBidRequestDelta(data);
				}
				if (interceptor.GetType() == typeof(MobileFuseAndroid))
				{
					gotDelta = ((MobileFuseAndroid) interceptor).GetBidRequestDelta(data);
				}
				var want = expectedBidResponse.User.Ext["mfx_buyerdata"]["sdk_version"];
				var got = gotDelta.complexUserExt.Value["sdk_version"];
				Assert.AreEqual(want, got);
				want = expectedBidResponse.User.Ext["mfx_buyerdata"]["mf_adapter"];
				got = gotDelta.complexUserExt.Value["mf_adapter"];
				Assert.AreEqual(want, got);
			}
			#endif
		}
		
		[Test]
		public void TestUnityAdsInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_UNITY_ADS
			const string unityUserData = "{\"unity_buyeruid\":\"unity_buyer_uid\"}";
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(unityUserData),
						}
					}, new UnityAdsAndroid(null, true, "12345")
				),

				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(unityUserData),
						}
					}, new UnityAdsIOS("12345")
				),
			};
				
			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var gotDelta = new BidRequestDelta();
				var data = "unity_buyer_uid";
				if (interceptor.GetType() == typeof(UnityAdsIOS))
				{
					gotDelta = ((UnityAdsIOS) interceptor).GetBidRequestDelta(expectedBidResponse, data);
				}
				if (interceptor.GetType() == typeof(UnityAdsAndroid))
				{
					gotDelta = ((UnityAdsAndroid) interceptor).GetBidRequestDelta(expectedBidResponse, data);
				}
				var want = expectedBidResponse.User.Ext["unity_buyeruid"].ToString();
				var got = gotDelta.simpleUserExt.Value;
				Assert.AreEqual(want, got);
			}
		#endif
		}
		
		[Test]
		public void TestVungleInterceptor()
		{
			#if UNITY_IOS && NIMBUS_ENABLE_VUNGLE
			const string vungleUserData = "{\"vungle_buyeruid\":\"vungle_buyer_uid\"}";
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(vungleUserData),
						}
					}, new VungleAndroid(null, "vungle_app_id")
				),

				new (
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ImpExt() {
									Position = "test",
								}
							}
						},
						User = new User
						{
							Ext = JObject.Parse(vungleUserData),
						}
					}, new VungleIOS("vungle_app_id")
				),
			};
				
			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var gotDelta = new BidRequestDelta();
				var data = "vungle_buyer_uid";
				if (interceptor.GetType() == typeof(VungleIOS))
				{
					gotDelta = ((VungleIOS) interceptor).GetBidRequestDelta(data);
				}
				if (interceptor.GetType() == typeof(VungleAndroid))
				{
					gotDelta = ((VungleAndroid) interceptor).GetBidRequestDelta(data);
				}
				var want = expectedBidResponse.User.Ext["vungle_buyeruid"].ToString();
				var got = gotDelta.simpleUserExt.Value;
				Assert.AreEqual(want, got);
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
					got = ((SkAdNetworkIOS) interceptor).GetBidRequestDelta(expectedBidResponse, "");
				}
				got.impressionExtension.Position = "test";
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.impressionExtension);
				Assert.AreEqual(wantBody, gotBody);
			}
		}
		
		[Test]
		public void TestMultipleImpExtInterceptors() {
			#if UNITY_ANDROID && NIMBUS_ENABLE_META && NIMBUS_ENABLE_APS
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
				, new MetaAndroid(null, true, "meta_app_id")
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
			foreach (var interceptor in interceptors)
			{
				var data =
					"[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				var got = new BidRequestDelta();
				#if UNITY_IOS
					if (interceptor.GetType() == typeof(ApsIOS))
					{
						got = ((ApsIOS) interceptor).GetBidRequestDelta(bidRequest, data);
					}
				#endif
				if (interceptor.GetType() == typeof(ApsAndroid))
				{
					got = ((ApsAndroid) interceptor).GetBidRequestDelta(bidRequest, data);
				}
				if (interceptor.GetType() == typeof(MetaAndroid))
				{
					data = "meta_buyer_uid";
					got = ((MetaAndroid) interceptor).GetBidRequestDelta(bidRequest, data);
				}
				if (interceptor.GetType() == typeof(SkAdNetworkIOS))
				{
					got = ((SkAdNetworkIOS) interceptor).GetBidRequestDelta(bidRequest, "");
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
				FacebookAppId = "meta_app_id",
				Aps = new JArray{
					JObject.Parse(ApsParseData)
				},
				MetaTestAdType = "IMG_16_9_LINK"
			});
			var gotBody = JsonConvert.SerializeObject(gotBidRequest.Imp[0].Ext);
			Assert.AreEqual(wantBody, gotBody);
			#endif
		}

		[Test]
		public void TestMultipleUserExtInterceptors()
		{
			#if UNITY_ANDROID && NIMBUS_ENABLE_META && NIMBUS_ENABLE_VUNGLE && NIMBUS_ENABLE_MINTEGRAL && NIMBUS_ENABLE_MOBILEFUSE
			var interceptors = new IInterceptor[] { 
				new MetaAndroid(null, true, "meta_app_id")
				, new VungleAndroid(null, "vungle_app_id")
				, new MintegralAndroid(null, "mintegral_app_id", "mintegral_app_key", new []
						{
							new ThirdPartyAdUnit {
								AdUnitId = "rewarded_video_slot",
								AdUnitType = AdUnitType.Rewarded,
							},
							new ThirdPartyAdUnit {
								AdUnitId = "interstitial_slot",
								AdUnitType =  AdUnitType.Interstitial,
							},
							new ThirdPartyAdUnit {
								AdUnitId = "banner_slot",
								AdUnitType = AdUnitType.Banner,
							}
						})
				, new MobileFuseAndroid()
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
			foreach (var interceptor in interceptors)
			{
				var data = "";
				var got = new BidRequestDelta();
				if (interceptor.GetType() == typeof(MetaAndroid))
				{
					data = "meta_buyer_uid";
					got = ((MetaAndroid) interceptor).GetBidRequestDelta(bidRequest, data);
				}
				if (interceptor.GetType() == typeof(VungleAndroid))
				{
					data = "vungle_buyer_uid";
					got = ((VungleAndroid) interceptor).GetBidRequestDelta(data);
				}
				if (interceptor.GetType() == typeof(MintegralAndroid))
				{
					data = "{\"buyeruid\": \"mintegral_buyer_uid\", " +
					           "\"sdkv\": \"mintegral_sdk_version\"}";
					got = ((MintegralAndroid) interceptor).GetBidRequestDelta(JObject.Parse(data));
				}
				if (interceptor.GetType() == typeof(MobileFuseAndroid))
				{
					data = "{\"v\": \"2\", \"mf_adapter\":\"nimbus\", " +
					       "\"sdk_version\": \"mobilefuse_sdk_version\"}";
					got = ((MobileFuseAndroid) interceptor).GetBidRequestDelta(data);
				}
				bidRequestDeltas[i] = got;
				i++;
			}
			var gotBidRequest = BidRequestDeltaManager.ApplyDeltas(bidRequestDeltas, bidRequest);
			Assert.AreEqual("meta_buyer_uid", gotBidRequest.User.Ext["facebook_buyeruid"].ToString());
			Assert.AreEqual("vungle_buyer_uid", gotBidRequest.User.Ext["vungle_buyeruid"].ToString());
			Assert.AreEqual("mintegral_sdk_version", gotBidRequest.User.Ext["mintegral_sdk"]["sdkv"].ToString());
			Assert.AreEqual("mobilefuse_sdk_version", gotBidRequest.User.Ext["mfx_buyerdata"]["sdk_version"].ToString());
		#endif
}
	}


	internal static class MockData {
		public static string PlistDataRaw() {
			return System.IO.File.ReadAllText(
				"../com.adsbynimbus.nimbus/Runtime/Scripts/Nimbus.Tests/TestData/MockPlistFile.json");
		}
	}
}