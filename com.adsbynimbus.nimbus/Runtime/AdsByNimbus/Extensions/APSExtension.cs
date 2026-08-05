namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_APS
    public class APSExtension
    {
        public APSExtension(string appKey)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidApsAppKey = appKey;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosApsAppKey = appKey;
#endif
        }
    }
    #endif
}