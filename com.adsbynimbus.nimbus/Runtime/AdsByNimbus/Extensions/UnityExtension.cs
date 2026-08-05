namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_UNITY_ADS
    public class UnityExtension
    {
        public UnityExtension(string gameId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidUnityAdsGameID = gameId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosUnityAdsGameID = gameId;
#endif
        }
    }
    #endif
}