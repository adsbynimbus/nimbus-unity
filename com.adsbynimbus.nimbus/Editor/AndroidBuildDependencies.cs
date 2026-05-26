using System.Text;

namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		
		//used if EDM4U is not implemented
		public static string BuildDependencies()
		{
			var builder = new StringBuilder();
			builder.AppendLine("");
			builder.AppendLine("dependencies {");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:nimbus:{VersionConstants.AndroidSdkVersion}""");
			builder.AppendLine($@"implementation ""io.github.pdvrieze.xmlutil:serialization:0.90.3""");
			builder.AppendLine("}");
			return builder.ToString();
		}

		public static string ApsBuildDependencies() {
			return $@"implementation ""com.adsbynimbus.android:extension-aps:{AndroidExtensionVersionConstants.Aps}""";
		}
		
		public static string VungleBuildDependencies()
		{
			return $@"implementation ""com.adsbynimbus.android:extension-vungle:{AndroidExtensionVersionConstants.Vungle}""";
		}
		
		public static string MetaBuildDependencies()
		{
			return $@"implementation ""com.adsbynimbus.android:extension-facebook:{AndroidExtensionVersionConstants.Meta}""
					  implementation ""com.facebook.android:audience-network-sdk:6.20.0""";
		}
		
		public static string AdMobNimbusBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-admob:{AndroidExtensionVersionConstants.AdMob}"")";
		}

		public static string MintegralBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-mintegral:{AndroidExtensionVersionConstants.Mintegral}"")";
		}

		public static string UnityAdsBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-unity:{AndroidExtensionVersionConstants.UnityAds}"")";
		}

		public static string MobileFuseBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-mobilefuse:{AndroidExtensionVersionConstants.MobileFuse}"")";
		}

		public static string LiveRampBuildDependencies()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-liveramp:{AndroidExtensionVersionConstants.LiveRamp}"")";
		}
		
		public static string MolocoBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-moloco:{AndroidExtensionVersionConstants.Moloco}"")";
		}
		
		public static string InMobiBuildDependency()
		{
			return $@"implementation (""com.adsbynimbus.android:extension-inmobi:{AndroidExtensionVersionConstants.InMobi}"")";
		}
	}
}