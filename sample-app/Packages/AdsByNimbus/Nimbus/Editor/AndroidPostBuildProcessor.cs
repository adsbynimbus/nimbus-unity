#if UNITY_EDITOR
using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace Nimbus.Editor {
	public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject {
		public int callbackOrder {
			get { return 999; }
		}

		public void OnPostGenerateGradleAndroidProject(string path) {
			Debug.Log("Bulid path : " + path);
			StreamWriter writer = File.AppendText(path + "/gradle.properties");
			writer.WriteLine("android.useAndroidX=true");
			writer.WriteLine("android.enableJetifier=true");
			writer.Flush();
			writer.Close();
		}
	}
}
#endif