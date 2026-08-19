using System.Collections.Generic;
using AdsByNimbus;
using AdsByNimbus.RTB;
public class FullscreenAd: Ad
{
    internal bool Interstitial;
    
    internal FullscreenAd(in AdEvents adEvents, string position, Format[] addFormats = null, 
        AdOrientation orientation = AdOrientation.deviceOrientation, float bidFloor = 0f, bool interstitial = true,
        List<RequestComponent> components = null, List<DemandComponent> demand = null) :
        base(AdType.Fullscreen, adEvents, position, addFormats, bidFloor, orientation, components, demand)
    {
        Interstitial = interstitial;
    }
}