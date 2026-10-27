using AdsByNimbus.Internal;

namespace AdsByNimbus.Editor.Dependencies
{
    public static class AndroidDependencyConstants
    {
        private const string NimbusExtensionPrefix = "com.adsbynimbus.android:extension-";
        public static string AdMobPackage =
            $"{NimbusExtensionPrefix}admob:{AndroidExtensionVersionConstants.AdMob}";
        public static string ApsPackage = $"{NimbusExtensionPrefix}aps:{AndroidExtensionVersionConstants.Aps}";
        public static string DigitalTurbinePackage = $"{NimbusExtensionPrefix}digitalturbine:{AndroidExtensionVersionConstants.DigitalTurbine}";
        public static string DisplayIOPackage = $"{NimbusExtensionPrefix}displayio:{AndroidExtensionVersionConstants.DisplayIO}";
        public static string DisplayIORepository = "https://maven.display.io/";
        public static string InMobiPackage = $"{NimbusExtensionPrefix}inmobi:{AndroidExtensionVersionConstants.InMobi}";
        public static string LiveRampPackage =
            $"{NimbusExtensionPrefix}liveramp:{AndroidExtensionVersionConstants.LiveRamp}";
        public static string MetaPackage = $"{NimbusExtensionPrefix}meta:{AndroidExtensionVersionConstants.Meta}";
        public static string MintegralPackage =
            $"{NimbusExtensionPrefix}mintegral:{AndroidExtensionVersionConstants.Mintegral}";
        public static string MintegralRepository =
            "https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea";
        public static string MobileFusePackage =
            $"{NimbusExtensionPrefix}mobilefuse:{AndroidExtensionVersionConstants.MobileFuse}";
        public static string MolocoPackage = $"{NimbusExtensionPrefix}moloco:{AndroidExtensionVersionConstants.Moloco}";
        public static string UnityAdsPackage =
            $"{NimbusExtensionPrefix}unity:{AndroidExtensionVersionConstants.UnityAds}";
        public static string VunglePackage = $"{NimbusExtensionPrefix}vungle:{AndroidExtensionVersionConstants.Vungle}";
    }

    //Pod name and SPM Names are identical
    public static class IOSDependencyConstants
    {
        public static string NimbusSpm = "https://github.com/adsbynimbus/nimbus-ios-";
        public static string AdMobPackage =
            $"{NimbusSpm}admob.git";
        public static string AdMobSpmName = "NimbusAdMobKit";
        public static string ApsPackage = $"{NimbusSpm}aps.git";
        public static string ApsSpmName = "NimbusAPSKit";
        public static string DigitalTurbinePackage = $"{NimbusSpm}dt.git";
        public static string DigitalTurbineSpmName = "NimbusDTKit";
        public static string DisplayIOPackage = $"{NimbusSpm}displayio.git";
        public static string DisplayIOSpmName = "NimbusDisplayIOKit";
        public static string InMobiPackage = $"{NimbusSpm}inmobi.git";
        public static string InMobiSpmName = "NimbusInMobiKit";
        public static string LiveRampPackage = $"{NimbusSpm}liveramp.git";
        public static string LiveRampSpmName = "NimbusLiveRampKit";
        public static string MetaPackage = $"{NimbusSpm}meta.git";
        public static string MetaSpmName = "NimbusMetaKit";
        public static string MintegralPackage = $"{NimbusSpm}mintegral.git";
        public static string MintegralSpmName = "NimbusMintegralKit";
        public static string MobileFusePackage = $"{NimbusSpm}mobilefuse.git";
        public static string MobileFuseSpmName = "NimbusMobileFuseKit";
        public static string MolocoPackage = $"{NimbusSpm}moloco.git";
        public static string MolocoSpmName = "NimbusMolocoKit";
        public static string UnityAdsPackage = $"{NimbusSpm}unity.git";
        public static string UnityAdsSpmName = "NimbusUnityKit";
        public static string VunglePackage = $"{NimbusSpm}vungle.git";
        public static string VungleSpmName = "NimbusVungleKit";
    }
}