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

			var flags = new List<string>();
			
			#if NIMBUS_ENABLE_LIVERAMP
				flags.Add("NIMBUS_ENABLE_LIVERAMP");    
			#endif
			
			#if NIMBUS_ENABLE_APS
				flags.Add("NIMBUS_ENABLE_APS");    
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				flags.Add("NIMBUS_ENABLE_VUNGLE");
			#endif
			
			#if NIMBUS_ENABLE_META
				flags.Add("NIMBUS_ENABLE_META");
			#endif
			
			#if NIMBUS_ENABLE_ADMOB
				flags.Add("NIMBUS_ENABLE_ADMOB");
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL
				flags.Add("NIMBUS_ENABLE_MINTEGRAL");
			#endif

			#if NIMBUS_ENABLE_UNITY_ADS
				flags.Add("NIMBUS_ENABLE_UNITY_ADS");
			#endif
			
			#if NIMBUS_ENABLE_MOBILEFUSE
				flags.Add("NIMBUS_ENABLE_MOBILEFUSE");
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO
				flags.Add("NIMBUS_ENABLE_MOLOCO");
			#endif
			
			#if NIMBUS_ENABLE_INMOBI
				flags.Add("NIMBUS_ENABLE_INMOBI");
			#endif
			
			#if NIMBUS_ENABLE_DIGITAL_TURBINE
				flags.Add("NIMBUS_ENABLE_DIGITAL_TURBINE");
			#endif

			#if NIMBUS_ENABLE_DISPLAY_IO
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