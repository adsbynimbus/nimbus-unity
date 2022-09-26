using OpenRTB.Request;

namespace Nimbus.Internal.ThirdPartyDemandProviders {
	public interface IProvider  {
		public void InitializeNativeSDK();
	}

	public interface IInterceptor {
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen);
		public BidRequest ModifyRequest(BidRequest bidRequest, string data);
	}
}