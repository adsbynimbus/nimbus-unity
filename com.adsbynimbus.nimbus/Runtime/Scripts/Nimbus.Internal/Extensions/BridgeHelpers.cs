using System;
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
        public ApsExt aps;
        public AdMobExt adMob;
        public InMobiExt inMobi;
        public MetaExt meta;
        public MintegralExt mintegral;
        public MobileFuseExt mobileFuse;
        public MolocoExt moloco;
        public UnityAdsExt unityAds;
        public VungleExt vungle;
    }
    public struct ApsExt
    {
        public String appKey;
    }
    
    public struct AdMobExt {
    }
    
    public struct InMobiExt
    {
        public String accountId;
    }
    
    public struct MetaExt
    {
        public String appId;
        public Boolean forceTestAd;
    }
    
    public struct MintegralExt
    {
        public String appId;
        public String appKey;
    }
    
    public struct MobileFuseExt {
    }
    
    public struct MolocoExt
    {
        public String appKey;
    }
    
    public struct UnityAdsExt
    {
        public String gameId;
    }
    
    public struct VungleExt
    {
        public String appId;
    }
}