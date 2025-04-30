using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.Session
{
    public class NimbusSessionHelpers
    {
        #if UNITY_IOS
            [DllImport("__Internal")]
            private static extern string _getSessionInfo();
        #endif

        public static BidRequest addSessionToRequest(BidRequest bidRequest)
        {
            var sessionInfo = "";
            #if UNITY_IOS
                sessionInfo = _getSessionInfo();
            #endif
            #if UNITY_ANDROID
                try 
                {
                    var helperClass = new AndroidJavaClass("com.adsbynimbus.unity.UnityHelper");
                    sessionInfo = helperClass.CallStatic<string>("getSessionInfo");
                }
                catch (Exception e)
                {
                    Debug.unityLogger.Log("LiveRamp Request Info ERROR", e.Message);
                }
            #endif
            if (sessionInfo.IsNullOrEmpty())
            {
                return bidRequest;
            }
            var sessionObject = JsonConvert.DeserializeObject(sessionInfo, typeof(JObject)) as JObject;
            bidRequest.CustomSignals = sessionObject;
            return bidRequest;
        }
    }
}
        
        