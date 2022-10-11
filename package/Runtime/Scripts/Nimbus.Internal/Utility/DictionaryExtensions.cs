using System.Collections.Generic;

namespace Nimbus.Internal.Utility {
	public static class DictionaryExtensions {
		public static T Get<T>(this Dictionary<string, object> instance, string name) {
			return (T)instance[name];
		}
	}
}