using System;
using AdsByNimbus;
using Internal;
using Internal.Extensions;

public class InlineAd: Ad
{
    public IabSupportedAdSizes BannerSize;
    public bool RespectSafeArea;
    public float BannerBidFloor;
    public int BannerRefreshIntervalInSeconds;

    public InlineAd(in AdEvents adEvents, string nimbusReportingPosition,
        NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, RequestModifiers? modifiers = null,
        IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner, bool respectSafeArea = false,
        int bannerRefreshIntervalInSeconds = 30, float bannerBidFloor = 0f): 
        base(AdType.Inline, adEvents, nimbusReportingPosition, adPosition, modifiers)
    {
        BannerSize = adSize;
        RespectSafeArea = respectSafeArea;
        BannerRefreshIntervalInSeconds = bannerRefreshIntervalInSeconds;
        BannerBidFloor = bannerBidFloor;
    }
}
public static class IabSupportedAdSizesExtension {
    public static Tuple<int, int> ToWidthAndHeight(this IabSupportedAdSizes isa) {
        switch (isa) {
            case IabSupportedAdSizes.Banner:
                return new Tuple<int, int>(320, 50);
            case IabSupportedAdSizes.FullScreenPortrait:
                return new Tuple<int, int>(320, 480);
            case IabSupportedAdSizes.FullScreenLandscape:
                return new Tuple<int, int>(480, 320);
            case IabSupportedAdSizes.HalfScreen:
                return new Tuple<int, int>(300, 600);
            case IabSupportedAdSizes.Letterbox:
                return new Tuple<int, int>(300, 250);
            case IabSupportedAdSizes.LeaderBoard:
                return new Tuple<int, int>(728, 90);
            default:
                return new Tuple<int, int>(0, 0);
        }
    }
}
public enum NimbusAdUnitPosition
{
    BOTTOM_CENTER = 0,
    TOP_CENTER = 1,
    CENTER = 2,
    BOTTOM_LEFT = 3,
    BOTTOM_RIGHT = 4,
    TOP_LEFT = 5,
    TOP_RIGHT = 6,
}

public enum IabSupportedAdSizes : byte {
    Banner,
    FullScreenPortrait,
    FullScreenLandscape,
    HalfScreen,
    Letterbox,
    LeaderBoard
}