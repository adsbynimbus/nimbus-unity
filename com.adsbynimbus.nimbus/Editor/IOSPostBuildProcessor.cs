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
		private const string SdkVersion = VersionConstants.IosSdkVersion;
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
			if (target != BuildTarget.iOS) return;
			
			#if NIMBUS_ENABLE_LIVERAMP
				Dependencies.Add("'NimbusRequestKit'");
				Dependencies.Add("'NimbusLiveRampKit'");
			#endif
			
			#if NIMBUS_ENABLE_APS
				Dependencies.Add("'NimbusRequestAPSKit'");
			#endif
			#if NIMBUS_ENABLE_VUNGLE
				Dependencies.Add("'NimbusVungleKit'");
			#endif
			#if NIMBUS_ENABLE_ADMOB
                Dependencies.Add("'NimbusAdMobKit'");
			#endif
			
			#if NIMBUS_ENABLE_META
			    Dependencies.Add("'NimbusRequestFANKit'");
				Dependencies.Add("'NimbusRenderFANKit'");
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL
				Dependencies.Add("'NimbusMintegralKit'");
			#endif

			#if NIMBUS_ENABLE_UNITY_ADS
				Dependencies.Add("'NimbusUnityKit'");
			#endif
			
			#if NIMBUS_ENABLE_MOBILEFUSE
				Dependencies.Add("'NimbusMobileFuseKit'");
			#endif
			
			var path = buildPath + "/Podfile";
			if (!File.Exists(path)) {
				CreatePodfile(buildPath);
				Debug.unityLogger.Log($"Copying generating pod file to {path}");
			}
			else {
				Debug.unityLogger.Log("Podfile already exists");
			}
			var lines = File.ReadAllLines(path);
			for(int i = 0 ; i < lines.Length ; i++)
			{
				if (lines[i].Contains("NimbusSDK"))
				{
					lines[i] = ($"{lines[i]}, subspecs: [{string.Join<string>(", ", Dependencies)}]");
				}
			}
			File.WriteAllLines(path, lines);

			var postInstallScript = @"
post_install do |installer|
  allowed_frameworks = [
    'NimbusCoreKit',
    'NimbusRequestKit',
    'NimbusRenderKit',
    'NimbusRenderStaticKit',
    'NimbusRenderVideoKit',
    'NimbusRenderVASTKit',
    'NimbusRequestAPSKit',
    'NimbusKit',
    'OMSDK_Adsbynimbus',
    'DTBiOSSDK',
    'GoogleInteractiveMediaAds',
    'GoogleMobileAds',
    'MobileFuseSDK',
    'MTGSDK',
    'MTGSDKBanner',
    'MTGSDKBidding',
    'MTGSDKNewInterstitial',
    'MTGSDKReward',
    'UnityAds',
	'NimbusLiveRampKit',
	'LRAtsSDK'
  ]

  main_project = installer.aggregate_targets.first.user_project

  if main_project.nil?
    puts 'Warning: Could not find the main project.'
    return
  end

  unity_iphone_target = main_project.targets.find { |target| target.name == 'Unity-iPhone' }

  if unity_iphone_target.nil?
    puts 'Warning: Could not find Unity-iPhone target in the main project.'
    return
  end

  xcframework_files = Dir.glob(""#{installer.sandbox.root}/**/*.xcframework"")

  xcframework_files.each do |xcframework_path|
    framework_name = File.basename(xcframework_path)
    framework_name_no_ext = File.basename(xcframework_path, File.extname(framework_name))
    
    unless allowed_frameworks.include?(framework_name_no_ext)
      next
    end

    framework_ref = main_project.frameworks_group.files.find do |file|
      file.name == framework_name || file.real_path.to_s == xcframework_path
    end

    if framework_ref.nil?
      framework_ref = main_project.frameworks_group.new_file(xcframework_path)
      puts ""Added #{framework_name} to the project.""
    else
      puts ""#{framework_name} already exists in the project.""
    end

    embed_phase = unity_iphone_target.build_phases.find { |phase| phase.display_name == 'Embed Frameworks' } || unity_iphone_target.new_copy_files_build_phase('Embed Frameworks')
    build_file = embed_phase.files_references.find { |file| file.name == framework_name }

    if build_file.nil?
      build_file = embed_phase.add_file_reference(framework_ref)
      build_file.settings = { 'ATTRIBUTES' => ['CodeSignOnCopy'] } # Embed & Sign
      puts ""Embedded #{framework_name} into Unity-iPhone target.""
    else
      puts ""#{framework_name} is already embedded in Unity-iPhone target.""
    end
  end

  main_project.save
end";
			
			File.AppendAllText(path, postInstallScript);
		}
		
		private static void CreatePodfile(string pathToBuiltProject)
		{
			var fullDependencies = BuildDependencies();
			var destPodfilePath = pathToBuiltProject + "/Podfile"; 
			File.WriteAllText(destPodfilePath, fullDependencies); 
			Debug.unityLogger.Log($"Copying generating pod file to {destPodfilePath}");
		}

		
		private static string BuildDependencies() {
			var builder = new StringBuilder();
			
			builder.AppendLine(@"platform :ios, '13.0'
				use_frameworks!
				source 'https://cdn.cocoapods.org/'
				");
			if (SdkVersion.Contains("internal")) {
				builder.AppendLine("source 'git@github.com:adsbynimbus/Specs.git'");
			}

			builder.AppendLine("def sdk_dependencies");
			builder.AppendLine($"  pod 'NimbusSDK', '{SdkVersion}'");
			builder.AppendLine("end");


			builder.AppendLine(@"
			target 'UnityFramework' do
			  sdk_dependencies
			end
			");
			return builder.ToString();
		}
	}

	public class IOSPostBuildProcessor {
		[PostProcessBuild]
		public static void OnPostprocessBuild(BuildTarget target, string path) {
			ChangeUnityFrameworkHeader(path);

			var pbx = new PBXProject();
			var pbxPath = PBXProject.GetPBXProjectPath(path);
			pbx.ReadFromFile(pbxPath);

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
			
			#if NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE
				flags.Add("NIMBUS_ENABLE_SDK_DEMAND");
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