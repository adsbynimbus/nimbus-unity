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
            int logLevel,
            string appName,
            string appDomain,
            string bundleId,
            string storeUrl,
            bool showMuteButton);

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

        private readonly IOSAdManager iOSAdManager;

        public IOS()
        {
            iOSAdManager = IOSAdManager.Instance;
        }

        internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration)
        {
            _initializeSDKWithPublisher(configuration.publisherKey,
                configuration.apiKey,
                true, // TODO: enableSDKInTestMode is enabled
                2, // TODO: logLevel is hardcoded to DEBUG
                configuration.appName,
                configuration.appDomain,
                configuration.iosBundleID,
                configuration.iosAppStoreURL,
                true); // TODO: showMuteButton is hardcoded to true
        }

        internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit)
        {
            iOSAdManager.SetAdUnit(nimbusAdUnit);
            nimbusAdUnit.DestroyAd += this.OnDestroyAd; // TODO: do we need to remove this at some point?

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

        public void OnDestroyAd()
        {
            _destroyAd();
        }
    }
}
