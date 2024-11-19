using System.Text;

namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		private const string SdkVersion = "2.23.0";

		public static string APSBuildDependencies() {
			var builder = new StringBuilder();
			
			builder.AppendLine("");
			builder.AppendLine("dependencies {");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""");
			builder.AppendLine("}");
			return builder.ToString();
		}
		
		public static string VungleBuildDependencies() {
			var builder = new StringBuilder();
			
			builder.AppendLine("");
			builder.AppendLine("dependencies {");
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:extension-vungle:{SdkVersion}""");
			builder.AppendLine("}");
			return builder.ToString();
		}
	}
}