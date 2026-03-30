using System.Collections.Generic;
using Nimbus.Internal.Interceptor;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;

namespace Nimbus.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(NimbusSDKConfiguration configuration);
		internal abstract void getAd(NimbusAdUnit nimbusAdUnit, bool showAd);
		internal abstract List<IInterceptor> Interceptors();
	}
}