using System;
using Nimbus.Internal.Extensions.APS;
using UnityEngine;

namespace Nimbus.Internal.Extensions
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
    
    public class Extensions
    {
        public Aps aps;
        public Admob adMob;
        public InMobi inMobi;
        public Meta meta;
        public Mintegral mintegral;
        public MobileFuse mobileFuse;
        public Moloco moloco;
        public UnityAds unityAds;
        public Vungle vungle;
    }
    public struct Aps {
        public String appKey;
        public ApsSlotData[] slotData;
    }
    public struct Admob
    {
        public String[] adUnitIds;
    }
    
    public struct InMobi
    {
        public String accountId;
    }
    
    public struct Meta
    {
        public String appId;
        public Boolean forceTestAd;
    }
    
    public struct Mintegral
    {
        public String appId;
        public String appKey;
    }
    
    public struct MobileFuse {
    }
    
    public struct Moloco
    {
        public String appKey;
    }
    
    public struct UnityAds
    {
        public String gameId;
    }
    
    public struct Vungle
    {
        public String appId;
    }
}