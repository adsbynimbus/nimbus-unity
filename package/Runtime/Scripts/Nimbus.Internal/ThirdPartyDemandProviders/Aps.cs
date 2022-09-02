using Nimbus.Internal.Utility;
using OpenRTB.Request;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.ThirdPartyDemandProviders {
	internal class Aps : IInterceptor, IProvider {
		private string _appID;
		private ApsSlotData[] _slotData;
		
		// Constructor will have to extend to pass in Android/IOS bridge data to make calls to retrieve data
		// Android
		// IOS
#if UNITY_ANDROID
#elif UNITY_IPHONE
#endif
		
		public Aps(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}

		public void InitializeNativeSDK() {
			
		}


		public string GetProviderRtbDataFromNativeSDK() {
			// this is where the native bridged code gets called in order to retrieve the APS data from the Native
			// sdk
			return "";
		}
		
		public BidRequest ModifyRequest(BidRequest bidRequest) {
			// ReSharper disable InvertIf
			if (!bidRequest.Imp.IsNullOrEmpty()) {
				bidRequest.Imp[0].Ext ??= new ThirdPartyProviderImpExt();
				// pass in the APS data
				if (bidRequest.Imp[0].Ext is ThirdPartyProviderImpExt apsData) {
					// TODO pass raw APS string data from SDK back to Nimbus
					apsData.Aps = "";
					bidRequest.Imp[0].Ext = apsData;
				}
			}
			return bidRequest;
		}
	}
	
}