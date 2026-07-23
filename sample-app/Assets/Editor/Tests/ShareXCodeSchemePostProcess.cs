using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class ShareXcodeSchemePostProcess
{
    // A high execution order (999) ensures this runs after other build scripts
    [PostProcessBuild(999)] 
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        string schemeName = "UnityPluginTests.xcscheme";
        
        // 1. Where the file lives in your Unity project
        string sourceSchemePath = Path.Combine(Application.dataPath, "Editor/iOS", schemeName);
        
        // 2. Where the file needs to go in the exported Xcode project
        string xcodeProjectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj");
        string sharedDataPath = Path.Combine(xcodeProjectPath, "xcshareddata", "xcschemes");
        string destSchemePath = Path.Combine(sharedDataPath, schemeName);

        if (File.Exists(sourceSchemePath))
        {
            // Create the xcshareddata/xcschemes folders if they don't exist
            Directory.CreateDirectory(sharedDataPath);
            
            // Copy the XML scheme file over
            File.Copy(sourceSchemePath, destSchemePath, true);
            Debug.Log($"Successfully injected shared scheme: {schemeName}");
        }
        else
        {
            Debug.LogError($"Could not find the shared scheme file at {sourceSchemePath}. Tests will fail on CI.");
        }
    }
}