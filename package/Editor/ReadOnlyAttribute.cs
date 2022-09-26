#if UNITY_EDITOR
using UnityEngine;

namespace Nimbus.Editor {
	/// <summary>
	/// Read Only attribute.
	/// Attribute is use only to mark ReadOnly properties.
	/// </summary>
	public class ReadOnlyAttribute : PropertyAttribute { }
}
#endif