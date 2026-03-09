using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal
{
    public class NimbusPrivacyHelpers
    {
        
        public static string TcfUserConsentString;
        
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern string _getPrivacyStrings();
        #endif
        
        public static Regs getPrivacyRegulations(Regs prevRegs)
        {
            TcfUserConsentString = "";
            Regs regulations = prevRegs;
            regulations ??= new Regs();
            var privacyStrings = "";
            #if UNITY_IOS
                privacyStrings = _getPrivacyStrings();
            #endif
            #if UNITY_ANDROID
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			    var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var helperClass = new AndroidJavaClass("com.adsbynimbus.unity.UnityHelper");
                privacyStrings =  helperClass.CallStatic<String>("getPrivacyStrings", currentActivity);
            #endif
            Debug.unityLogger.Log("Nimbus privacy", privacyStrings);
            if (privacyStrings.IsNullOrEmpty() || privacyStrings == "{}") {
                return null;
            }
            var privacyObject = JsonConvert.DeserializeObject(privacyStrings, typeof(JObject)) as JObject;
            if (privacyObject == null)
            {
                return null;
            }
            if (privacyObject.ContainsKey("gdprApplies"))
            {
                regulations.Ext ??= new RegExt();
                regulations.Ext.Gdpr = privacyObject["gdprApplies"].ToObject<Int16>();
            }
            if (privacyObject.ContainsKey("usPrivacyString"))
            {
                if (!privacyObject["usPrivacyString"].ToObject<String>().IsNullOrEmpty())
                {
                    regulations.Ext ??= new RegExt();
                    regulations.Ext.UsPrivacy = privacyObject["usPrivacyString"].ToObject<String>();
                }
            }
            if (privacyObject.ContainsKey("gppConsentString"))
            {
                if (!privacyObject["gppConsentString"].ToObject<String>().IsNullOrEmpty())
                {
                    regulations.Ext ??= new RegExt();
                    regulations.Ext.GPP = privacyObject["gppConsentString"].ToObject<String>();
                }
            }
            if (privacyObject.ContainsKey("gppSectionId"))
            {
                if (!privacyObject["gppSectionId"].ToObject<String>().IsNullOrEmpty())
                {
                    regulations.Ext ??= new RegExt();
                    regulations.Ext.GPPSIDs = privacyObject["gppSectionId"].ToObject<String>();
                }
            }

            if (privacyObject.ContainsKey("tcfPrivacyString"))
            {
                if (!privacyObject["tcfPrivacyString"].ToObject<String>().IsNullOrEmpty())
                {
                    TcfUserConsentString = privacyObject["tcfPrivacyString"].ToObject<String>();
                }
            }
            if (regulations.Ext == null)
            {
                return null;
            }
            return regulations;
        }
    }
}