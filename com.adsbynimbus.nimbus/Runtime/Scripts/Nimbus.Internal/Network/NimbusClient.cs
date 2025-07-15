using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using static System.String;
using UnityEngine;



namespace Nimbus.Internal.Network {
	public class NimbusClient {
		private const string ProductionPath = "/rta/v1";
		private const string TestingPath = "/rta/test";
		
		private static readonly HttpClient Client = new HttpClient(new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.GZip
		});
		private readonly string _nimbusEndpoint = "https://{0}.adsbynimbus.com{1}";
		private CancellationTokenSource _ctx;

		public readonly string platformSdkv;

		public NimbusClient(CancellationTokenSource ctx, NimbusSDKConfiguration configuration, string platformSdkVersion) {
			platformSdkv = platformSdkVersion;
			Client.DefaultRequestHeaders.Add("Nimbus-Api-Key", configuration.apiKey);
			Client.DefaultRequestHeaders.Add("Nimbus-Sdkv", platformSdkv);
			Client.DefaultRequestHeaders.Add("Nimbus-Unity-Sdkv", VersionConstants.UnitySdkVersion);
			Client.DefaultRequestHeaders.Add("X-Openrtb-Version", "2.5");
			Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
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
				Debug.unityLogger.LogError("Nimbus", "UserAgent could not be retrieved from the platform at the moment, try again");
			}
			
			#pragma warning disable CS1998
				return await Task.Run(async () => {
			#pragma warning restore CS1998
			#if UNITY_EDITOR
				Debug.unityLogger.LogError("Nimbus", "In Editor mode, network request will not be made");
				return "";
			#else
				// This will throw an exception if the bid request is missing required data from Nimbus 
				var body = JsonConvert.SerializeObject(bidRequest, new JsonSerializerSettings() { 
															NullValueHandling = NullValueHandling.Ignore });
				Debug.unityLogger.Log("Nimbus", $"BID REQUEST: {body}");
				HttpContent jsonContent = new StringContent(body, Encoding.UTF8, "application/json");			
				HttpContent requestContent = new GzipContent(jsonContent);
				var serverResponse = await Client.PostAsync(_nimbusEndpoint, requestContent, _ctx.Token);
				if (_ctx.Token.IsCancellationRequested) {
					Client.CancelPendingRequests();
					return "{\"message\": \"Application Closed\"}";
				}
				var nimbusResponse = await serverResponse.Content.ReadAsStringAsync();
				switch ((int)serverResponse.StatusCode)
				{
					case 200:
						return nimbusResponse;
					case 400:
						Debug.unityLogger.Log("Nimbus", "RESPONSE ERROR: Status Code 400: POST data was malformed");
						break;
					case 404:
						Debug.unityLogger.Log("Nimbus", "RESPONSE ERROR: Status Code 404: No bids returned");
						break;
					case 429:
						Debug.unityLogger.Log("Nimbus", "RESPONSE ERROR: Status Code 429: Rate Limited");
						break;
					case 500:
						Debug.unityLogger.Log("Nimbus", "RESPONSE ERROR: Status Code 500: Server is Unavailable");
						break;
					default:
						Debug.unityLogger.Log("Nimbus", "RESPONSE ERROR: Unknown Network Error Occurred");
						break;
				}
				return "";
				#endif
			});
		}
	}
}

internal sealed class GzipContent : HttpContent
{
	private readonly HttpContent content;

	public GzipContent(HttpContent content)
	{
		this.content = content;

		// Keep the original content's headers ...
		foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
		{
			Headers.TryAddWithoutValidation(header.Key, header.Value);
		}

		// ... and let the server know we've Gzip-compressed the body of this request.
		Headers.ContentEncoding.Add("gzip");
	}

	protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
	{
		// Open a GZipStream that writes to the specified output stream.
		using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress, true))
		{
			// Copy all the input content to the GZip stream.
			await content.CopyToAsync(gzip);
		}
	}

	protected override bool TryComputeLength(out long length)
	{
		length = -1;
		return false;
	}
}