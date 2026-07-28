#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    public class CITestSetup
    {
        // This method will be called by GitHub Actions via the command line
        public static void EnableX86_64()
        {
            Debug.Log("🔧 [CI Setup] Configuring Android settings for x86_64 Emulator...");

            // 1. x86_64 strictly requires the IL2CPP scripting backend
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        
            // 2. Set architecture to include both ARM64 (for real devices) and x86_64 (for CI emulator)
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.X86_64;
        
            Debug.Log("✅ [CI Setup] Successfully configured Android architectures for CI testing.");
        }
    }
#endif
