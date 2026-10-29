namespace AdsByNimbus.Extensions
{
    #if NIMBUS_ENABLE_INMOBI
    public class InMobiExtension
    {
        public InMobiExtension(string accountId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidInMobiAccountId = accountId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosInMobiAccountId = accountId;
#endif
        } 
    }
    #endif
}