using System;

namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_APS
    public class APSExtension
    {
        // Use this constructor in the Nimbus.initialize() block to setup the AdMob extension
        public APSExtension(string appKey)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidApsAppKey = appKey;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosApsAppKey = appKey;
#endif
        }
    }
    
    public class aps: DemandComponent
    {
        public apsAd[] apsAds;
        // Use this constructor in the request blocks in Nimbus.bannerAd(), Nimbus.fullscreenAd(), etc.
        public aps(apsAd[] ads)
        {
            this.apsAds = ads;
        }
    }

    #endif
    
        
    [Serializable]
    public class apsAd
    {
        public string slotId;
        public APSAdFormat AdFormat;
        
        public apsAd(string slotId, APSAdFormat adFormat)
        {
            this.slotId = slotId;
            this.AdFormat = adFormat;
        }
    }
    
    [Serializable]
    public enum APSAdFormat : byte {
        Display320X50 = 0,
        Display300X250 = 1,
        Display728X90 = 2,
        InterstitialDisplay = 3,
        InterstitialVideo = 4,
        RewardedVideo = 5
    }
}