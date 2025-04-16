using OpenRTB.Request;

namespace Nimbus.Internal.Interceptor {
	public interface IInterceptor {
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen, int width=0, int height=0);
		public BidRequest ModifyRequest(BidRequest bidRequest, string data);
	}
}