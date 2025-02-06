namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = "2.26.1";

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
			return $@"implementation (""com.adsbynimbus.android:extension-admob:{SdkVersion}"")" + "{exclude group: 'androidx.collection', module: 'collection'}";
		}
		
		//this is needed because of a weird error caused by the Unity Build System
		public static string AdMobCollectionFixBuildDependency()
		{
			return @"implementation ""androidx.collection:collection:1.4.5""";
		}
	}
}