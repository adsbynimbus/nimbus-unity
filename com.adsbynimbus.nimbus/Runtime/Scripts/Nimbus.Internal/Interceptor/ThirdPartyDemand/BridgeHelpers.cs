using System;
using UnityEngine;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    public class BridgeHelpers
    {
        public static string GetStringFromJavaFuture(String className, String methodName, object[] methodParams, long timeout)
        {
            AndroidJNI.AttachCurrentThread();
            var timeUnit = new AndroidJavaClass("java.util.concurrent.TimeUnit");
            var timeUnitMillis = timeUnit.CallStatic<AndroidJavaObject>("valueOf", "MILLISECONDS");
            var unityHelper = new AndroidJavaClass(className);
            var future = unityHelper.CallStatic<AndroidJavaObject>(methodName, methodParams);
            return future.Call<String>("get", timeout, timeUnitMillis);
        }
    }

    public enum ThirdPartyDemandEnum: int
    {
        AdMob = 0,
        Aps = 1,
        InMobi = 2,
        Meta = 3,
        Mintegral = 4, 
        MobileFuse = 5,
        Moloco = 6,
        UnityAds = 7,
        Vungle = 8
    }
    
    public class ThirdPartyDemandObj
    {
        public ThirdPartyDemandEnum demandType;
        public String firstKey;
        public String secondKey;
        public Boolean testMode;
        public Boolean autoInit;

        public ThirdPartyDemandObj(ThirdPartyDemandEnum demandType, String firstKey = "", String secondKey = "", Boolean testMode = false, Boolean autoInit = false)
        {
            this.demandType = demandType;
            this.firstKey = firstKey;
            this.secondKey = secondKey;
            this.testMode = testMode;
            this.autoInit = autoInit;
        }
    }
}