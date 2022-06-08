#if UNITY_EDITOR && UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class IOSPostBuildProcessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path) {
        if (target == BuildTarget.iOS) {
            var targetName = "Unity-iPhone";

            var proj = new PBXProject();
            var projPath = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(projPath);

#if UNITY_2020_2_OR_NEWER
            var targetGUID = proj.GetUnityMainTargetGuid();
#else
            var targetGUID = pbxProject.TargetGuidByName(targetName);
#endif
            proj.AddBuildProperty(targetGUID, "SWIFT_VERSION", "5.0");
            proj.AddBuildProperty(targetGUID, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/com.adsbynimbus.unity/Runtime/Plugins/iOS/NimbusSDK-Bridging-Header.h");
            proj.AddBuildProperty(targetGUID, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "UnityFramework-Swift.h");
            proj.SetBuildProperty(targetGUID, "SDKROOT", "iOS");
            proj.SetBuildProperty(targetGUID, "SUPPORTED_PLATFORMS", "iOS");

            string frameworkGUID = proj.GetUnityFrameworkTargetGuid();
            proj.AddPublicHeaderToBuild(frameworkGUID, "Libraries/com.adsbynimbus.unity/Runtime/Plugins/iOS/SendMessageInterface.h");

            proj.WriteToFile(projPath);
        }
    }
}
#endif
