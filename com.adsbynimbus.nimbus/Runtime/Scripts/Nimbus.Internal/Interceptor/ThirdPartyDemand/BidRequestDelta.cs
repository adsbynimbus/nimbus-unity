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
            return bidRequest;
        }
        
        private static T Merge<T>(T target, T source)
        {
            var properties = target.GetType().GetProperties();

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