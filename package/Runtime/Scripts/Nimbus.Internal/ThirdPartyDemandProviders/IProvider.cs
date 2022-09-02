using OpenRTB.Request;

namespace Nimbus.Internal.ThirdPartyDemandProviders {
	public interface IProvider {
		public void InitializeNativeSDK();
		public string GetProviderRtbDataFromNativeSDK();
	}

	public interface IInterceptor {
		public BidRequest ModifyRequest(BidRequest bidRequest);
	}
}