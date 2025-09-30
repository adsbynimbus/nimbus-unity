using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    public class BidRequestDelta
    {
        public KeyValuePair<string, string> SimpleUserExt;
        public KeyValuePair<string, JObject> ComplexUserExt;
        public ImpExt ImpressionExtension;
    }

    public class BidRequestDeltaManager
    {
        public static BidRequest ApplyDeltas(BidRequestDelta[] deltas, BidRequest bidRequest)
        {
            var adUnitType = bidRequest.Imp[0].Ext.AdUnitType;
            foreach (var delta in deltas)
            {
                if (delta != null)
                {
                    if (delta.SimpleUserExt.Value != null)
                    {
                        bidRequest.User ??= new User();
                        bidRequest.User.Ext ??= new JObject();
                        bidRequest.User.Ext.Add(delta.SimpleUserExt.Key, delta.SimpleUserExt.Value);
                    }
                    if (delta.ComplexUserExt.Value != null)
                    {
                        bidRequest.User ??= new User();
                        bidRequest.User.Ext ??= new JObject();
                        bidRequest.User.Ext.Add(delta.ComplexUserExt.Key, delta.ComplexUserExt.Value);
                    }
                    if (delta.ImpressionExtension != null && !bidRequest.Imp.IsNullOrEmpty())
                    {
                        bidRequest.Imp[0].Ext ??= new ImpExt();
                        bidRequest.Imp[0].Ext = Merge(bidRequest.Imp[0].Ext, delta.ImpressionExtension);
                    }
                }
            }
            // This has to be reset here because it is an enum and so it is non-nullable so it gets replaced during
            // Merge() when it shouldn't
            bidRequest.Imp[0].Ext.AdUnitType = adUnitType;
            return bidRequest;
        }
        
        private static T Merge<T>(T target, T source)
        {
            var properties = source.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }

            return target;
        }
    }
}