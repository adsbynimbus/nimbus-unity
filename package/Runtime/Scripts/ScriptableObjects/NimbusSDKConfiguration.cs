using System;
using System.Linq;
using UnityEngine;

namespace Nimbus.Runtime.Scripts.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[Header("Publisher Credentials")] [Tooltip("Typically your applications name")]
		public string publisherKey;

		[Tooltip("A UUID used to authenticate your requests to Nimbus")]
		public string apiKey;

		[Header("Application Data")]
		[Tooltip("Must be the name of the application has defined by the app store publication")]
		public string appName;

		[Tooltip("Website point to information about your app")]
		public string appDomain;

		[Header("Android Data")] public string androidAppStoreURL;

		[Tooltip(
			"The android bundle can be found at the end of the appstore URL, eg if the URL is https://play.google.com/store/apps/details?id=com.timehop, the bundle id is com.timehop")]
		public string androidBundleID;

		[Header("Ios Data")] public string iosAppStoreURL;

		[Tooltip(
			"The ios bundle can be found at the end of the appstore URL, eg if the URL is https://apps.apple.com/us/app/timehop-memories-then-now/id569077959, the bundle id is 569077959")]
		public string iosBundleID;

		[Header("SDK Flags")] public bool enableSDKInTestMode;

		[Header("Enable Unity Logs")] public bool enableUnityLogs;

		private void OnValidate() {
			if (publisherKey.Trim().Length == 0) throw new Exception("Publisher key cannot be empty");

			if (apiKey.Trim().Length == 0) throw new Exception("Apikey cannot be empty");

			if (appName.Trim().Length == 0) throw new Exception("Application name cannot be empty");

			if (appDomain.Trim().Length == 0) throw new Exception("Application Domain cannot be empty");
		}

		public void ValidateMobileData() {
#if UNITY_ANDROID
			if (androidAppStoreURL.Trim().Length == 0) throw new Exception("Android store link cannot be empty");

			if (androidBundleID.Trim().Length == 0) throw new Exception("Android bundle cannot be empty");
#elif UNITY_IOS
			if (iosAppStoreURL.Trim().Length == 0) {
				throw new Exception("Ios store link cannot be empty");
			}
			
			if (iosBundleID.Trim().Length == 0) {
				throw new Exception("Ios bundle cannot be empty");
			}
			
			if (!IsDigitsOnly(iosBundleID.Trim())) {
				throw new Exception("The ios bundle should only consists of numeric values");
			}
#endif
		}

		private bool IsDigitsOnly(string str) {
			return str.All(c => c >= '0' && c <= '9');
		}
	}
}