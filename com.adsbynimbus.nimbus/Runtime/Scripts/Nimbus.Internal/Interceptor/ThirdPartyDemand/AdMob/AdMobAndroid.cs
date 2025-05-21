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

      private readonly string _appID;
      private readonly bool _enableTestMode;
      private readonly bool _testMode;
      private readonly ThirdPartyAdUnit[] _adUnitIds;
      private AdUnitType _adUnitType;
      private string _adUnitId;
      private readonly AndroidJavaObject _applicationContext;


      public AdMobAndroid(AndroidJavaObject applicationContext, string appID, ThirdPartyAdUnit[] adUnitIds,
         bool enableTestMode)
      {
         _applicationContext = applicationContext;
         _appID = appID;
         _adUnitIds = adUnitIds;
         _enableTestMode = enableTestMode;
      }

      public void InitializeNativeSDK()
      {
         //do nothing
      }

      internal string GetAdUnitId(AdUnitType type, bool isFullScreen)
      {
         foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
         {
            if (adUnit.AdUnitType == type)
            {
               _adUnitId = adUnit.AdUnitId;
               _adUnitType = type;
               return adUnit.AdUnitId;
            }
         }

         return "";
      }

      internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data)
      {
         var bidRequestDelta = new BidRequestDelta();
         if (data.IsNullOrEmpty())
         {
            return bidRequestDelta;
         }

         // data is the adUnitId
         try
         {
            var adMobSignal = "";
            var args = new object[] { data };
            switch (_adUnitType)
            {
               case AdUnitType.Banner:
               case AdUnitType.Undefined:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobBannerSignal", args, 500L);
                  break;
               case AdUnitType.Interstitial:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobInterstitialSignal", args, 500L);
                  break;
               case AdUnitType.Rewarded:
                  adMobSignal = BridgeHelpers.GetStringFromJavaFuture(
                     NimbusAdMobPackage,
                     "fetchAdMobRewardedSignal", args, 500L);
                  break;
            }
            bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("admob_gde_signals", adMobSignal);
         }
         catch (Exception e)
         {
            Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
         }

         return bidRequestDelta;
      }
      
      public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
      {
         return Task<BidRequestDelta>.Run(() =>
         {
            try
            {
               return ModifyRequest(bidRequest, GetAdUnitId(type, isFullScreen));
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