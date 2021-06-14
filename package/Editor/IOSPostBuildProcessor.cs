#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class IOSPostBuildProcessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path) {
        if (target == BuildTarget.iOS) {
            string projPath = PBXProject.GetPBXProjectPath(path);

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            string targetGUID = proj.TargetGuidByName("Unity-iPhone");
            proj.AddBuildProperty(targetGUID, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(targetGUID, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/com.adsbynimbus.unity/Runtime/Plugins/iOS/NimbusSDK-Bridging-Header.h");
            proj.SetBuildProperty(targetGUID, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "NimbusSDK-Swift.h");

            proj.WriteToFile(projPath);
        }
    }
}
#endif
