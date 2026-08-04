using System;
using Internal.Extensions.APS;
using JetBrains.Annotations;
using UnityEngine;

namespace Internal.Extensions
{
    public class BridgeHelpers
    {
#if UNITY_ANDROID
        public static string GetStringFromJavaFuture(String className, String methodName, object[] methodParams,
            long timeout)
        {
            AndroidJNI.AttachCurrentThread();
            var timeUnit = new AndroidJavaClass("java.util.concurrent.TimeUnit");
            var timeUnitMillis = timeUnit.CallStatic<AndroidJavaObject>("valueOf", "MILLISECONDS");
            var unityHelper = new AndroidJavaClass(className);
            var future = unityHelper.CallStatic<AndroidJavaObject>(methodName, methodParams);
            return future.Call<String>("get", timeout, timeUnitMillis);
        }
#endif
    }

#if UNITY_IOS
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
#endif
#if UNITY_ANDROID
    public class Extensions
    {
        public String apsAppKey;
        public ApsSlotData[] apsSlotData;
        public String[] adMobAdUnitIds;
        public String inMobiAccountId;
        public String metaAppId;
        public Boolean metaForceTestAd;
        public String mintegralAppId;
        public String mintegralAppKey;
        public String molocoAppKey;
        public String unityAdsGameId;
        public String vungleAppId;
        public String[] appPageCat;
        public String[] appSectionCat;
        public String userKeywords;
        public String viewabilityOmidPn;
        public String viewabilityOmidPv;
        public double latitude;
        public double longitude;
        public LocationType locationType;
        public int accuracy;
    }
#endif
}