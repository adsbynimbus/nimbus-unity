using System;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    [Serializable]
    public class ThirdPartyAdUnit
    {
        public string AdUnitId;
        public string AdUnitPlacementId;
        public AdUnitType AdUnitType;
    }
}