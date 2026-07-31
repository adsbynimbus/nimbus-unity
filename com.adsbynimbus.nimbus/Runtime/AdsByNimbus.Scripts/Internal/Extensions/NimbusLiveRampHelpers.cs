using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Internal.Extensions
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
                var internalHelper = new AndroidJavaObject("com.adsbynimbus.unity.nimbusunityinternal");
                var instance = internalHelper.GetStatic<AndroidJavaObject> ("INSTANCE");
                instance.CallStatic("initLiveRamp", configId, email, hasConsentForNoLegislation, isTestMode);
            #endif
        }
    }
    #endif
}
        
        