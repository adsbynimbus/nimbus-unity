using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class AddiOSNativeTests
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(pbxProjectPath));

        // 1. Create Unit Test Target
        string unitTestTarget = project.AddTarget("NativeUnitTests", "xctest", "com.apple.product-type.bundle.unit-test");
        string unityUnitPath = Path.Combine(UnityEngine.Application.dataPath, "Plugins/iOS/Tests/Unit");
        ProcessTestFolder(project, pathToBuiltProject, unitTestTarget, unityUnitPath, "NativeUnitTests");

        // 2. Create UI Test Target
        string uiTestTarget = project.AddTarget("NativeUITests", "xctest", "com.apple.product-type.bundle.ui-testing");
        string unityUIPath = Path.Combine(UnityEngine.Application.dataPath, "Plugins/iOS/Tests/UI");
        ProcessTestFolder(project, pathToBuiltProject, uiTestTarget, unityUIPath, "NativeUITests");

        // Save modifications
        File.WriteAllText(pbxProjectPath, project.WriteToString());
    }

    private static void ProcessTestFolder(PBXProject project, string buildPath, string targetGuid, string unityFolderPath, string targetName)
    {
        if (!Directory.Exists(unityFolderPath)) return;

        string[] testFiles = Directory.GetFiles(unityFolderPath, "*.swift");
        foreach (string file in testFiles)
        {
            string fileName = Path.GetFileName(file);
            string targetPath = Path.Combine(buildPath, targetName, fileName);
            
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(file, targetPath, true);

            string fileGuid = project.AddFile(targetName + "/" + fileName, targetName + "/" + fileName, PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, fileGuid);
        }
    }
}
