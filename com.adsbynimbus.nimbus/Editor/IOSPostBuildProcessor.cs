#if UNITY_EDITOR && UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AdsByNimbus.Internal;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AdsByNimbus.Editor {

	public class IOSPostBuildProcessor {
		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string path) {
			ChangeUnityFrameworkHeader(path);

			var pbx = new PBXProject();
			var pbxPath = PBXProject.GetPBXProjectPath(path);
			pbx.ReadFromFile(pbxPath);
			string unityTargetGuid = pbx.GetUnityFrameworkTargetGuid();
			const string nimbusSpm = "https://github.com/adsbynimbus/nimbus-ios-";

			var flags = new List<string>();
			
			#if NIMBUS_ENABLE_LIVERAMP
				string lrPackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}liveramp.git", 
					iOSExtensionVersionConstants.LiveRamp);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusLiveRampKit", lrPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_LIVERAMP");    
			#endif
			
			#if NIMBUS_ENABLE_APS
				string apsPackageGuid = pbx.AddRemotePackageReferenceAtVersion("{nimbusSpm}aps.git", 
					iOSExtensionVersionConstants.Aps);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusAPSKit", apsPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_APS");    
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				string vunglePackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}vungle.git", 
					iOSExtensionVersionConstants.Vungle);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusVungleKit", vunglePackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_VUNGLE");
			#endif
			
			#if NIMBUS_ENABLE_META
				string metaPackageGuid = pbx.AddRemotePackageReferenceAtVersion("{nimbusSpm}meta.git", 
					iOSExtensionVersionConstants.Meta);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusMetaKit", metaPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_META");
			#endif
			
			#if NIMBUS_ENABLE_ADMOB
				string adMobPackageGuid = pbx.AddRemotePackageReferenceAtVersion("{nimbusSpm}admob.git", 
					iOSExtensionVersionConstants.AdMob);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusAdMobKit", adMobPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_ADMOB");
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL
				string mintegralPackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}mintegral.git", 
					iOSExtensionVersionConstants.Mintegral);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusMintegralKit", mintegralPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_MINTEGRAL");
			#endif

			#if NIMBUS_ENABLE_UNITY_ADS
				string unityPackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}unity.git", 
					iOSExtensionVersionConstants.UnityAds);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusUnityKit", unityPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_UNITY_ADS");
			#endif
			
			#if NIMBUS_ENABLE_MOBILEFUSE
				string mobileFusePackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}mobilefuse.git", 
					iOSExtensionVersionConstants.MobileFuse);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusMobileFuseKit", mobileFusePackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_MOBILEFUSE");
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO
				string molocoPackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}moloco.git", 
					iOSExtensionVersionConstants.Moloco);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusMolocoKit", molocoPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_MOLOCO");
			#endif
			
			#if NIMBUS_ENABLE_INMOBI
				string inMobiPackageGuid = pbx.AddRemotePackageReferenceAtVersion($"{nimbusSpm}inmobi.git", 
					iOSExtensionVersionConstants.InMobi);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusInMobiKit", inMobiPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_INMOBI");
			#endif
			
			#if NIMBUS_ENABLE_DIGITAL_TURBINE
				string dtPackageGuid = pbx.AddRemotePackageReferenceAtVersion("{nimbusSpm}dt.git", 
					iOSExtensionVersionConstants.DigitalTurbine);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusDTKit", dtPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_DIGITAL_TURBINE");
			#endif

			#if NIMBUS_ENABLE_DISPLAY_IO
				string displayIOPackageGuid = pbx.AddRemotePackageReferenceAtVersion("{nimbusSpm}displayio.git", 
					iOSExtensionVersionConstants.DisplayIO);
				pbx.AddRemotePackageFrameworkToProject(unityTargetGuid, "NimbusDisplayIOKit", displayIOPackageGuid, false); 
				flags.Add("NIMBUS_ENABLE_DISPLAY_IO");
			#endif
			
			if (flags.Count > 0)
			{
				var projectGuid = pbx.ProjectGuid();
				
				var joinedFlags = string.Join(" ", flags);
				
				// Enable MACRO for Swift code
				pbx.SetBuildProperty(projectGuid, "SWIFT_ACTIVE_COMPILATION_CONDITIONS", joinedFlags);
				// Enable MACRO for C++ code
				pbx.SetBuildProperty(projectGuid, "GCC_PREPROCESSOR_DEFINITIONS", joinedFlags);	
			}

			// Unity-IPhone
			var targetGuid = pbx.GetUnityMainTargetGuid();
			pbx.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
			pbx.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
			pbx.SetBuildProperty(targetGuid, "SDKROOT", "iphoneos");
			pbx.SetBuildProperty(targetGuid, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");
			
			// UnityFramework
			pbx.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", "-ObjC");
			var unityFrameworkGuid = pbx.GetUnityFrameworkTargetGuid();
			var unityInterfaceHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityInterface.h");
			var unityForwardDeclsHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityForwardDecls.h");
			var unityRenderingHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityRendering.h");
			var unitySharedDeclsHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnitySharedDecls.h");

			pbx.AddPublicHeaderToBuild(unityFrameworkGuid, unityInterfaceHeaderFile);
			pbx.AddPublicHeaderToBuild(unityFrameworkGuid, unityForwardDeclsHeaderFile);
			pbx.AddPublicHeaderToBuild(unityFrameworkGuid, unityRenderingHeaderFile);
			pbx.AddPublicHeaderToBuild(unityFrameworkGuid, unitySharedDeclsHeaderFile);
			pbx.WriteToFile(pbxPath);
			AddItemsToPlist(path);
		}

		private static void ChangeUnityFrameworkHeader(string path) {
			var headerPath = path + "/UnityFramework/UnityFramework.h";

			var sb = new StringBuilder();
			using (var sr = new StreamReader(headerPath)) {
				string line;
				do {
					line = sr.ReadLine();
					sb.AppendLine(line);
				} while (line != null && !line.Contains("#import \"UnityAppController.h\""));

				sb.Append("#import \"UnityInterface.h\"");
				sb.AppendLine();
				sb.Append(sr.ReadToEnd());
			}

			using (var sr = new StreamWriter(headerPath)) {
				sr.Write(sb.ToString());
			}
		}

		private static void AddItemsToPlist(string path) {
			if (!File.Exists(SkaAdNetworkEditor.SkaAdSavePath))
			{
				return;
			}
			var plistPath = path + "/Info.plist";
			var plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));
			
			var array = plist.root.values.TryGetValue(SkaAdNetworkEditor.SkaKey, out var existingArray)
				? existingArray.AsArray()
				: plist.root.CreateArray(SkaAdNetworkEditor.SkaKey);

			foreach (var id in File.ReadLines(SkaAdNetworkEditor.SkaAdSavePath)) {
				var trimmedID = id.Trim();
				if (string.IsNullOrEmpty(trimmedID)) continue;

				var found = array.values
					.Select(element => element.AsDict())
					.Select(map => 
						map[SkaAdNetworkEditor.SkaItem]
					)
					.Any(storedID => trimmedID == storedID.AsString().Trim());
				
				if (!found) {
					array.AddDict().SetString(SkaAdNetworkEditor.SkaItem, id);
				}
			}
			Debug.unityLogger.Log($"Writing SkAdNetwork ids to {path}");
			#if NIMBUS_ENABLE_ADMOB
				foreach (var id in File.ReadLines("Assets/Editor/AdMobIds")) {
					var trimmedID = id.Trim();
					if (trimmedID.Contains("ios"))
					{
						trimmedID = trimmedID.Remove(0, 4);
						plist.root.SetString("GADApplicationIdentifier",trimmedID);
					}
				}
			#endif
			plist.WriteToFile(plistPath);
		}
	}
}
#endif