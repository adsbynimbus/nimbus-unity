using System;
using System.IO;
using System.Text;
using AdsByNimbus.Internal.Extensions.AdMob;
using AdsByNimbus.Internal.Extensions.APS;
using UnityEngine;

namespace ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[HideInInspector] public string publisherKey;
		[HideInInspector] public string apiKey;
		[HideInInspector] public bool enableSDKInTestMode;
		[HideInInspector] public bool enableUnityLogs;
		[HideInInspector] public bool enableManualInitialization;
		[HideInInspector] public bool sdkInitialized = false;

		// APS data
		[HideInInspector] public string androidApsAppKey;
		[HideInInspector] public ApsSlotData[] androidApsSlotData;
		
		[HideInInspector] public string iosApsAppKey;
		[HideInInspector] public ApsSlotData[] iosApsSlotData;

		// Vungle Data
		[HideInInspector] public string androidVungleAppID;
		[HideInInspector] public string iosVungleAppID;

		// Meta Data
		[HideInInspector] public string androidMetaAppID;
		[HideInInspector] public string iosMetaAppID;
		
		// AdMob Data
		[HideInInspector] public string androidAdMobAppID;
		[HideInInspector] public AdMobAdUnit[] androidAdMobAdUnitData;
		[HideInInspector] public string iosAdMobAppID;
		[HideInInspector] public AdMobAdUnit[] iosAdMobAdUnitData;
		
		// Mintegral Data
		[HideInInspector] public string androidMintegralAppID;
		[HideInInspector] public string androidMintegralAppKey;
		[HideInInspector] public string iosMintegralAppID;
		[HideInInspector] public string iosMintegralAppKey;
		
		//Unity Ads Data
		[HideInInspector] public string androidUnityAdsGameID;
		[HideInInspector] public string iosUnityAdsGameID;
		
		//Moloco Data
		[HideInInspector] public string androidMolocoAppKey;
		[HideInInspector] public string iosMolocoAppKey;
		
		//InMobi Data
		[HideInInspector] public string androidInMobiAccountId;
		[HideInInspector] public string iosInMobiAccountId;
		
		private void OnValidate() {
			Sanitize();
		}

		public void Sanitize() {
			publisherKey = publisherKey?.Trim();
			apiKey = apiKey?.Trim();
			#if NIMBUS_ENABLE_APS 
				androidApsAppKey = androidApsAppKey?.Trim();
				iosApsAppKey = iosApsAppKey?.Trim();

				// ReSharper disable ForCanBeConvertedToForeach
				// ReSharper disable InvertIf
				if (androidApsSlotData != null) {
					for (var i = 0; i < androidApsSlotData.Length; i++) {
						androidApsSlotData[i].slotId = androidApsSlotData[i].slotId?.Trim();
					}
				}
				
				if (iosApsSlotData != null) {
					for (var i = 0; i < iosApsSlotData.Length; i++) {
						iosApsSlotData[i].slotId = iosApsSlotData[i].slotId?.Trim();
					}
				}
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				androidVungleAppID = androidVungleAppID?.Trim();
				iosVungleAppID = iosVungleAppID?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_META
				androidMetaAppID = androidMetaAppID?.Trim();
				iosMetaAppID = iosMetaAppID?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_ADMOB
				androidAdMobAppID = androidAdMobAppID?.Trim();
				iosAdMobAppID = iosAdMobAppID?.Trim();
				var builder = new StringBuilder();
				builder.AppendLine($"android-{androidAdMobAppID}");
				builder.AppendLine($"ios-{iosAdMobAppID}");
				var idPath = "Assets/Editor/AdMobIds";
				// Ensure directory exists before writing to file
				Directory.CreateDirectory(idPath.Substring(0, idPath.LastIndexOf('/')));
				File.WriteAllText(idPath, builder.ToString());
				if (androidAdMobAdUnitData != null) {
					for (var i = 0; i < androidAdMobAdUnitData.Length; i++) {
						androidAdMobAdUnitData[i].AdUnitId = androidAdMobAdUnitData[i].AdUnitId?.Trim();
					}
				}
				
				if (iosAdMobAdUnitData != null) {
					for (var i = 0; i < iosAdMobAdUnitData.Length; i++) {
						iosAdMobAdUnitData[i].AdUnitId = iosAdMobAdUnitData[i].AdUnitId?.Trim();
					}
				}
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				androidMintegralAppID = androidMintegralAppID?.Trim();
				androidMintegralAppKey = androidMintegralAppKey?.Trim();
				iosMintegralAppID = iosMintegralAppID?.Trim();
				iosMintegralAppKey = iosMintegralAppKey?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_UNITY_ADS
				androidUnityAdsGameID = androidUnityAdsGameID?.Trim();
				iosUnityAdsGameID = iosUnityAdsGameID?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO
			androidMolocoAppKey = androidMolocoAppKey?.Trim();
			iosMolocoAppKey = iosMolocoAppKey?.Trim();
			#endif
			
		#if NIMBUS_ENABLE_INMOBI
			androidInMobiAccountId = androidInMobiAccountId?.Trim();
			iosInMobiAccountId = iosInMobiAccountId?.Trim();
			#endif
		}
		


		public Tuple<string, ApsSlotData[]> GetApsData() {
			var appKey = androidApsAppKey;
			var slots = androidApsSlotData;
			#if UNITY_IOS
				appKey = iosApsAppKey;
				slots =  iosApsSlotData;
			#endif
			return new Tuple<string, ApsSlotData[]>(appKey, slots);
		}

		public string GetVungleData()
		{
			var appID = androidVungleAppID;
			#if UNITY_IOS
				appID = iosVungleAppID;
			#endif
			return appID;
		}
		
		public string GetMetaData()
		{
			var appID = androidMetaAppID;
		#if UNITY_IOS
			appID = iosMetaAppID;
		#endif
			return appID;
		}
		
		public AdMobAdUnit[] GetAdMobData() {
			var adUnitIds = androidAdMobAdUnitData;
			#if UNITY_IOS
				adUnitIds =  iosAdMobAdUnitData;
			#endif
			return adUnitIds;
		}
		
		public Tuple<string, string> GetMintegralData() {
			var appID = androidMintegralAppID;
			var appKey = androidMintegralAppKey;
			#if UNITY_IOS
				appID = iosMintegralAppID;
				appKey = iosMintegralAppKey;
			#endif
			return new Tuple<string, string>(appID, appKey);
		}
		
		public string GetUnityAdsData()
		{
			var appID = androidUnityAdsGameID;
			#if UNITY_IOS
				appID = iosUnityAdsGameID;
			#endif
			return appID;
		}
		
		public string GetMolocoData() {
			var appKey = androidMolocoAppKey;
			#if UNITY_IOS
				appKey = iosMolocoAppKey;
			#endif
			return appKey;
		}
		
		public string GetInMobiData() {
			var appKey = androidInMobiAccountId;
			#if UNITY_IOS
				appKey = iosInMobiAccountId;
			#endif
			return appKey;
		}

	}
}