#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using UnityEditor.Android;

namespace Nimbus.Editor {
	public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {
		private const string RepoString = @"
allprojects {
	repositories {
		maven {
			url = uri(""https://adsbynimbus-public.s3.amazonaws.com/android/sdks"")
			credentials {
				username = ""*""
			}
			content {
                includeGroup(""com.adsbynimbus.android"")
                includeGroup(""com.adsbynimbus.openrtb"")
                includeGroup(""com.iab.omid.library.adsbynimbus"")
            }
		}
	}
}";

		private static readonly string Dependencies = AndroidBuildDependencies.BuildDependencies();

		private const string KeepRules = @"
-keep class com.nimbus.demo.UnityHelper { *; }
-keep class com.adsbynimbus.** { *; }";

		private const string PackagingOptions = @"
android {
	packagingOptions {
    	pickFirst ""META-INF/*.kotlin_module""
	}
}";

		public int callbackOrder => 999;

		public void OnPostGenerateGradleAndroidProject(string path) {
			WriteGradleProps(path + "/../gradle.properties");
			var repoWriter = File.AppendText(path + "/../build.gradle");
			repoWriter.WriteLine(RepoString);
			repoWriter.Flush();
			repoWriter.Close();

			var buildWriter = File.AppendText(path + "/build.gradle");
			buildWriter.WriteLine(Dependencies);
			buildWriter.Flush();
			buildWriter.Close();

			var proguardWriter = File.AppendText(path + "/proguard-unity.txt");
			proguardWriter.WriteLine(KeepRules);
			proguardWriter.Flush();
			proguardWriter.Close();

			var packagingWriter = File.AppendText(path + "/../launcher/build.gradle");
			packagingWriter.WriteLine(PackagingOptions);
			packagingWriter.Flush();
			packagingWriter.Close();
		}

		private static void WriteGradleProps(string gradleFile) {
			var propWriter = File.AppendText(gradleFile);
			propWriter.WriteLine(@"
android.useAndroidX=true
");
			propWriter.Flush();
			propWriter.Close();
		}
	}
}
#endif