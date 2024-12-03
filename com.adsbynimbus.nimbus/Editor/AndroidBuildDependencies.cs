namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = "2.23.0";

		public static string APSBuildDependencies() {
			return $@"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""";
		}
		
		public static string VungleBuildDependencies()
		{
			return $@"implementation ""com.adsbynimbus.android:extension-vungle:{SdkVersion}""";
		}
	}
}