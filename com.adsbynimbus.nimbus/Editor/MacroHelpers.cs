#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace Nimbus.Editor
{
    public static class MacroHelpers
    {
        
        public static bool IsBuildMacroSet(BuildTargetGroup group, string buildMacro) {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
            return macros.Any(macro => macro == buildMacro);
        }

        public static void SetBuildMacroForGroup(BuildTargetGroup group, string buildMacro) {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
            if (macros.Any(macro => macro == buildMacro)) {
                return;
            }

            var enumerable = macros.Append(buildMacro);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, enumerable.ToArray());
        }

        public static void RemoveBuildMacroForGroup(BuildTargetGroup group, string buildMacro) {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
            macros = macros.Where((source, index) => source != buildMacro).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, macros.ToArray());
        }
        
        public static void RemoveBuildMacroForBothPlatforms(string buildMacro)
        {
            RemoveBuildMacroForGroup(BuildTargetGroup.iOS, buildMacro);
            RemoveBuildMacroForGroup(BuildTargetGroup.Android, buildMacro);
        }
		
        public static void SetBuildMacroForBothPlatforms(string buildMacro)
        {
            SetBuildMacroForGroup(BuildTargetGroup.iOS, buildMacro);
            SetBuildMacroForGroup(BuildTargetGroup.Android, buildMacro);
        }

    }
}
#endif