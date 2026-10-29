namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_VUNGLE
    public class VungleExtension
    {
        public VungleExtension(string appId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidVungleAppID = appId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosVungleAppID = appId;
#endif
        }
    }
    #endif
}