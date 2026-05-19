using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using UnityEngine;

namespace Nimbus.Internal.Extensions
{
    #if NIMBUS_ENABLE_LIVERAMP
    public class NimbusLiveRampHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void _initializeLiveRamp(String configId, String email, 
                Boolean hasConsentForNoLegislation, Boolean isTestMode);
        
        #endif

        public static void initializeLiveRamp(String configId, 
            String email = "", Boolean hasConsentForNoLegislation = false,  Boolean isTestMode = false)
        {
            #if UNITY_IOS
                _initializeLiveRamp(configId, email, hasConsentForNoLegislation, isTestMode);
            #endif
            #if UNITY_ANDROID
                var liveRamp = new AndroidJavaClass("com.adsbynimbus.request.LiveRampExtension");
                if (isTestMode)
                {
                    liveRamp.CallStatic("initializeTestMode", configId, email);
                }
                else
                {
                    liveRamp.CallStatic("initialize", configId, email, hasConsentForNoLegislation);
                }
            #endif
        }
    }
    #endif
}
        
        