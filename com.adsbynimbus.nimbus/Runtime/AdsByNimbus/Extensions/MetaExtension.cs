namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_META
    public class MetaExtension
    {
        public MetaExtension(string appId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidMetaAppID = appId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosMetaAppID = appId;
#endif
        }
    }
    #endif
}