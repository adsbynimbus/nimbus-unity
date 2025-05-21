using System.Threading.Tasks;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using OpenRTB.Request;

namespace Nimbus.Internal.Interceptor {
	public interface IInterceptor {
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest);
	}
}