using System;
using System.Runtime.InteropServices;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal
{
    public class IOS : NimbusAPI
    {

        #region Declare external C interface
        [DllImport("__Internal")]
        private static extern void _initializeSDKWithPublisher(string publisher,
            string apiKey,
            bool enableSDKInTestMode,
            bool enableUnityLogs);

        [DllImport("__Internal")]
        private static extern void _showBannerAd(string position, float bannerFloor);

        [DllImport("__Internal")]
        private static extern void _showInterstitialAd(string position, float bannerFloor, float videoFloor, double closeButtonDelay);

        [DllImport("__Internal")]
        private static extern void _showRewardedVideoAd(string position, float videoFloor, double closeButtonDelay);

        [DllImport("__Internal")]
        private static extern void _setGDPRConsentString(string consent);

        [DllImport("__Internal")]
        private static extern void _destroyAd();
        #endregion

        #region Wrapped methods and properties

        private readonly NimbusIOSAdManager _iOSAdManager;
        public IOS()
        {
            _iOSAdManager = NimbusIOSAdManager.Instance;
        }

        internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration)
        {
            logger.Log("Initializing IOS SDK");
            _initializeSDKWithPublisher(configuration.publisherKey,
                configuration.apiKey,
                configuration.enableSDKInTestMode,
                configuration.enableUnityLogs);
        }

        internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit)
        {
            _iOSAdManager.SetAdUnit(nimbusAdUnit);
            nimbusAdUnit.DestroyIOSAd += OnDestroyIOSAd;

            // iOS uses seconds
            var closeButtonDelaySeconds = nimbusAdUnit.CloseButtonDelayMillis / 1000;
            switch (nimbusAdUnit.AdType)
            {
                case AdUnityType.Banner:
                    _showBannerAd(nimbusAdUnit.Position, nimbusAdUnit.BidFloors.BannerFloor);
                    break;
                case AdUnityType.Interstitial:
                    closeButtonDelaySeconds = 5;
                    _showInterstitialAd(nimbusAdUnit.Position, nimbusAdUnit.BidFloors.BannerFloor, nimbusAdUnit.BidFloors.VideoFloor, closeButtonDelaySeconds);
                    break;
                case AdUnityType.Rewarded:
                    _showRewardedVideoAd(nimbusAdUnit.Position, nimbusAdUnit.BidFloors.VideoFloor, closeButtonDelaySeconds);
                    break;
                default:
                    throw new Exception("ad type not supported");
            }
            return nimbusAdUnit;
        }
        

        internal override void SetGDPRConsentString(string consent)
        {
            _setGDPRConsentString(consent);
        }

        #endregion

        private static void OnDestroyIOSAd()
        {
            _destroyAd();
        }
    }
}
