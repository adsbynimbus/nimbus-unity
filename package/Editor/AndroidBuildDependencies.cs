using System.Text;
using Nimbus.Runtime.Scripts;
using UnityEngine;

namespace Nimbus.Editor {
	public static class AndroidBuildDependencies {
		// TODO point to a production build before merging
		private const string SdkVersion = "2.1.0-dev";

		public static string BuildDependencies() {
			var builder = new StringBuilder();

			builder.AppendLine("dependencies {");
			
			builder.AppendLine($@"implementation ""com.adsbynimbus.android:nimbus:{SdkVersion}""");
			var nimbusManager = Object.FindObjectOfType<NimbusManager>();
			if (nimbusManager == null) {
				builder.AppendLine("}");
				return builder.ToString();
			}
			
			var config = nimbusManager.GetNimbusConfiguration();
			if (!config) {
				builder.AppendLine("}");
				return builder.ToString();
			}
			
			if (config.enableAPS) {
				builder.AppendLine($@"implementation ""com.adsbynimbus.android:extension-aps:{SdkVersion}""");
			}
			
			builder.AppendLine("}");
			return builder.ToString();
		}
	}
}