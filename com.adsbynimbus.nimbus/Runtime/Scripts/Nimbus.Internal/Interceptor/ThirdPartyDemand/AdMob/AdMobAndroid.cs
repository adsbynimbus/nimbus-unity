using System;
using System.Runtime.CompilerServices;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
    internal class AdMobAndroid : IInterceptor, IProvider {
       private const string NimbusAdMobPackage = "com.adsbynimbus.request.internal.NimbusRequestsAdMobInternal";

       private readonly string _appID;
       private readonly bool _enableTestMode;
       private readonly bool _testMode;
       private readonly ThirdPartyAdUnit[] _adUnitIds;
       private AdUnitType _adUnitType;
       private string _adUnitId;
       private readonly AndroidJavaObject _applicationContext;
       
       
       public AdMobAndroid(AndroidJavaObject applicationContext, string appID, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
          _applicationContext = applicationContext;
          _appID = appID;
          _adUnitIds = adUnitIds;
          _enableTestMode = enableTestMode;
       }
       
       public void InitializeNativeSDK() {
          //do nothing
       }
       
       public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
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
       
       public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
          if (data.IsNullOrEmpty()) {
             return bidRequest;
          }
          // data is the adUnitId
          try
          {
             var adMobSignal = "";
             var args = new object[] {data};
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
             bidRequest.User.Ext.AdMobSignals = adMobSignal;
          }
          catch (Exception e)
          {
             Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
          }
          return bidRequest;
       }
    }
    
}