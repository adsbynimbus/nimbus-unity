using System.Runtime.CompilerServices;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]

namespace Nimbus.Internal.Extensions {
	internal class MolocoAndroid {

		private readonly string _appKey;
		private readonly AndroidJavaObject _applicationContext;
		
		public MolocoAndroid(AndroidJavaObject applicationContext, string appKey) {
			_applicationContext = applicationContext;
			_appKey = appKey;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var molocoMediationInfoObj = new AndroidJavaObject("com.moloco.sdk.publisher.MediationInfo", "none");
				var molocoInitObj = new AndroidJavaObject("com.moloco.sdk.publisher.init.MolocoInitParams", _applicationContext, _appKey, molocoMediationInfoObj);
				var molocoClass = new AndroidJavaClass("com.moloco.sdk.publisher.Moloco");
				molocoClass.CallStatic("initialize", molocoInitObj);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Moloco Initialization ERROR", e.Message);
			}
		}
	}
	
}