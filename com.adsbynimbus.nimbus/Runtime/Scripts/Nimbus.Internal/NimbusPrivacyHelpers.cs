using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;

namespace Nimbus.Internal
{
    public class NimbusPrivacyHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern string _getPrivacyStrings();
        #endif
        
        public static Regs getPrivacyRegulations()
        {
            Regs regulations = new Regs();
            #if UNITY_IOS
                var privacyStrings = _getPrivacyStrings();
                if (privacyStrings.IsNullOrEmpty()) {
                    return null;
                }
                var privacyObject = JsonConvert.DeserializeObject(privacyStrings, typeof(JObject)) as JObject;
                if (privacyObject == null)
                {
                    return null;
                }
                if (privacyObject.ContainsKey("gdprApplies"))
                {
                    regulations.Ext.Gdpr = privacyObject["gdprApplies"].ToObject<bool>() ? 1 : 0;
                }
                regulations.Ext.UsPrivacy = privacyObject["usPrivacyString"].ToObject<String>();
                regulations.Ext.GPP = privacyObject["gppConsentString"].ToObject<String>();
                regulations.Ext.GPPSIDs = privacyObject["gppSectionId"].ToObject<String>();
            #endif
            #if UNITY_ANDROID
                
            #endif
            return regulations;
        }
    }
}