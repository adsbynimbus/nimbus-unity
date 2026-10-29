namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_MINTEGRAL
    public class MintegralExtension
    {
        public MintegralExtension(string appId, string appKey)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidMintegralAppID = appId;
            NimbusManager.Instance._configuration.androidMintegralAppKey = appKey;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosMintegralAppID = appId;
            NimbusManager.Instance._configuration.iosMintegralAppKey = appKey;
#endif
        }
    }
    #endif
}