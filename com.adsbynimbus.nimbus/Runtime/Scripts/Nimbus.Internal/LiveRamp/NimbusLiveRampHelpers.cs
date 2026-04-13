using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.LiveRamp
{
    #if NIMBUS_ENABLE_LIVERAMP
    public class NimbusLiveRampHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void _initializeLiveRamp(String configId, String email, 
                String phoneNumber, Boolean isTestMode, Boolean hasConsentForNoLegislation);
        
        #endif

        public static void initializeLiveRamp(String configId, 
            Boolean hasConsentForNoLegislation, Boolean isTestMode, String email = "", 
            String phoneNumber = "")
        {
            #if UNITY_IOS
                _initializeLiveRamp(configId, email, phoneNumber, isTestMode, hasConsentForNoLegislation);
            #endif
            #if UNITY_ANDROID
                var liveRamp = new AndroidJavaClass("com.adsbynimbus.request.LiveRampExtension");
                if (isTestMode)
                {
                    liveRamp.CallStatic("initializeTestMode", configId, email);
                }
                else
                {
                    liveRamp.CallStatic("initialize", configId, email, phoneNumber, hasConsentForNoLegislation);
                }
            #endif
        }
    }
    #endif
}
        
        