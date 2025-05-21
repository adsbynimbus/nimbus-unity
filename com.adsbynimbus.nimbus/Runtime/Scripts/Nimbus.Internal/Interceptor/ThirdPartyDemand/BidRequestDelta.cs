using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand
{
    public class BidRequestDelta
    {
        public KeyValuePair<string, string> simpleUserExt;
        public KeyValuePair<string, JObject> complexUserExt;
        public ImpExt impressionExtension;
    }

    public class BidRequestDeltaManager
    {
        public static BidRequest ApplyDeltas(BidRequestDelta[] deltas, BidRequest bidRequest)
        {
            foreach (var delta in deltas)
            {
                if (delta != null)
                {
                    if (delta.simpleUserExt.Value != null)
                    {
                        bidRequest.User ??= new User();
                        bidRequest.User.Ext ??= new JObject();
                        bidRequest.User.Ext.Add(delta.simpleUserExt.Key, delta.simpleUserExt.Value);
                    }
                    if (delta.complexUserExt.Value != null)
                    {
                        bidRequest.User ??= new User();
                        bidRequest.User.Ext ??= new JObject();
                        bidRequest.User.Ext.Add(delta.complexUserExt.Key, delta.complexUserExt.Value);
                    }
                    if (delta.impressionExtension != null && !bidRequest.Imp.IsNullOrEmpty())
                    {
                        bidRequest.Imp[0].Ext ??= new ImpExt();
                        bidRequest.Imp[0].Ext = Merge(bidRequest.Imp[0].Ext, delta.impressionExtension);
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