using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;

public class AddiOSNativeTests
{
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        // 1. Initialize the Xcode Project file reader
        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string mainTargetGuid = proj.GetUnityMainTargetGuid(); 

        // 2. Generate the Unit Test Target Shell
        string testTargetGuid = proj.AddTarget(
            "UnityPluginTests", 
            "xctest", 
            "com.apple.product-type.bundle.unit-test"
        );

        // Initialize the compile sources build phase for the new target
        proj.AddSourcesBuildPhase(testTargetGuid); 

        // Configure Info.plist generation and bundle tracking flags
        proj.SetBuildProperty(testTargetGuid, "GENERATE_INFOPLIST_FILE", "YES");
        proj.SetBuildProperty(testTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", "com.mycompany.UnityPluginTests");
        proj.SetBuildProperty(testTargetGuid, "PRODUCT_NAME", "UnityPluginTests");
        proj.SetBuildProperty(testTargetGuid, "EXECUTABLE_NAME", "UnityPluginTests");

        // Map Target Host and Loader environment macros to the application target
        proj.SetBuildProperty(testTargetGuid, "TEST_HOST", "$(BUILT_PRODUCTS_DIR)/NimbusSampleUnityApp.app/NimbusSampleUnityApp");
        proj.SetBuildProperty(testTargetGuid, "BUNDLE_LOADER", "$(TEST_HOST)");

        // Map header tracking criteria for core compilation support
        proj.AddBuildProperty(testTargetGuid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Classes/**");
        proj.AddBuildProperty(testTargetGuid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/**");
        proj.SetBuildProperty(mainTargetGuid, "ENABLE_TESTABILITY", "YES");

        // Establish core target dependency sequence rules
        proj.AddTargetDependency(testTargetGuid, mainTargetGuid);

        // ====================================================================
        // 3. COMPILE PRODUCTION AND TEST FILES DIRECTLY TOGETHER
        // ====================================================================
        
        // Setup local source and physical directory targets
        string productionSourcePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages/com.adsbynimbus.nimbus/Runtime/Plugins/iOS");       
        string unityTestsSourcePath = Path.Combine(Application.dataPath, "iOS/Tests/Unit");
        string xcodeTestsDestPath = Path.Combine(pathToBuiltProject, "UnityPluginTests");

        if (!Directory.Exists(xcodeTestsDestPath))
        {
            Directory.CreateDirectory(xcodeTestsDestPath);
        }

        List<string> allFilesToCompile = new List<string>();

        // Gather test logic structures
        if (Directory.Exists(unityTestsSourcePath))
        {
            allFilesToCompile.AddRange(Directory.GetFiles(unityTestsSourcePath, "*.swift"));
            allFilesToCompile.AddRange(Directory.GetFiles(unityTestsSourcePath, "*.m"));
        }

        // Gather plugin source code files to bypass dynamic linking barriers
        if (Directory.Exists(productionSourcePath))
        {
            allFilesToCompile.AddRange(Directory.GetFiles(productionSourcePath, "*.swift", SearchOption.AllDirectories));
        }
        
        // Process file injection operations
        foreach (string file in allFilesToCompile)
        {
            string fileName = Path.GetFileName(file);
            string destinationFile = Path.Combine(xcodeTestsDestPath, fileName);

            // Execute local OS file copy sequence
            File.Copy(file, destinationFile, true);

            // FORCE FORWARD SLASHES: Xcode strictly drops paths with backslashes (\)
            string projectRelativePath = "UnityPluginTests/" + fileName;
            
            // Map file indexes and bind compilation flags explicitly
            string fileGuid = proj.AddFile(projectRelativePath, projectRelativePath, PBXSourceTree.Source);
            proj.AddFileToBuild(testTargetGuid, fileGuid);
        }

        // Save modifications to the Xcode project structures
        File.WriteAllText(projPath, proj.WriteToString());
        Debug.Log("Successfully built Xcode structural configurations and targets.");

        // ====================================================================
        // 4. AUTOMATIC SCHEME INJECTION PASS
        // ====================================================================
        AddTestTargetToXcodeScheme(pathToBuiltProject, "UnityPluginTests");
    }

    private static void AddTestTargetToXcodeScheme(string pathToBuiltProject, string testTargetName)
    {
        string schemePath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme");
        
        if (!File.Exists(schemePath)) return;

        XmlDocument doc = new XmlDocument();
        doc.Load(schemePath);

        XmlNode testActionNode = doc.SelectSingleNode("//TestAction");
        if (testActionNode != null)
        {
            XmlNode testablesNode = testActionNode.SelectSingleNode("Testables");
            if (testablesNode == null)
            {
                testablesNode = doc.CreateElement("Testables");
                testActionNode.AppendChild(testablesNode);
            }

            if (testablesNode.SelectSingleNode($"//TestableReference[contains(@BlueprintName, '{testTargetName}')]") == null)
            {
                XmlElement testableRef = doc.CreateElement("TestableReference");
                testableRef.SetAttribute("skipped", "NO");

                XmlElement buildableRef = doc.CreateElement("BuildableReference");
                buildableRef.SetAttribute("BuildableIdentifier", "primary");
                buildableRef.SetAttribute("BlueprintName", testTargetName);
                
                string mockGuid = System.Guid.NewGuid().ToString("N").Substring(0, 24).ToUpper();
                buildableRef.SetAttribute("BlueprintIdentifier", mockGuid);
                
                buildableRef.SetAttribute("BuildProductRunnable", "No");
                buildableRef.SetAttribute("ReferencedContainer", "container:Unity-iPhone.xcodeproj");

                testableRef.AppendChild(buildableRef);
                testablesNode.AppendChild(testableRef);
                
                doc.Save(schemePath);
                Debug.Log($"Successfully injected '{testTargetName}' into the Unity-iPhone Xcode Scheme.");
            }
        }
    }
}
#endif
