using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using static System.String;

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

		public NimbusClient(CancellationTokenSource ctx, NimbusSDKConfiguration configuration) {
			Client.DefaultRequestHeaders.Add("Nimbus-Api-Key", configuration.apiKey);
			Client.DefaultRequestHeaders.Add("Nimbus-Sdkv", "2.1.0");
			Client.DefaultRequestHeaders.Add("Nimbus-Unity-Sdkv", "1.1.1");
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
				var body = JsonConvert.SerializeObject(bidRequest);
				HttpContent jsonBody = new StringContent(body, Encoding.UTF8, "application/json");
				var serverResponse = await Client.PostAsync(_nimbusEndpoint, jsonBody, _ctx.Token);
				if (_ctx.Token.IsCancellationRequested) {
					Client.CancelPendingRequests();
					return "{\"message\": \"Application Closed\"}";
				}
				var nimbusResponse = await serverResponse.Content.ReadAsStringAsync();
#endif

				return nimbusResponse;
			});
		}
	}
}