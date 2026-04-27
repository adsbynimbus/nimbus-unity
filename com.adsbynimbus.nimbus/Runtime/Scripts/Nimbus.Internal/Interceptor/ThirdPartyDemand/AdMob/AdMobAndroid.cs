using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
   internal class AdMobAndroid : IInterceptor, IProvider
   {
      private const string NimbusAdMobPackage = "com.adsbynimbus.request.internal.NimbusRequestsAdMobInternal";

      private readonly bool _testMode;
      private readonly ThirdPartyAdUnit[] _adUnitIds;
      private AdType _adUnitType;


      public AdMobAndroid(ThirdPartyAdUnit[] adUnitIds)
      {
         _adUnitIds = adUnitIds;
      }
      

      public void InitializeNativeSDK()
      {
         //do nothing
      }

      private string GetAdUnitId(AdType type)
      {
         foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
         {
            if (adUnit.AdUnitType == type)
            {
               _adUnitType = type;
               return adUnit.AdUnitId;
            }
         }

         return "";
      }

      internal String GetProviderRtbDataFromNativeSDK(BidRequest bidRequest, AdType type)
      {
         try
         {
            var adMobSignal = "";
            var args = new object[] { GetAdUnitId(type) };
            switch (_adUnitType)
            {
               case AdType.Banner:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobBannerSignal", args, 3000L);
                  break;
               case AdType.Interstitial:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobInterstitialSignal", args, 3000L);
                  break;
               case AdType.Rewarded:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobRewardedSignal", args, 3000L);
                  break;
            }
            return adMobSignal;
         }
         catch (Exception e)
         {
            Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
         }

         return "";
      }
      
      internal BidRequestDelta GetBidRequestDelta(string data)
      {
         return data.IsNullOrEmpty() ? new BidRequestDelta() : new BidRequestDelta()
         {
            SimpleUserExt = new KeyValuePair<string, string>("admob_gde_signals", data)
         };
      }
      
      public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdType type, bool isFullScreen, BidRequest bidRequest)
      {
         return Task<BidRequestDelta>.Run(() =>
         {
            try
            {
               return GetBidRequestDelta(GetProviderRtbDataFromNativeSDK(bidRequest, type));
            }
            catch (Exception e)
            {
               Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
               return null;
            }
         });
      }
   }

}