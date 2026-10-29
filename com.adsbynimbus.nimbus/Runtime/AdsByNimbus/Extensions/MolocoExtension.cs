namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_MOLOCO
    public class MolocoExtension
    {
        public MolocoExtension(string appKey)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidMolocoAppKey = appKey;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosMolocoAppKey = appKey;
#endif
        }
    }
    #endif
}