using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nimbus.Editor {
	public class PlistDictionary : Dictionary<string, object> {
		private PlistDictionary() { }

		public PlistDictionary(string xmlData) {
			Parse(xmlData);
		}

		private void Parse(string xmlData) {
			Clear();
			var doc = XDocument.Parse(xmlData);
			var plist = doc.Element("plist");
			var dict = plist?.Element("dict");
			if (dict == null) return;
			var dictElements = dict.Elements();
			Parse(this, dictElements);
		}

		private void Parse(PlistDictionary dict, IEnumerable<XElement> elements) {
			var xElements = elements as XElement[] ?? elements.ToArray();
			for (var i = 0; i < xElements.Count(); i += 2) {
				var key = xElements.ElementAt(i);
				var val = xElements.ElementAt(i + 1);
				dict[key.Value] = ParseValue(val);
			}
		}

		private List<object> ParseArray(IEnumerable<XElement> elements) {
			var list = new List<object>();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var e in elements) {
				var one = ParseValue(e);
				list.Add(one);
			}

			return list;
		}

		private object ParseValue(XElement val) {
			switch (val.Name.ToString()) {
				case "string":
					return val.Value;
				case "integer":
					return int.Parse(val.Value);
				case "real":
					return float.Parse(val.Value);
				case "true":
					return true;
				case "false":
					return false;
				case "dict":
					var plist = new PlistDictionary();
					Parse(plist, val.Elements());
					return plist;
				case "array":
					var list = ParseArray(val.Elements());
					return list;
				default:
					throw new ArgumentException("Unsupported " + val.Value);
			}
		}
	}
}