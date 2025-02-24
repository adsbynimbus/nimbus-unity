using System.Text;
using DefaultNamespace;

namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = VersionConstants.AndroidSdkVersion;

		//used if EDM4U is not implemented
		public static string BuildDependencies()
		{
			var builder = new StringBuilder();
			builder.AppendLine("");
			builder.AppendLine("dependencies {");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:nimbus:{SdkVersion}""");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:nimbus-vast:{SdkVersion}""");
			builder.AppendLine($@"implementation ""io.github.pdvrieze.xmlutil:serialization:0.90.3""");
			builder.AppendLine("}");
			return builder.ToString();

		}

		public static string APSBuildDependencies() {
			return $@"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""";
		}
		
		public static string VungleBuildDependencies()
		{
			return $@"implementation ""com.adsbynimbus.android:extension-vungle:{SdkVersion}""";
		}
		
		public static string MetaBuildDependencies()
		{
			return $@"implementation ""com.adsbynimbus.android:extension-facebook:{SdkVersion}""";
		}
		
		public static string AdMobNimbusBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-admob:{SdkVersion}"")";
		}
		
		//this is needed because of a weird error caused by the Unity Build System
		public static string AdMobCollectionFixBuildDependency()
		{
			return @"implementation ""androidx.collection:collection:1.4.5""";
		}
	}
}