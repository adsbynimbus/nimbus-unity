using AdsByNimbus;
using AdsByNimbus.Internal;
using AdsByNimbus.Internal.Extensions;

public class RewardedAd: Ad
{
    public float VideoBidFloor;

    public RewardedAd(in AdEvents adEvents, string nimbusReportingPosition,
        NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, RequestModifiers? modifiers = null,
        float videoBidFloor = 0f) :
        base(AdType.Rewarded, adEvents, nimbusReportingPosition, adPosition, modifiers)
    {
        VideoBidFloor = videoBidFloor;
    }
}