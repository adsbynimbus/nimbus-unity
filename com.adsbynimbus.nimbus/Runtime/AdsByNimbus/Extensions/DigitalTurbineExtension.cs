namespace AdsByNimbus.Extensions
{
#if NIMBUS_ENABLE_DIGITAL_TURBINE
    public class DigitalTurbineExtension
    {
        public DigitalTurbineExtension(string appId)
        {
#if UNITY_ANDROID
            NimbusManager.Instance._configuration.androidDigitalTurbineAppId = appId;
#elif UNITY_IOS
            NimbusManager.Instance._configuration.iosDigitalTurbineAppId = appId;
#endif
        } 
    }
#endif
}