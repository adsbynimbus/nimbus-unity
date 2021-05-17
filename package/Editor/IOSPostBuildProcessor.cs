#if UNITY_EDITOR && UNITY_IOS
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
            proj.AddBuildProperty(targetGUID, "ENABLE_BITCODE", "YES");
            //proj.SetBuildProperty(targetGUID, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "NimbusSDK-Swift.h");
            proj.AddBuildProperty(targetGUID, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            proj.SetBuildProperty(targetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            //proj.SetBuildProperty(targetGUID, "SWIFT_OBJC_BRIDGING_HEADER", "/Users/bruno/dev/nimbus-unity/package/Runtime/Plugins/iOS/Source/NimbusUnityKit/NimbusUnityKit-Bridge-Bridging-Header.h");
            //proj.AddBuildProperty(targetGUID, "EMBEDDED_CONTENT_CONTAINS_SWIFT", "YES");
            proj.AddBuildProperty(targetGUID, "HEADER_SEARCH_PATHS", "($(inherited),  \"$(SRCROOT)/Classes\",  \"$(SRCROOT)\",  $(SRCROOT)/Classes/Native,  $(SRCROOT)/Libraries/bdwgc/include,  $(SRCROOT)/Libraries/libil2cpp/include,))");


            proj.WriteToFile(projPath);
        }
    }
}
#endif