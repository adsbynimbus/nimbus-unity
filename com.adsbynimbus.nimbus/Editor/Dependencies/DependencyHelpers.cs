using System;
using System.IO;
using System.Xml;
using AdsByNimbus.Internal.Utility;
using UnityEngine;

namespace AdsByNimbus.Editor.Dependencies
{
    public class DependencyHelpers
    {
        private const string RelativeFolderPath = "Editor/NimbusFiles_DO_NOT_REMOVE";
        private const string TargetFileName = "NimbusThirdPartyDependencies.xml";

        /// <summary>
        /// Gets the absolute path to Assets/Editor/SpecialFolder/Dependencies.xml.
        /// Creates the directory and a clean XML skeleton if missing.
        /// </summary>
        private static string GetOrCreateXmlPath()
        {
            // 1. Establish the absolute path to Assets/Editor/SpecialFolder
            string folderPath = Path.Combine(Application.dataPath, RelativeFolderPath);

            // 2. Ensure the directory path exists on disk
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string xmlPath = Path.Combine(folderPath, TargetFileName);

            // 3. If the file doesn't exist, build the full EDM4U skeleton structure
            if (!File.Exists(xmlPath))
            {
                try
                {
                    XmlDocument newDoc = new XmlDocument();

                    // Create standard XML declaration
                    XmlDeclaration xmlDeclaration = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    newDoc.AppendChild(xmlDeclaration);

                    // Create the main root <dependencies>
                    XmlElement rootElement = newDoc.CreateElement("dependencies");
                    newDoc.AppendChild(rootElement);

                    // Pre-create the <androidPackages> container tag
                    XmlElement androidPackages = newDoc.CreateElement("androidPackages");
                    rootElement.AppendChild(androidPackages);
                    
                    // Pre-create the <iosPods> container tag for our CocoaPod backups
                    XmlElement iosPods = newDoc.CreateElement("iosPods");
                    rootElement.AppendChild(iosPods);

                    // Commit the baseline file to disk
                    newDoc.Save(xmlPath);

                    #if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
                    #endif
                }
                catch (Exception e)
                {
                    Debug.LogError($"[NimbusXml] Failed to initialize a new XML file: {e.Message}");
                    return null;
                }
            }

            return xmlPath;
        }

        /// <summary>
        /// Adds an Android package to Assets/Editor/SpecialFolder/Dependencies.xml
        /// </summary>
        public static void AddAndroidPackage(string spec, string repositoryUrl = null)
        {
            string absolutePath = GetOrCreateXmlPath();
            if (string.IsNullOrEmpty(absolutePath)) return;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(absolutePath);

                XmlNode rootNode = xmlDoc.SelectSingleNode("//dependencies");
                XmlNode androidPackagesNode = xmlDoc.SelectSingleNode("//androidPackages");

                if (androidPackagesNode == null && rootNode != null)
                {
                    androidPackagesNode = xmlDoc.CreateElement("androidPackages");
                    rootNode.AppendChild(androidPackagesNode);
                }

                XmlNode existing = xmlDoc.SelectSingleNode($"//androidPackage[@spec='{spec}']");
                if (existing != null)
                {
                    Debug.LogWarning($"[NimbusXml] Android package '{spec}' already exists.");
                    return;
                }

                XmlElement newPackage = xmlDoc.CreateElement("androidPackage");
                newPackage.SetAttribute("spec", spec);

                if (!string.IsNullOrEmpty(repositoryUrl))
                {
                    XmlElement repositories = xmlDoc.CreateElement("repositories");
                    XmlElement repository = xmlDoc.CreateElement("repository");
                    repository.InnerText = repositoryUrl;

                    repositories.AppendChild(repository);
                    newPackage.AppendChild(repositories);
                }
                androidPackagesNode.AppendChild(newPackage);

                xmlDoc.Save(absolutePath);

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError($"[NimbusXml] Error adding Android package: {e.Message}");
            }
        }

        /// <summary>
        /// Adds a Swift dependency to Assets/Editor/SpecialFolder/Dependencies.xml
        /// </summary>
        public static void AddSwiftPackage(string url, string version, string swiftName,
            string replacesPod)
        {
            string target = "Unity-iPhone";
            string absolutePath = GetOrCreateXmlPath();
            if (string.IsNullOrEmpty(absolutePath)) return;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(absolutePath);

                XmlNode rootNode = xmlDoc.SelectSingleNode("//dependencies");
                if (rootNode == null) return;

                XmlNode existing = xmlDoc.SelectSingleNode($"//remoteSwiftPackage[@url='{url}']");
                if (existing != null)
                {
                    Debug.LogWarning($"[NimbusXml] Swift package URL '{url}' already exists.");
                    return;
                }

                XmlElement remoteSwift = xmlDoc.CreateElement("remoteSwiftPackage");
                remoteSwift.SetAttribute("url", url);
                remoteSwift.SetAttribute("version", version);
                remoteSwift.SetAttribute("target", target);

                XmlElement swiftPackage = xmlDoc.CreateElement("swiftPackage");
                swiftPackage.SetAttribute("name", swiftName);
                swiftPackage.SetAttribute("replacesPod", replacesPod);

                remoteSwift.AppendChild(swiftPackage);
                rootNode.AppendChild(remoteSwift);

                XmlNode iosPodsNode = xmlDoc.SelectSingleNode("//iosPods");
                if (iosPodsNode == null)
                {
                    iosPodsNode = xmlDoc.CreateElement("iosPods");
                    rootNode.AppendChild(iosPodsNode);
                }

                XmlNode existingPod = xmlDoc.SelectSingleNode($"//iosPod[@name='{swiftName}']");
                if (existingPod == null)
                {
                    XmlElement iosPod = xmlDoc.CreateElement("iosPod");
                    iosPod.SetAttribute("name", swiftName); // Same name as the Swift package
                    iosPod.SetAttribute("version", version); // Same version as the Swift package
                    iosPod.SetAttribute("bitcodeEnabled", "false");
                    iosPod.SetAttribute("addToAllTargets", "false");

                    iosPodsNode.AppendChild(iosPod);
                }
                
                xmlDoc.Save(absolutePath);

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError($"[NimbusXml] Error adding Swift package: {e.Message}");
            }
        }

        /// <summary>
        /// Removes an Android package completely by matching its partial name prefix.
        /// </summary>
        public static void RemoveAndroidPackage(string packageName)
        {
            string absolutePath = GetOrCreateXmlPath();
            if (string.IsNullOrEmpty(absolutePath)) return;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(absolutePath);

                XmlNode targetNode = xmlDoc.SelectSingleNode($"//androidPackage[starts-with(@spec, '{packageName}')]");
                if (targetNode != null)
                {
                    targetNode.ParentNode.RemoveChild(targetNode);
                    xmlDoc.Save(absolutePath);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[NimbusXml] Error removing Android package: {e.Message}");
            }
        }

        /// <summary>
        /// Removes a remote Swift package block using its internal name attribute.
        /// </summary>
        public static void RemoveSwiftPackage(string swiftPackageName)
        {
            string absolutePath = GetOrCreateXmlPath();
            if (string.IsNullOrEmpty(absolutePath)) return;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(absolutePath);

                bool isModified = false;

                // Remove SPM
                XmlNode swiftNode = xmlDoc.SelectSingleNode($"//remoteSwiftPackage[swiftPackage[@name='{swiftPackageName}']]");
                if (swiftNode != null)
                {
                    swiftNode.ParentNode.RemoveChild(swiftNode);
                    isModified = true;
                }

                // Remove CocoaPod
                XmlNode podNode = xmlDoc.SelectSingleNode($"//iosPod[@name='{swiftPackageName}']");
                if (podNode != null)
                {
                    podNode.ParentNode.RemoveChild(podNode);
                    isModified = true;
                }

                if (isModified)
                {
                    xmlDoc.Save(absolutePath);
                    UnityEditor.AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SpecialFolderXml] Error deleting dependencies: {e.Message}");
            }
        }
    }
}