using OpenRTB.Request;

namespace Nimbus.Internal.Interceptor {
	public interface IInterceptor {
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen);
		public BidRequest ModifyRequest(BidRequest bidRequest, string data);
	}
}