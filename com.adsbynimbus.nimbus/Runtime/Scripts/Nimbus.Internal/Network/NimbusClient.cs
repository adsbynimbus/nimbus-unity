using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using static System.String;
using UnityEngine;

#if !UNITY_EDITOR
using System.Text;
#endif


namespace Nimbus.Internal.Network {
	public class NimbusClient {
		private const string ProductionPath = "/rta/v1";
		private const string TestingPath = "/rta/test";
		
		private static readonly HttpClient Client = new HttpClient();
		private readonly string _nimbusEndpoint = "https://{0}.adsbynimbus.com{1}";
		private CancellationTokenSource _ctx;

		public readonly string platformSdkv;

		public NimbusClient(CancellationTokenSource ctx, NimbusSDKConfiguration configuration, string platformSdkVersion) {
			platformSdkv = platformSdkVersion;
			Client.DefaultRequestHeaders.Add("Nimbus-Api-Key", configuration.apiKey);
			Client.DefaultRequestHeaders.Add("Nimbus-Sdkv", platformSdkv);
			Client.DefaultRequestHeaders.Add("Nimbus-Unity-Sdkv", "1.3.1");
			Client.DefaultRequestHeaders.Add("X-Openrtb-Version", "2.5");
			Client.Timeout = TimeSpan.FromSeconds(10);

			var path = ProductionPath;
			if (configuration.enableSDKInTestMode) path = TestingPath;
			_nimbusEndpoint = Format(_nimbusEndpoint, configuration.publisherKey, path);
			_ctx = ctx;
		}

		public async Task<string> MakeRequestAsync(BidRequest bidRequest) {
			// getting the UserAgent on a mobile device can fail on the first attempt to retrieve
			// it's not an issue try again on the next ad call.
			if (bidRequest.Device.Ua.IsNullOrEmpty()) {
				return "{\"message\": \"UserAgent could not be retrieved from the platform at the moment, try again.\"}";
			}
			
#pragma warning disable CS1998
			return await Task.Run(async () => {
#pragma warning restore CS1998
#if UNITY_EDITOR
				const string nimbusResponse = "{\"message\": \"in Editor mode, network request will not be made\"}";
#else
				// This will throw an exception if the bid request is missing required data from Nimbus 
				//var signal = bidRequest.User.Ext.AdMobSignals;
				var body = JsonConvert.SerializeObject(bidRequest, new JsonSerializerSettings() { 
															NullValueHandling = NullValueHandling.Ignore });
				//var testJson = "{\"user\":{\"age\":20,\"gender\":\"male\",\"ext\":{\"admob_gde_signals\":\"" + signal + "\"      }   },   \"format\":{      \"w\":402,      \"h\":874   },   \"app\":{      \"ver\":\"1.0.0\"   },   \"ext\":{      \"session_id\":\"F7CDEC67-D0F9-4017-BFA6-3C9BCE8FB41B\"   },   \"source\":{      \"ext\":{         \"omidpv\":\"2.18.0\",         \"omidpn\":\"Adsbynimbus\"      }   },   \"device\":{      \"carrier\":\"Unknown\",      \"model\":\"arm64\",      \"connectiontype\":2,      \"ua\":\"Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148\",      \"pxratio\":3,      \"os\":\"ios\",      \"osv\":\"18.0\",      \"lmt\":0,      \"h\":874,      \"language\":\"en\",      \"ext\":{         \"atts\":3      },      \"w\":402,      \"devicetype\":1,      \"ifa\":\"00000000-0000-0000-0000-000000000000\",      \"make\":\"apple\"   },   \"imp\":[      {         \"ext\":{            \"skadn\":{               \"sourceapp\":\"\",               \"productpage\":1,               \"versions\":[                  \"4.0\"               ],               \"skadnetids\":[                  \"t38b2kh725.skadnetwork\",                  \"n6fk4nfna4.skadnetwork\",                  \"3sh42y64q3.skadnetwork\",                  \"v9wttpbfk9.skadnetwork\",                  \"8s468mfl3y.skadnetwork\",                  \"578prtvx9j.skadnetwork\",                  \"n38lu8286q.skadnetwork\",                  \"2u9pt9hc89.skadnetwork\",                  \"4468km3ulz.skadnetwork\",                  \"klf5c3l5u5.skadnetwork\",                  \"424m5254lk.skadnetwork\",                  \"2fnua5tdw4.skadnetwork\",                  \"mlmmfzh3r3.skadnetwork\",                  \"c6k4g5qg8m.skadnetwork\",                  \"4fzdc2evr5.skadnetwork\",                  \"v72qych5uu.skadnetwork\",                  \"uw77j35x4d.skadnetwork\",                  \"ludvb6z3bs.skadnetwork\",                  \"5a6flpkh64.skadnetwork\",                  \"p78axxw29g.skadnetwork\",                  \"4pfyvq9l8r.skadnetwork\",                  \"cp8zw746q7.skadnetwork\",                  \"ydx93a7ass.skadnetwork\",                  \"f38h382jlk.skadnetwork\",                  \"prcb7njmu6.skadnetwork\",                  \"7ug5zh24hu.skadnetwork\",                  \"v4nxqhlyqp.skadnetwork\",                  \"9rd848q2bz.skadnetwork\",                  \"3qcr597p9d.skadnetwork\",                  \"av6w8kgt66.skadnetwork\",                  \"kbd757ywx3.skadnetwork\",                  \"ecpz2srf59.skadnetwork\",                  \"3qy4746246.skadnetwork\",                  \"4dzt52r2t5.skadnetwork\",                  \"cstr6suwn9.skadnetwork\",                  \"a2p9lx4jpn.skadnetwork\",                  \"gta9lk7p23.skadnetwork\",                  \"22mmun2rn5.skadnetwork\",                  \"zq492l623r.skadnetwork\",                  \"hs6bdukanm.skadnetwork\",                  \"ppxm28t8ap.skadnetwork\",                  \"e5fvkxwrpn.skadnetwork\",                  \"8c4e2ghe7u.skadnetwork\",                  \"wzmmz9fp6w.skadnetwork\",                  \"s39g8k73mm.skadnetwork\",                  \"9t245vhmpl.skadnetwork\",                  \"3rd42ekr43.skadnetwork\",                  \"y5ghdn5j9k.skadnetwork\",                  \"yclnxrl5pm.skadnetwork\",                  \"47vhws6wlr.skadnetwork\"               ]            },            \"position\":\"banner\"         },         \"banner\":{            \"h\":50,            \"pos\":0,            \"api\":[               6,               5,               7,               3            ],            \"w\":320         }      }   ],   \"test\":1}";
				Debug.unityLogger.Log("Nimbus", $"BID REQUEST: {body}");
				HttpContent jsonBody = new StringContent(body, Encoding.UTF8, "application/json");
				var serverResponse = await Client.PostAsync(_nimbusEndpoint, jsonBody, _ctx.Token);
				if (_ctx.Token.IsCancellationRequested) {
					Client.CancelPendingRequests();
					return "{\"message\": \"Application Closed\"}";
				}
				var nimbusResponse = await serverResponse.Content.ReadAsStringAsync();
				if (nimbusResponse.IsNullOrEmpty())
				{
					switch ((int)serverResponse.StatusCode)
					{
						case 400:
							nimbusResponse = "{\"status_code\": 400, \"message\": \"POST data was malformed\"}";
							break;
						case 404:
							nimbusResponse = "{\"status_code\": 404,\"message\": \"No bids returned\"}";
							break;
						case 429:
							nimbusResponse = "{\"status_code\": 429,\"message\": \"Rate limited\"}";
							break;
						case 500:
							nimbusResponse = "{\"status_code\": 500,\"message\": \"Server is unavailable\"}";
							break;
						default:
							nimbusResponse = $"{{\"status_code\": {(int)serverResponse.StatusCode},\"message\": \"Unknown network error occurred\"}}";
							break;
					}
				}
				#endif
				return nimbusResponse;
			});
		}
	}
}