#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;

public class CITestSetup : IPreprocessBuildWithReport
{
    // Determines when this script runs if you have multiple pre-build scripts. 0 is fine.
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // GitHub Actions automatically sets this environment variable to "true"
        bool isRunningOnGitHub = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

        if (isRunningOnGitHub && report.summary.platform == BuildTarget.Android)
        {
            Debug.Log("🔧 [CI Setup] GitHub Actions detected! Forcing x86_64 architecture for Emulator testing...");

            // 1. Enforce IL2CPP (Required for x86_64)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            
            // 2. Add x86_64 to the architectures alongside ARM64
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.X86_64;
            
            Debug.Log("✅ [CI Setup] Architecture set successfully.");
        }
        else
        {
            Debug.Log("🏠 [CI Setup] Local build detected. Leaving architectures unchanged.");
        }
    }
}
#endif
