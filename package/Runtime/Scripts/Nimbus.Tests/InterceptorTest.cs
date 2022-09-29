using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nimbus.Internal;
using Nimbus.Internal.ThirdPartyDemandProviders;
using NUnit.Framework;
using OpenRTB.Request;

namespace Nimbus.Tests {
	public class InterceptorTest {
		[Test]
		public void TestInterceptors() {
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
				var data = "[{\"amzn_h\":\"aax-us-east.amazon-adsystem.com\",\"amznslots\":\"foobar\",\"amznrdr\":\"default\",\"amznp\":\"cnabk0\",\"amzn_b\":\"foobar-bid\",\"dc\":\"iad\"}]";
				got = interceptor.ModifyRequest(got, data);
				var wantBody = JsonConvert.SerializeObject(expectedBidResponse.Imp[0].Ext);
				var gotBody = JsonConvert.SerializeObject(got.Imp[0].Ext);
				Assert.AreEqual(wantBody, gotBody);
			}
		}
	}
}