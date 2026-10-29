using System;
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
    
        
    public new RewardedAd onEvent(Action<AdEvent> onEvent)
    {
        base.onEvent(onEvent);
        return this;
    }

    public new RewardedAd onError(Action<NimbusError> onError)
    {
        base.onError(onError);
        return this;
    }

    public new RewardedAd load()
    {
        base.load();
        return this;
    }

    public new RewardedAd show()
    {
        base.show();
        return this;
    }
}