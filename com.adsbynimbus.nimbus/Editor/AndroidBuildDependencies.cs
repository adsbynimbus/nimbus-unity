namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = "2.23.0";

		public static string APSBuildDependencies() {
			return @"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""";
		}
		
		public static string VungleBuildDependencies()
		{
			return @"implementation ""com.adsbynimbus.android:extension-vungle:{SdkVersion}""";
		}
		
		public static string MetaBuildDependencies()
		{
			return @"implementation ""com.adsbynimbus.android:extension-facebook:2.+""";
		}
		
		public static string AdMobNimbusBuildDependency()
		{
			return @"implementation ""com.adsbynimbus.android:extension-admob:2.+""";
		}

		public static string AdMobGoogleBuildDependency()
		{
			return @"implementation ""com.google.android.gms:play-services-ads:23.+""";
		}
	}
}