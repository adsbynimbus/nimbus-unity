using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AddAndroidNativeTests : IPostprocessBuildWithReport
{
    // High execution order ensures this runs after Unity finishes its own Gradle generation
    public int callbackOrder => 99; 

    public void OnPostprocessBuild(BuildReport report)
    {
        // Only run when building for Android
        if (report.summary.platform != BuildTarget.Android) return;

        string exportPath = report.summary.outputPath;
        
        // Target the unityLibrary module where your native classes live
        string unityLibraryPath = Path.Combine(exportPath, "unityLibrary");

        // Paths to your test files inside Unity
        string sourceAndroidTest = Path.Combine(Application.dataPath, "AndroidTests~/Espresso");

        // Paths where Android Studio expects tests to be
        string destAndroidTest = Path.Combine(unityLibraryPath, "src/androidTest/java/com/adsbynimbus/unity");

        CopyDirectory(sourceAndroidTest, destAndroidTest);

        Debug.Log("Successfully injected Kotlin unit tests into the exported Gradle project.");
    }

    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(sourceDir)) return;

        Directory.CreateDirectory(destinationDir);

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            if (file.EndsWith(".meta")) continue; // Don't copy Unity metadata files

            string relativePath = file.Substring(sourceDir.Length + 1);
            string destFile = Path.Combine(destinationDir, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(file, destFile, true);
        }
    }
}