using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.LiveRamp
{
    public class NimbusLiveRampHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void _initializeLiveRamp(String configId, String email, 
                String phoneNumber, Boolean isTestMode, 
                Boolean hasConsentForNoLegislation);

            [DllImport("__Internal")]
            private static extern string _getLiveRampData();
        #endif

        public static void initializeLiveRamp(String configId, Boolean isTestMode, 
            Boolean hasConsentForNoLegislation, String email = "", 
            String phoneNumber = "")
        {
            #if UNITY_IOS
                _initializeLiveRamp(configId, email, phoneNumber, isTestMode, hasConsentForNoLegislation);
            #endif
            #if UNITY_ANDROID
            #endif
        }

        public static BidRequest getLiveRampData(BidRequest bidRequest)
        {
            var liveRampData = "";
            #if UNITY_IOS
               liveRampData = _getLiveRampData();
               var liveRampObject = JsonConvert.DeserializeObject(liveRampData, typeof(JObject)) as JObject;
            #endif
            
            return bidRequest;
        }
    }
}
        
        