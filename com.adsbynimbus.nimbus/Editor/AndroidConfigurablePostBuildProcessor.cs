#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Android;

namespace Nimbus.Editor
{
    public class AndroidConfigurablePostBuildProcessor : IPostGenerateGradleAndroidProject {
		
	    public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
		{
			#if NIMBUS_ENABLE_KOTLIN_UPGRADE
				UpdateGradleFileForKotlinVersion(path + "/../build.gradle");
			#endif

			#if NIMBUS_ENABLE_GRADLE_UPGRADE
				var gradleWrapperPath = path + "/../gradle/wrapper/gradle-wrapper.properties";
				var gradleLine = @"distributionUrl=https\://services.gradle.org/distributions/gradle-8.11.1-all.zip";
				try
				{
					CompareVersionInFile(gradleWrapperPath, "distributionUrl", "8.11.1", gradleLine);
				}
				catch (Exception e)
				{
					if (e.GetType() == typeof(FileNotFoundException))
					{
						if (Path.GetDirectoryName(gradleWrapperPath) != null)
						{
							Directory.CreateDirectory(Path.GetDirectoryName(gradleWrapperPath));
						}
						File.WriteAllText(gradleWrapperPath, gradleLine);
					}
					else
					{
						File.AppendAllText(gradleWrapperPath, Environment.NewLine + gradleLine);
					}
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
		
		public static void UpdateGradleFileForKotlinVersion(
			string filePath)
		{
			var kotlinLine = "id 'org.jetbrains.kotlin.android' version '2.2.0' apply false";
			var kotlinLineFound = false;
			var kotlinAddLineIndex = -1;

			
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			string[] lines = File.ReadAllLines(filePath);
			bool updated = false;
			var versionMappings = new Dictionary<string, string>
			{
				{"org.jetbrains.kotlin.android", "2.2.0" },
				{"com.android.application", "8.13.2" },
				{"com.android.library", "8.13.2" }
			};

			for (int i = 0; i < lines.Length; i++)
			{
				foreach (var mapping in versionMappings)
				{
					string searchText = mapping.Key;
					string targetVersionString = mapping.Value;

					if (lines[i].Contains("plugins {"))
					{
						kotlinAddLineIndex = i;
					}

					if (!lines[i].Contains(searchText))
						continue;

					Match match = Regex.Match(lines[i], @"\d+(\.\d+)+");

					if (!match.Success)
						continue;

					if (searchText == "org.jetbrains.kotlin.android")
					{
						kotlinLineFound = true;
					}
					
					Version currentVersion = new Version(match.Value);
					Version targetVersion = new Version(targetVersionString);

					if (currentVersion.CompareTo(targetVersion) < 0)
					{
						lines[i] = Regex.Replace(
							lines[i],
							@"\d+(\.\d+)+",
							targetVersionString);

						updated = true;
					}

					break; // stop checking other mappings for this line
				}
			}

			if (!kotlinLineFound)
			{
				lines[kotlinAddLineIndex] += kotlinLine;
			}
			
			if (updated)
				File.WriteAllLines(filePath, lines);
			
		}
    }
}
#endif