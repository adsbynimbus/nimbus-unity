using System.Text;

namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = "2.18.0";

		public static string BuildDependencies() {
			var builder = new StringBuilder();
			
			builder.AppendLine("");
			builder.AppendLine("dependencies {");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:nimbus:{SdkVersion}""");
			
			#if NIMBUS_ENABLE_APS
				builder.AppendLine($@"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""");
			#endif
			
			builder.AppendLine("}");
			return builder.ToString();
		}
	}
}