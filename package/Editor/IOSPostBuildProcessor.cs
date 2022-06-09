#if UNITY_EDITOR && UNITY_IOS

using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class IOSPostBuildProcessor {
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string path) {
        if (target == BuildTarget.iOS) {
            ChangeUnityFrameworkHeader(path);

            var pbx = new PBXProject();
            var pbxPath = PBXProject.GetPBXProjectPath(path);
            pbx.ReadFromFile(pbxPath);

            // Unity-IPhone
            var targetGUID = pbx.GetUnityMainTargetGuid();
            pbx.AddBuildProperty(targetGUID, "SWIFT_VERSION", "5.0");
            pbx.SetBuildProperty(targetGUID, "SDKROOT", "iphoneos");
            pbx.SetBuildProperty(targetGUID, "SUPPORTED_PLATFORMS", "iphonesimulator iphoneos");         

            // UnityFramework
            var unityFrameworkGUID = pbx.GetUnityFrameworkTargetGuid();
            var unityInterfaceHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityInterface.h");
            var unityForwardDeclsHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityForwardDecls.h");
            var unityRenderingHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnityRendering.h");
            var unitySharedDeclsHeaderFile = pbx.FindFileGuidByProjectPath("Classes/Unity/UnitySharedDecls.h");

            pbx.AddPublicHeaderToBuild(unityFrameworkGUID, unityInterfaceHeaderFile);
            pbx.AddPublicHeaderToBuild(unityFrameworkGUID, unityForwardDeclsHeaderFile);
            pbx.AddPublicHeaderToBuild(unityFrameworkGUID, unityRenderingHeaderFile);
            pbx.AddPublicHeaderToBuild(unityFrameworkGUID, unitySharedDeclsHeaderFile);

            pbx.WriteToFile(pbxPath);
        }
    }

    private static void ChangeUnityFrameworkHeader(string path) {
        var headerPath = path + "/UnityFramework/UnityFramework.h";

        var sb = new StringBuilder();
        using (var sr = new StreamReader(headerPath)) {
            string line;
            do {
                line = sr.ReadLine();
                sb.AppendLine(line);
            } while (!line.Contains("#import \"UnityAppController.h\""));

            sb.Append("#import \"UnityInterface.h\"");
            sb.AppendLine();
            sb.Append(sr.ReadToEnd());
        }

        using (var sr = new StreamWriter(headerPath)) {
            sr.Write(sb.ToString());
        }
    }
}
#endif
