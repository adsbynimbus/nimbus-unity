using System;
using System.IO;
using System.Text;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
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
		
		[HideInInspector] public string iosAppID;
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
		}
		


		public Tuple<string, ApsSlotData[]> GetApsData() {
			var appID = androidAppID;
			var slots = androidApsSlotData;
			#if UNITY_IOS
				appID = iosAppID;
				slots =  iosApsSlotData;
			#endif
			return new Tuple<string, ApsSlotData[]>(appID, slots);
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
		
		public Tuple<string, AdMobAdUnit[]> GetAdMobData() {
			var appID = androidAdMobAppID;
			var adUnitIds = androidAdMobAdUnitData;
			#if UNITY_IOS
				appID = iosAdMobAppID;
				adUnitIds =  iosAdMobAdUnitData;
			#endif
			return new Tuple<string, AdMobAdUnit[]>(appID, adUnitIds);
		}
	}
}