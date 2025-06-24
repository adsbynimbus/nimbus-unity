#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using System.Text;
using Nimbus.Internal.Utility;
using UnityEditor.Android;

namespace Nimbus.Editor {
	public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {

		private const string KeepRules = @"
-keep class com.nimbus.demo.UnityHelper { *; }
-keep class com.adsbynimbus.** { *; }";

		private const string PackagingOptions = @"
			android {
				packagingOptions {
					pickFirst ""META-INF/*.kotlin_module""
				}
			}
			androidComponents.finalizeDsl { dsl ->
				if (dsl.compileSdk == 34) {
					dependencies {
						constraints {
							implementation(""androidx.core:core-ktx:1.13.1"")
							implementation(""androidx.fragment:fragment:1.7.1"")
							implementation(""androidx.lifecycle:lifecycle-runtime-ktx:2.7.0"")
							implementation(""androidx.media3:media3-exoplayer:1.3.1"")
							implementation(""androidx.media3:media3-exoplayer-hls:1.3.1"")
						}
					}
				}
			}
			// Always update Androidx Collection to 1.4.5
			dependencies {
			    constraints {
			        implementation(""androidx.collection:collection:1.4.5"")
			    }
			}
			// Force Play Services Ads Indentifier to 18.1.0 if using Java 8
			afterEvaluate {
			    android.compileOptions {
			        if (targetCompatibility < JavaVersion.VERSION_11 && !isCoreLibraryDesugaringEnabled) {
			            configurations.configureEach {
			                resolutionStrategy.force(""com.google.android.gms:play-services-ads-identifier:18.1.0"")
			            }
			        }
			    }
			}";

		public int callbackOrder => 999;

		public void OnPostGenerateGradleAndroidProject(string path)
		{
			var extraRepositories = "";
			#if NIMBUS_ENABLE_MINTEGRAL
				extraRepositories =
					@"
					maven {
					url ""https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea""  
											content {
												includeGroupByRegex("".*\\.mbridge.*"")
											}}";
			#endif
			#if NIMBUS_ENABLE_LIVERAMP
				extraRepositories +=
					@"        
						maven {
						url = uri(""https://sdk-android-prod.launch.liveramp.com"")
							content {
							includeGroupByRegex("".*\\.liveramp.*"")
							}}";
			#endif
			string repoString = @"
				dependencyResolutionManagement {
				    repositories {
								        maven {
											url ""https://adsbynimbus-public.s3.amazonaws.com/android/sdks"" 
											content {
				                includeGroupByRegex("".*\\.adsbynimbus.*"")
										}" + extraRepositories + "}}}";
			WriteGradleProps(path + "/../gradle.properties");
			var repoWriter = File.AppendText(path + "/../settings.gradle");
			repoWriter.WriteLine(repoString);
			repoWriter.Flush();
			repoWriter.Close();
			
			RunEdm4uCheck(path);
			var proguardWriter = File.AppendText(path + "/proguard-unity.txt");
			proguardWriter.WriteLine(KeepRules);
			proguardWriter.Flush();
			proguardWriter.Close();
			
			var packagingWriter = File.AppendText(path + "/../launcher/build.gradle");
			packagingWriter.WriteLine(PackagingOptions);
			packagingWriter.Flush();
			packagingWriter.Close();
			
			#if NIMBUS_ENABLE_ADMOB
				//pull saved appId from file
				var trimmedID = "";
				foreach (var id in File.ReadLines("Assets/Editor/AdMobIds")) {
					trimmedID = id.Trim();
					if (trimmedID.Contains("android"))
					{
						trimmedID = trimmedID.Remove(0, 8);
						break;
					}
				}
				//put saved appId in AndroidManifest
				var sb = new StringBuilder();
				var manifestPath = path + "/../unityLibrary/src/main/AndroidManifest.xml";
				var metaData =
					$"<meta-data android:name=\"com.google.android.gms.ads.APPLICATION_ID\" android:value=\"{trimmedID}\"/>";
				using (var sr = new StreamReader(manifestPath)) {
					string line;
					do {
						line = sr.ReadLine();
						sb.AppendLine(line);
					} while (line != null && !line.ToLower().Contains("<application"));
					
					sb.Append(metaData);
					sb.AppendLine();
					sb.Append(sr.ReadToEnd());
				}
				using (var sr = new StreamWriter(manifestPath)) {
					sr.Write(sb.ToString());
				}
			#endif
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE || NIMBUS_ENABLE_LIVERAMP || NIMBUS_ENABLE_MOLOCO
				var builder = new StringBuilder();
				builder.AppendLine("");
				builder.AppendLine("dependencies {");
				#if NIMBUS_ENABLE_LIVERAMP
					builder.AppendLine(AndroidBuildDependencies.LiveRampBuildDependencies());
				#endif
				#if NIMBUS_ENABLE_APS
					builder.AppendLine(AndroidBuildDependencies.APSBuildDependencies());
				#endif
				#if NIMBUS_ENABLE_VUNGLE
					builder.AppendLine(AndroidBuildDependencies.VungleBuildDependencies());
				#endif
				#if NIMBUS_ENABLE_META
					builder.AppendLine(AndroidBuildDependencies.MetaBuildDependencies());
				#endif
				#if NIMBUS_ENABLE_ADMOB
					builder.AppendLine(AndroidBuildDependencies.AdMobNimbusBuildDependency());
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL
					builder.AppendLine(AndroidBuildDependencies.MintegralBuildDependency());
				#endif
				#if NIMBUS_ENABLE_UNITY_ADS
					builder.AppendLine(AndroidBuildDependencies.UnityAdsBuildDependency());
				#endif
				#if NIMBUS_ENABLE_MOBILEFUSE
					builder.AppendLine(AndroidBuildDependencies.MobileFuseBuildDependency());
				#endif
				#if NIMBUS_ENABLE_MOLOCO
					builder.AppendLine(AndroidBuildDependencies.MolocoBuildDependency());
				#endif
				builder.AppendLine("}");
				var apsBuildWriter = File.AppendText(path + "/build.gradle");
				apsBuildWriter.WriteLine(builder.ToString());
				apsBuildWriter.Flush();
				apsBuildWriter.Close();
			#endif
		}
		private static void WriteGradleProps(string gradleFile) {
			var propWriter = File.AppendText(gradleFile);
			propWriter.WriteLine(@"
				android.useAndroidX=true
				");
			propWriter.Flush();
			propWriter.Close();
		}

		private static void RunEdm4uCheck(string path)
		{
			if (!File.Exists("Packages/manifest.json"))
				return;
			if(!File.ReadAllText("Packages/manifest.json").Contains("com.google.external-dependency-manager"))
			{
				var dependencies = AndroidBuildDependencies.BuildDependencies();
				var buildWriter = File.AppendText(path + "/build.gradle");
				buildWriter.WriteLine(dependencies);
				buildWriter.Flush();
				buildWriter.Close();
			}
		}
	}
}
#endif