using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	internal class MetaAndroid : IProvider {
		private const string NimbusMetaPackage = "com.adsbynimbus.request.FANDemandProvider";
		private readonly string _appID;
		private readonly AndroidJavaObject _applicationContext;

		public MetaAndroid(string appID) {
			_appID = appID;
		}
		
		public MetaAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		
		public void InitializeNativeSDK() {
			var meta = new AndroidJavaClass(NimbusMetaPackage);
			meta.CallStatic("initialize", _appID);
		}
	}
}