using System.Collections.Generic;

namespace AdsByNimbus.Internal.Utility {
	public static class ArrayExtensions {
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection) {
			return collection == null || collection.Count == 0;
		}
	}
}