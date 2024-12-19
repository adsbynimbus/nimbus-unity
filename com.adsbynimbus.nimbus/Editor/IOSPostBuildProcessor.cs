#if UNITY_EDITOR && UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nimbus.Internal.Utility;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Nimbus.Editor {
	public class PostProcessIOS : MonoBehaviour
	{
		private static readonly List<string> Dependencies = new List<string>
		{
			"'NimbusKit'",
			"'NimbusRenderVideoKit'",
			"'NimbusRenderStaticKit'",
			"'NimbusRenderVASTKit'"
		};
		[PostProcessBuild(45)]
		private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
		{
			if (target == BuildTarget.iOS)
			{
				#if NIMBUS_ENABLE_APS
					Dependencies.Add("'NimbusRequestAPSKit'");
				#endif
				#if NIMBUS_ENABLE_VUNGLE
					Dependencies.Add("'NimbusVungleKit'");
				#endif
				
				var path = buildPath + "/Podfile";
				var lines = File.ReadAllLines(path);
				for(int i = 0 ; i < lines.Length ; i++)
				{
					if (lines[i].ToLower().Contains("nimbussdk"))
					{
						lines[i] = ($"{lines[i]}, subspecs: [{string.Join<string>(", ", Dependencies)}]");
					}
				}
				File.WriteAllLines(path, lines);
			}
		}
	}

	public class IOSPostBuildProcessor {
		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string path) {
			ChangeUnityFrameworkHeader(path);

			var pbx = new PBXProject();
			var pbxPath = PBXProject.GetPBXProjectPath(path);
			pbx.ReadFromFile(pbxPath);

			#if NIMBUS_ENABLE_APS
				var projectGuid = pbx.ProjectGuid();
				// Enable MACRO for Swift code
				pbx.SetBuildProperty(projectGuid, "SWIFT_ACTIVE_COMPILATION_CONDITIONS", "NIMBUS_ENABLE_APS");
				// Enable MACRO for C++ code
				pbx.SetBuildProperty(projectGuid, "GCC_PREPROCESSOR_DEFINITIONS", "NIMBUS_ENABLE_APS");
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				var projectGuid = pbx.ProjectGuid();
				// Enable MACRO for Swift code
				pbx.SetBuildProperty(projectGuid, "SWIFT_ACTIVE_COMPILATION_CONDITIONS", "NIMBUS_ENABLE_VUNGLE");
				// Enable MACRO for C++ code
				pbx.SetBuildProperty(projectGuid, "GCC_PREPROCESSOR_DEFINITIONS", "NIMBUS_ENABLE_VUNGLE");
			#endif

			// Unity-IPhone
			var targetGuid = pbx.GetUnityMainTargetGuid();
			pbx.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
			pbx.SetBuildProperty(targetGuid, "SDKROOT", "iphoneos");
			pbx.SetBuildProperty(targetGuid, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");

			// UnityFramework
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
			AddSkaAdNetworkIdsToPlist(path);
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

		private static void AddSkaAdNetworkIdsToPlist(string path) {
			if (!File.Exists(SkaAdNetworkEditor.SkaAdSavePath)) {
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
				if (trimmedID.IsNullOrEmpty()) continue;

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
			plist.WriteToFile(plistPath);
		}
	}
}
#endif