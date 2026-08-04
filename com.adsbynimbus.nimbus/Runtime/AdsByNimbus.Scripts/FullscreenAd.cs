using AdsByNimbus.Scripts;
using Internal;

public class FullscreenAd: Ad
{
    public float BannerBidFloor;
    public float VideoBidFloor;
    
    public FullscreenAd(in AdEvents adEvents, string nimbusReportingPosition,
        NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, RequestModifiers? modifiers = null,
        float bannerBidFloor = 0f, float videoBidFloor = 0f) :
        base(AdType.Fullscreen, adEvents, nimbusReportingPosition, adPosition, modifiers)
    {
        BannerBidFloor = bannerBidFloor;
        VideoBidFloor = videoBidFloor;
    }
}