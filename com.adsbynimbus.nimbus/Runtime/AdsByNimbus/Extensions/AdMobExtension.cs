namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_ADMOB
    public class AdMobExtension
    {
        // Use this constructor in the Nimbus.initialize() block to setup the AdMob extension
        // This method adds the GADApplicationIdentifier to the Info.plist and Android manifest files
        public AdMobExtension(string appId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidAdMobAppID = appId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosAdMobAppID = appId;
#endif
        }
    }

    public class adMob: DemandComponent
    {
        public string adUnitId;
        // Use this constructor in the request blocks in Nimbus.bannerAd(), Nimbus.fullscreenAd(), etc.
        public adMob(string adUnitId)
        {
            this.adUnitId = adUnitId;
        }
    }
    #endif
}