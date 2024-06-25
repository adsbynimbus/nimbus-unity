using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nimbus.Internal;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using NUnit.Framework;
using OpenRTB.Request;

namespace Nimbus.Tests {
	public class InterceptorTest {
		[Test]
		public void TestApsInterceptor() {
			var table = new List<Tuple<BidRequest, IInterceptor>> {
				new Tuple<BidRequest, IInterceptor>(
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ThirdPartyProviderImpExt {
									Position = "test",
									Aps = new ApsResponse[] {
										new ApsResponse {
											AmznB = "foobar-bid",
											AmznH = "aax-us-east.amazon-adsystem.com",
											Amznp = "cnabk0",
											Amznrdr = "default",
											Amznslots = "foobar",
											Dc = "iad"
										},
									}
								}
							}
						}
					}, new ApsAndroid("foo_app_id",
						new[] {
							new ApsSlotData {
								SlotId = "rewarded_video_slot",
								AdUnitType = AdUnitType.Rewarded,
							},
							new ApsSlotData {
								SlotId = "interstitial_slot",
								AdUnitType = AdUnitType.Interstitial,
							},
							new ApsSlotData {
								SlotId = "banner_slot",
								AdUnitType = AdUnitType.Banner,
							}
						})
				),

				new Tuple<BidRequest, IInterceptor>(
					new BidRequest {
						Imp = new[] {
							new Imp {
								Ext = new ThirdPartyProviderImpExt {
									Position = "test",
									Aps = new ApsResponse[] {
										new ApsResponse {
											AmznB = "foobar-bid",
											AmznH = "aax-us-east.amazon-adsystem.com",
											Amznp = "cnabk0",
											Amznrdr = "default",
											Amznslots = "foobar",
											Dc = "iad"
										},
									}
								}
							}
						}
					}, new ApsIOS("foo_app_id",
						new[] {
							new ApsSlotData {
								SlotId = "rewarded_video_slot",
								AdUnitType = AdUnitType.Rewarded,
							},
							new ApsSlotData {
								SlotId = "interstitial_slot",
								AdUnitType = AdUnitType.Interstitial,
							},
							new ApsSlotData {
								SlotId = "banner_slot",
								AdUnitType = AdUnitType.Banner,
							}
						})
				),
			};

			foreach (var tt in table) {
				var (expectedBidResponse, interceptor) = tt;
				// extensions are only added if the imp data has been initialized already
				var got = new BidRequest {
					Imp = new[] {
						new Imp {
							Ext = new ThirdPartyProviderImpExt {
								Position = "test",
							}
						}
					}
				};
				var data =
					"[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				got = interceptor.ModifyRequest(got, data);
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.Imp[0].Ext);
				Assert.AreEqual(wantBody, gotBody);
			}
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
				var got = new BidRequest {
					Imp = new[] {
						new Imp {
							Ext = new ThirdPartyProviderImpExt {
								Position = "test",
							}
						}
					}
				};
				got = interceptor.ModifyRequest(got, "");
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.Imp[0].Ext);
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
							AdUnitType = AdUnitType.Rewarded,
						},
						new ApsSlotData {
							SlotId = "interstitial_slot",
							AdUnitType = AdUnitType.Interstitial,
						},
						new ApsSlotData {
							SlotId = "banner_slot",
							AdUnitType = AdUnitType.Banner,
						}
					})
			};


			var got = new BidRequest {
				Imp = new[] {
					new Imp {
						Ext = new ThirdPartyProviderImpExt {
							Position = "test",
						}
					}
				}
			};

			foreach (var interceptor in interceptors) {
				var data = "";
				if (interceptor is ApsAndroid) {
					data =
						"[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				}

				got = interceptor.ModifyRequest(got, data);
			}

			var wantBody = JsonConvert.SerializeObject(new ThirdPartyProviderImpExt {
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
				Aps = new[] {
					new ApsResponse {
						AmznB = "foobar-bid",
						AmznH = "aax-us-east.amazon-adsystem.com",
						Amznp = "cnabk0",
						Amznrdr = "default",
						Amznslots = "foobar",
						Dc = "iad"
					},
				}
			});
			var gotBody = JsonConvert.SerializeObject(got.Imp[0].Ext);
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