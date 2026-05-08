using System.Runtime.CompilerServices;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Extensions {
	internal class MintegralAndroid {
		private const string NimbusMintegralPackage = "com.adsbynimbus.request.MintegralDemandProvider";

		private readonly string _appID;
		private readonly string _appKey;
		private readonly AndroidJavaObject _applicationContext;
		
		public MintegralAndroid(AndroidJavaObject applicationContext, string appID, string appKey) {
			_applicationContext = applicationContext;
			_appID = appID;
			_appKey = appKey;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var mintegralDemandClass = new AndroidJavaClass(NimbusMintegralPackage);
				mintegralDemandClass.CallStatic("initialize", _appID, _appKey);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Mintegral Initialization ERROR", e.Message);
			}

		}
	}
}