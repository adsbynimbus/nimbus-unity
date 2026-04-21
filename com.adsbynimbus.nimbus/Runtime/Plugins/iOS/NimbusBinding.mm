//
//  NimbusBinding.mm
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 AdsByNimbus. All rights reserved.
//

#import "UnityFramework/UnityFramework-Swift.h"

#pragma mark - Helpers

// Converts C style string to NSString
#define GetStringParam(_x_) ((_x_) != NULL ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""])
#define GetNullableStringParam(_x_) ((_x_) != NULL ? [NSString stringWithUTF8String:_x_] : nil)

#pragma mark - C interface

extern "C" {
    void _initializeSDKWithPublisher(const char* publisher,
                                     const char* apikey,
                                     bool enableUnityLogs,
                                     bool enableSDKInTestMode,
                                     const char* thirdPartyJson) {
        [NimbusManager initializeNimbusSDKWithPublisher: GetStringParam(publisher)
                                                 apiKey: GetStringParam(apikey)
                                        enableUnityLogs: enableUnityLogs
                                        enableSDKInTestMode: enableSDKInTestMode
                                        thirdPartyJson: GetStringParam(thirdPartyJson)];
    }

    void _bannerAd(int adUnitInstanceId, 
                   const char* position,
                   int width,
                   int height,                   
                   int refreshInterval,
                   bool respectSafeArea,
                   int bannerPosition,
                   bool showAd,
                   const char* apsAdUnitId,
                   const char* adMobAdUnitId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            bannerAdWithPosition:GetStringParam(position) width:width height:height refreshInterval:refreshInterval respectSafeArea:respectSafeArea
            bannerPosition:bannerPosition showAd: showAd apsAdUnitId:GetStringParam(apsAdUnitId) adMobAdUnitId:GetStringParam(adMobAdUnitId)];
    }

    void _interstitialAd(int adUnitInstanceId,
                                const char* position,
                                bool showAd,
                                const char* apsAdUnitId,
                                const char* adMobAdUnitId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            interstitialAdWithPosition:GetStringParam(position) showAd: showAd apsAdUnitId:GetStringParam(apsAdUnitId) adMobAdUnitId:GetStringParam(adMobAdUnitId)];
    }
    
    void _rewardedAd(int adUnitInstanceId,
                            const char* position,
                            bool showAd,
                            const char* apsAdUnitId,
                            const char* adMobAdUnitId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            rewardedAdWithPosition:GetStringParam(position) showAd: showAd apsAdUnitId:GetStringParam(apsAdUnitId) adMobAdUnitId:GetStringParam(adMobAdUnitId)];
    }
    
    void _showAd(int adUnitInstanceId,
                 bool respectSafeArea,
                 int bannerPosition) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
         showAdWithRespectSafeArea:respectSafeArea bannerPosition: bannerPosition];
    }

    void _destroyAd(int adUnitInstanceId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
    }
    
    const char* _getDeviceLanguage() {
        return strdup([[NimbusHelper getDeviceLanguage] UTF8String]);
    }

    int _getAtts() {
        return (int)[NimbusHelper getAtts];
    }

    bool _isLimitAdTrackingEnabled() {
        return [NimbusHelper isLimitAdTrackingEnabled];
    }
    
    const char* _getPlistJSON() {
        return strdup([[NimbusHelper getPlistJSON] UTF8String]);
    }

#if NIMBUS_ENABLE_LIVERAMP
    void _initializeLiveRamp(const char* configId, const char* email, bool hasConsentForNoLegislation) {
        [NimbusManager initializeLiveRampWithConfigId:GetStringParam(configId) email:GetStringParam(email) hasConsentForNoLegislation:hasConsentForNoLegislation];
    }
#endif
}
