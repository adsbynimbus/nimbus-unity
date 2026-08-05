using System;
using AdsByNimbus;
using AdsByNimbus.Internal;
using AdsByNimbus.Internal.Extensions;

public class InlineAd: Ad
{
    public AdSize BannerSize;
    public bool RespectSafeArea;
    public float BannerBidFloor;
    public int BannerRefreshIntervalInSeconds;

    public InlineAd(in AdEvents adEvents, string nimbusReportingPosition,
        NimbusUnityPosition adPosition = NimbusUnityPosition.BOTTOM_CENTER, RequestModifiers? modifiers = null,
        AdSize adSize = AdSize.banner, bool respectSafeArea = false,
        int bannerRefreshIntervalInSeconds = 30, float bannerBidFloor = 0f): 
        base(AdType.Inline, adEvents, nimbusReportingPosition, adPosition, modifiers)
    {
        BannerSize = adSize;
        RespectSafeArea = respectSafeArea;
        BannerRefreshIntervalInSeconds = bannerRefreshIntervalInSeconds;
        BannerBidFloor = bannerBidFloor;
    }
}
public static class AdSizesExtension {
    public static Tuple<int, int> ToWidthAndHeight(this AdSize isa) {
        switch (isa) {
            case AdSize.banner:
                return new Tuple<int, int>(320, 50);
            case AdSize.mrec:
                return new Tuple<int, int>(300, 250);
            case AdSize.halfScreen:
                return new Tuple<int, int>(300, 600);
            case AdSize.leaderboard:
                return new Tuple<int, int>(728, 90);
            case AdSize.interstitialPortrait:
                return new Tuple<int, int>(320, 480);
            case AdSize.interstitialLandscape:
                return new Tuple<int, int>(480, 320);
            default:
                return new Tuple<int, int>(0, 0);
        }
    }
}
public enum NimbusUnityPosition
{
    BOTTOM_CENTER = 0,
    TOP_CENTER = 1,
    CENTER = 2,
    BOTTOM_LEFT = 3,
    BOTTOM_RIGHT = 4,
    TOP_LEFT = 5,
    TOP_RIGHT = 6,
}

public enum AdSize : byte {
    // Standard banner format (320×50)
    banner,
    // Medium rectangle (MREC) format (300×250)
    mrec,
    // Half-screen format (300×600)
    halfScreen,
    // Leaderboard format (728×90)
    leaderboard,
    // Interstitial portrait format (320×480)
    interstitialPortrait,
    // Interstitial landscape format (480×320)
    interstitialLandscape,
}