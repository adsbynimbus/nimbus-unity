using System.Collections.Generic;
using AdsByNimbus;
public class RewardedAd: Ad
{

    public RewardedAd(in AdEvents adEvents, string position, AdOrientation orientation, float bidFloor = 0f, 
        List<RequestComponent> components = null, List<DemandComponent> demand = null) :
        base(AdType.Rewarded, adEvents, position, bidFloor: bidFloor, orientation:orientation, 
            components: components, demand: demand)
    {

    }
}