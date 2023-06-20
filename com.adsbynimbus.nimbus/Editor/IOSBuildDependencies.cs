using System.Collections.Generic;
using System.Text;

namespace Nimbus.Editor {
	public class IOSBuildDependencies {
		private const string SdkVersion = "2.8.1";
		private readonly List<string> _dependencies = new List<string> {
			"'NimbusKit'",
			"'NimbusRenderVideoKit'",
			"'NimbusRenderStaticKit'",
		};
		
		public string BuildDependencies() {
			var builder = new StringBuilder();
			
			builder.AppendLine(@"platform :ios, '14.0'
use_frameworks!
source 'https://cdn.cocoapods.org/'
");
			if (SdkVersion.Contains("internal")) {
				builder.AppendLine("source 'git@github.com:timehop/Specs.git'");
			}
			
			#if NIMBUS_ENABLE_APS
				_dependencies.Add("'NimbusRequestAPSKit'");
			#endif

			var dependencies = string.Join<string>(", ", _dependencies);
			builder.AppendLine("def sdk_dependencies");
			builder.AppendLine($"  pod 'NimbusSDK', '{SdkVersion}', subspecs: [{dependencies}]");
			builder.AppendLine("end");


			builder.AppendLine(@"
target 'Unity-iPhone' do
  sdk_dependencies
  target 'Unity-iPhone Tests' do
    inherit! :search_paths
    # Pods for testing
  end
end
target 'UnityFramework' do
  sdk_dependencies
end
");
			return builder.ToString();
		}
	}
}