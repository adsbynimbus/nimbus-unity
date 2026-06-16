#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Android;
using UnityEngine;

namespace Nimbus.Editor
{
    public class AndroidConfigurablePostBuildProcessor : IPostGenerateGradleAndroidProject {
		
	    public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
		{
			#if NIMBUS_ENABLE_KOTLIN_UPGRADE
				var projectBuildGradlePath = path + "/../build.gradle";
				var kotlinLine = "id 'org.jetbrains.kotlin.android' version '2.2.0' apply false";
				try
				{
					CompareVersionInFile(projectBuildGradlePath, "org.jetbrains.kotlin.android",
						"2.2", kotlinLine);
				}
				catch (FileNotFoundException)
				{
					Debug.unityLogger.LogError("Nimbus", "Project-level build.gradle not found.");
				}
				catch (Exception)
				{
					InsertTextAfterString(projectBuildGradlePath, "plugins {", kotlinLine);
				}

			#endif

			#if NIMBUS_ENABLE_GRADLE_UPGRADE
				var gradleWrapperPath = path + "/../gradle/wrapper/gradle-wrapper.properties";
				var gradleLine = @"distributionUrl=https\://services.gradle.org/distributions/gradle-8.11.1-all.zip";
				try
				{
					CompareVersionInFile(gradleWrapperPath, "distributionUrl", "8.11.1", gradleLine);
				}
				catch (FileNotFoundException)
				{
					if (Path.GetDirectoryName(gradleWrapperPath) != null)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(gradleWrapperPath));
					}

					File.WriteAllText(gradleWrapperPath, gradleLine);
				}
				catch (Exception)
				{
					File.AppendAllText(gradleWrapperPath, Environment.NewLine + gradleLine);
				}
			#endif
		}
		public static void CompareVersionInFile(
			string filePath,
			string searchText,
			string newVersion,
			string newLine)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			string[] lines = File.ReadAllLines(filePath);

			for (int i = 0; i < lines.Length; i++)
			{
				if (!lines[i].Contains(searchText))
					continue;

				Match match = Regex.Match(lines[i], @"\d+(\.\d+)+");

				Version currentVersion = new Version(match.Value);
				Version targetVersion = new Version(newVersion);

				int comparison = currentVersion.CompareTo(targetVersion);

				if (comparison < 0)
				{
					// Replace the entire line
					lines[i] = newLine;

					File.WriteAllLines(filePath, lines);
				}
			}

			throw new Exception($"Could not find line containing '{searchText}'.");
		}
		
		public static void InsertTextAfterString(
			string filePath,
			string searchText,
			string textToInsert)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			var lines = File.ReadAllLines(filePath).ToList();

			for (int i = 0; i < lines.Count; i++)
			{
				if (lines[i].Contains(searchText))
				{
					lines.Insert(i + 1, textToInsert);
					File.WriteAllLines(filePath, lines);
					return;
				}
			}

			throw new Exception($"Could not find '{searchText}' in file.");

		}
    }
}
#endif