using System;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Nimbus.Tests
{
    public class VersionTests
    {
        // Tests to make sure the version number in package.json is the same as VersionConstants.cs
        [Test]
        public void TestUnityVersion()
        {
            using (StreamReader r = new StreamReader(@"../com.adsbynimbus.nimbus/package.json"))
            {
                string json = r.ReadToEnd();
                var packageObject = JsonConvert.DeserializeObject<JObject>(json);
                var packageVersion = packageObject["version"].ToString();
                Assert.AreEqual(packageVersion, VersionConstants.UnitySdkVersion);
            }
        }
        
        // Tests to make sure the version number in NimbusSDKDependencies.xml is the same as VersionConstants.cs
        [Test]
        public void TestIOSVersion()
        {
            var xmlDoc= new XmlDocument(); 
            xmlDoc.Load("../com.adsbynimbus.nimbus/Editor/NimbusSDKDependencies.xml"); 
            
            var nodes = xmlDoc.SelectNodes("dependencies/iosPods/iosPod");
            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    if (nodes[0]!= null)
                    {
                        Assert.AreEqual(nodes[0].Attributes["version"].Value, VersionConstants.IosSdkVersion);
                    }
                }
            }
        }
        
        // Tests to make sure the version number in NimbusSDKDependencies.xml is the same as VersionConstants.cs
        [Test]
        public void TestAndroidVersion()
        {
            var xmlDoc= new XmlDocument(); 
            xmlDoc.Load("../com.adsbynimbus.nimbus/Editor/NimbusSDKDependencies.xml"); 
            var nodes = xmlDoc.SelectNodes("dependencies/androidPackages/androidPackage");
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node != null)
                    {
                        if (node.Attributes["spec"] != null)
                        {
                            var androidPackage = node.Attributes["spec"].Value;
                            if (androidPackage.Contains("com.adsbynimbus.nimbus"))
                            {
                                var androidVersion = androidPackage.Substring(androidPackage.LastIndexOf(":") + 1);
                                Assert.AreEqual(androidVersion, VersionConstants.AndroidSdkVersion);
                            }
                        }
                    }
                }
            }
        }
    }
}
