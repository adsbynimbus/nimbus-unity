namespace AdsByNimbus.Extensions
{
#if NIMBUS_ENABLE_DISPLAY_IO
    public class DisplayIOExtension
    {
        public DisplayIOExtension(string appId, string userId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidDisplayIOAppId = appId;
            NimbusManager.Instance._configuration.androidDisplayIOUserId = userId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosDisplayIOAppId = appId;
            NimbusManager.Instance._configuration.iosDisplayIOUserId = userId;
#endif
        } 
    }
#endif
}