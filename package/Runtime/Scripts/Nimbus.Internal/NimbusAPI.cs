using System.Collections.Generic;
using Nimbus.Internal.ThirdPartyDemandProviders;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;

namespace Nimbus.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(NimbusSDKConfiguration configuration);
		internal abstract void ShowAd(NimbusAdUnit nimbusAdUnit);
		internal abstract string GetSessionID();
		internal abstract Device GetDevice();
		internal abstract void SetCoppaFlag(bool flag);
		internal abstract List<IInterceptor> Interceptors();
	}
}