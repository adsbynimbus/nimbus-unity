using UnityEditor;
using UnityEngine;

namespace Nimbus.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[Header("Publisher Credentials")]
		[Tooltip("Typically your applications name")]
		public string publisherKey;
		[Tooltip("A UUID used to authenticate your requests to Nimbus")]
		public string apiKey;
		[Header("Application Data")]
		[Tooltip("Must be the name of the application has defined by the app store publication")]
		public string appName;
		[Header("Android Data")]
		public string androidAppStoreURL;
		[Tooltip("The android bundle can be found at the end of the appstore URL, eg if the URL is https://play.google.com/store/apps/details?id=com.timehop, the bundle id is com.timehop")]
		public string androidBundleID;
		[Header("Ios Data")]
		public string iosAppStoreURL;
		[Tooltip("The ios bundle can be found at the end of the appstore URL, eg if the URL is https://apps.apple.com/us/app/timehop-memories-then-now/id569077959, the bundle id is 569077959")]
		public string iosBundleID;
		[Header("SDK Flags")]
		public bool enableSDKInTestMode;
		[Header("Enable Unity Logs")]
		public bool enableUnityLogs;
	}
}