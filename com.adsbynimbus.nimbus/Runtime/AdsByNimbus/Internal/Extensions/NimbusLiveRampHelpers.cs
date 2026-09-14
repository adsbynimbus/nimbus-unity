using System;
using System.Runtime.InteropServices;
using AdsByNimbus.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace AdsByNimbus.Internal.Extensions
{
    #if NIMBUS_ENABLE_LIVERAMP
    internal class NimbusLiveRampHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern void _initializeLiveRamp(String placementId, String identifiers, String appId, bool isTestMode);

            [DllImport("__Internal")]
            private static extern void _clearLiveRamp();
        #endif

        internal static void InitializeLiveRamp(String placementId, 
            LiveRamp.Identifier[] identifiers, String appId)
        {
            var isTestMode = NimbusManager.Instance._configuration.enableSDKInTestMode;
            #if UNITY_IOS
                _initializeLiveRamp(placementId, JsonConvert.SerializeObject(identifiers), appId, isTestMode);
            #endif
            #if UNITY_ANDROID
                var internalHelper = new AndroidJavaObject("com.adsbynimbus.unity.nimbusunityinternal");
                var instance = internalHelper.GetStatic<AndroidJavaObject> ("INSTANCE");
                instance.CallStatic("initLiveRamp", placementId, appId, JsonConvert.SerializeObject(identifiers), isTestMode);
            #endif
        }

        internal static void Clear()
        {
            #if UNITY_IOS
                _clearLiveRamp();
            #endif
            #if UNITY_ANDROID
                var internalHelper = new AndroidJavaObject("com.adsbynimbus.unity.nimbusunityinternal");
                var instance = internalHelper.GetStatic<AndroidJavaObject> ("INSTANCE");
                instance.CallStatic("clearLiveRamp");
            #endif
        }
    }
    #endif
}
        
        