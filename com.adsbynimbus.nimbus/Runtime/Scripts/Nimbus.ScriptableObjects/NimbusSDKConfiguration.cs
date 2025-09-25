using System;
using System.IO;
using System.Text;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using UnityEngine;

namespace Nimbus.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[HideInInspector] public string publisherKey;
		[HideInInspector] public string apiKey;
		[HideInInspector] public bool enableSDKInTestMode;
		[HideInInspector] public bool enableUnityLogs;

		// APS data
		[HideInInspector] public string androidAppID;
		[HideInInspector] public ApsSlotData[] androidApsSlotData;
		[HideInInspector] public int androidApsTimeoutInMilliseconds = 1000;
		
		public const int ApsDefaultTimeout = 1000;
		
		[HideInInspector] public string iosAppID;
		[HideInInspector] public ApsSlotData[] iosApsSlotData;
		[HideInInspector] public int iosApsTimeoutInMilliseconds = 1000;

		// Vungle Data
		[HideInInspector] public string androidVungleAppID;
		[HideInInspector] public string iosVungleAppID;

		// Meta Data
		[HideInInspector] public string androidMetaAppID;
		[HideInInspector] public string iosMetaAppID;
		
		// AdMob Data
		[HideInInspector] public string androidAdMobAppID;
		[HideInInspector] public ThirdPartyAdUnit[] androidAdMobAdUnitData;
		[HideInInspector] public string iosAdMobAppID;
		[HideInInspector] public ThirdPartyAdUnit[] iosAdMobAdUnitData;
		
		// Mintegral Data
		[HideInInspector] public string androidMintegralAppID;
		[HideInInspector] public string androidMintegralAppKey;
		[HideInInspector] public ThirdPartyAdUnit[] androidMintegralAdUnitData;
		[HideInInspector] public string iosMintegralAppID;
		[HideInInspector] public string iosMintegralAppKey;
		[HideInInspector] public ThirdPartyAdUnit[] iosMintegralAdUnitData;
		
		//Unity Ads Data
		[HideInInspector] public string androidUnityAdsGameID;
		[HideInInspector] public string iosUnityAdsGameID;
		
		//Moloco Data
		[HideInInspector] public string androidMolocoAppKey;
		[HideInInspector] public ThirdPartyAdUnit[] androidMolocoAdUnitData;
		[HideInInspector] public string iosMolocoAppKey;
		[HideInInspector] public ThirdPartyAdUnit[] iosMolocoAdUnitData;
		
		//InMobi Data
		[HideInInspector] public string androidInMobiAccountId;
		[HideInInspector] public ThirdPartyAdUnit[] androidInMobiAdUnitData;
		[HideInInspector] public string iosInMobiAccountId;
		[HideInInspector] public ThirdPartyAdUnit[] iosInMobiAdUnitData;
		
		private void OnValidate() {
			Sanitize();
		}

		public void Sanitize() {
			publisherKey = publisherKey?.Trim();
			apiKey = apiKey?.Trim();
			#if NIMBUS_ENABLE_APS 
				androidAppID = androidAppID?.Trim();
				iosAppID = iosAppID?.Trim();

				// ReSharper disable ForCanBeConvertedToForeach
				// ReSharper disable InvertIf
				if (androidApsSlotData != null) {
					for (var i = 0; i < androidApsSlotData.Length; i++) {
						androidApsSlotData[i].SlotId = androidApsSlotData[i].SlotId?.Trim();
					}
				}
				
				if (iosApsSlotData != null) {
					for (var i = 0; i < iosApsSlotData.Length; i++) {
						iosApsSlotData[i].SlotId = iosApsSlotData[i].SlotId?.Trim();
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
				if (androidMintegralAdUnitData != null) {
					for (var i = 0; i < androidMintegralAdUnitData.Length; i++) {
						androidMintegralAdUnitData[i].AdUnitId = androidMintegralAdUnitData[i].AdUnitId?.Trim();
					}
				}
					
				if (iosMintegralAdUnitData != null) {
					for (var i = 0; i < iosMintegralAdUnitData.Length; i++) {
						iosMintegralAdUnitData[i].AdUnitId = iosMintegralAdUnitData[i].AdUnitId?.Trim();
					}
				}
			#endif
			
			#if NIMBUS_ENABLE_UNITY_ADS
				androidUnityAdsGameID = androidUnityAdsGameID?.Trim();
				iosUnityAdsGameID = iosUnityAdsGameID?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO
			androidMolocoAppKey = androidMolocoAppKey?.Trim();
			iosMolocoAppKey = iosMolocoAppKey?.Trim();
			if (androidMolocoAdUnitData != null) {
				for (var i = 0; i < androidMolocoAdUnitData.Length; i++) {
					androidMolocoAdUnitData[i].AdUnitId = androidMolocoAdUnitData[i].AdUnitId?.Trim();
				}
			}
					
			if (iosMolocoAdUnitData != null) {
				for (var i = 0; i < iosMolocoAdUnitData.Length; i++) {
					iosMolocoAdUnitData[i].AdUnitId = iosMolocoAdUnitData[i].AdUnitId?.Trim();
				}
			}
			#endif
			
		#if NIMBUS_ENABLE_INMOBI
			androidInMobiAccountId = androidInMobiAccountId?.Trim();
			iosInMobiAccountId = iosInMobiAccountId?.Trim();
			if (androidInMobiAdUnitData != null) {
				for (var i = 0; i < androidInMobiAdUnitData.Length; i++) {
					androidInMobiAdUnitData[i].AdUnitId = androidInMobiAdUnitData[i].AdUnitId?.Trim();
				}
			}
					
			if (iosInMobiAdUnitData != null) {
				for (var i = 0; i < iosInMobiAdUnitData.Length; i++) {
					iosInMobiAdUnitData[i].AdUnitId = iosInMobiAdUnitData[i].AdUnitId?.Trim();
				}
			}
			#endif
		}
		


		public Tuple<string, ApsSlotData[], int> GetApsData() {
			var appID = androidAppID;
			var slots = androidApsSlotData;
			var timeoutInMilliseconds = androidApsTimeoutInMilliseconds;
			#if UNITY_IOS
				appID = iosAppID;
				slots =  iosApsSlotData;
				timeoutInMilliseconds = iosApsTimeoutInMilliseconds;
			#endif
			return new Tuple<string, ApsSlotData[], int>(appID, slots, timeoutInMilliseconds);
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
		
		public Tuple<string, ThirdPartyAdUnit[]> GetAdMobData() {
			var appID = androidAdMobAppID;
			var adUnitIds = androidAdMobAdUnitData;
			#if UNITY_IOS
				appID = iosAdMobAppID;
				adUnitIds =  iosAdMobAdUnitData;
			#endif
			return new Tuple<string, ThirdPartyAdUnit[]>(appID, adUnitIds);
		}
		
		public Tuple<string, string, ThirdPartyAdUnit[]> GetMintegralData() {
			var appID = androidMintegralAppID;
			var appKey = androidMintegralAppKey;
			var adUnitIds = androidMintegralAdUnitData;
			#if UNITY_IOS
				appID = iosMintegralAppID;
				appKey = iosMintegralAppKey;
				adUnitIds =  iosMintegralAdUnitData;
			#endif
			return new Tuple<string, string, ThirdPartyAdUnit[]>(appID, appKey, adUnitIds);
		}
		
		public string GetUnityAdsData()
		{
			var appID = androidUnityAdsGameID;
			#if UNITY_IOS
				appID = iosUnityAdsGameID;
			#endif
			return appID;
		}
		
		public Tuple<string, ThirdPartyAdUnit[]> GetMolocoData() {
			var appKey = androidMolocoAppKey;
			var adUnitIds = androidMolocoAdUnitData;
			#if UNITY_IOS
				appKey = iosMolocoAppKey;
				adUnitIds =  iosMolocoAdUnitData;
			#endif
			return new Tuple<string, ThirdPartyAdUnit[]>(appKey, adUnitIds);
		}
		
		public Tuple<string, ThirdPartyAdUnit[]> GetInMobiData() {
			var appKey = androidInMobiAccountId;
			var adUnitIds = androidInMobiAdUnitData;
			#if UNITY_IOS
				appKey = iosInMobiAccountId;
				adUnitIds =  iosInMobiAdUnitData;
			#endif
			return new Tuple<string, ThirdPartyAdUnit[]>(appKey, adUnitIds);
		}

	}
}