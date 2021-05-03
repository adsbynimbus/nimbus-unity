#if UNITY_EDITOR
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Nimbus.Editor {
	public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {
		public int callbackOrder {
			get { return 999; }
		}

		public void OnPostGenerateGradleAndroidProject(string path) {			
			#if !UNITY_2020_1_OR_NEWER
			StreamWriter buildWriter = File.AppendText(path + "/build.gradle");
			buildWriter.WriteLine(@"
allprojects {
	repositories {
		maven {
			url = uri(""https://adsbynimbus-public.s3.amazonaws.com/android/sdks"")
			credentials {
				username = ""*""
			}
		}
	}
}");
			buildWriter.WriteLine(@"
dependencies {
	implementation ""com.adsbynimbus.android:nimbus:1.10.0""
	implementation ""com.adsbynimbus.android:extension-okhttp:1.10.0""
	implementation ""org.jetbrains.kotlin:kotlin-reflect:1.4.31""
}");
			buildWriter.Flush();
			buildWriter.Close();

			StreamWriter proguardWriter = File.AppendText(path + "/proguard-unity.txt");
			proguardWriter.WriteLine(@"
-keep class com.nimbus.demo.UnityHelper { *; }
-keep class com.adsbynimbus.** { *; }");
			proguardWriter.Flush();
			proguardWriter.Close();
			writeGradleProps(path + "/gradle.properties")
			#else
			writeGradleProps(path + "/../gradle.properties")
			#endif				
		}

		private void writeGradleProps(String path) {
			StreamWriter propWriter = File.AppendText();
			propWriter.WriteLine(@"
android.useAndroidX=true
android.enableJetifier=true");
			propWriter.Flush();
			propWriter.Close();
		}
	}
}
#endif