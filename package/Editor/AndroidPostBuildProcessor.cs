#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Nimbus.Editor {
	public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {
		
		private static string repoString = @"
allprojects {
	repositories {
		maven {
			url = uri(""https://adsbynimbus-public.s3.amazonaws.com/android/sdks"")
			credentials {
				username = ""*""
			}
		}
	}
}";
		private static string dependencies = @"dependencies {
	implementation ""com.adsbynimbus.android:nimbus:1.10.3""
	implementation ""com.adsbynimbus.android:extension-okhttp:1.10.3""
	implementation ""org.jetbrains.kotlin:kotlin-reflect:1.3.61""
}";

		private static string keepRules = @"
-keep class com.nimbus.demo.UnityHelper { *; }
-keep class com.adsbynimbus.** { *; }";

		public int callbackOrder {
			get { return 999; }
		}

		public void OnPostGenerateGradleAndroidProject(string path) {
			StreamWriter buildWriter = File.AppendText(path + "/build.gradle");			
			#if UNITY_2020_1_OR_NEWER
			writeGradleProps(path + "/../gradle.properties");
			StreamWriter repoWriter = File.AppendText(path + "/../build.gradle");
			repoWriter.WriteLine(repoString);
			repoWriter.Flush();
			repoWriter.Close();
			#else
			writeGradleProps(path + "/gradle.properties");
			buildWriter.WriteLine(repoString);
			buildWriter.Flush();
			#endif

			buildWriter.WriteLine(dependencies);
			buildWriter.Flush();
			buildWriter.Close();

			StreamWriter proguardWriter = File.AppendText(path + "/proguard-unity.txt");
			proguardWriter.WriteLine(keepRules);
			proguardWriter.Flush();
			proguardWriter.Close();		
		}

		private void writeGradleProps(string gradleFile) {
			StreamWriter propWriter = File.AppendText(gradleFile);
			propWriter.WriteLine(@"
android.useAndroidX=true
android.enableJetifier=true");
			propWriter.Flush();
			propWriter.Close();
		}
	}
}
#endif