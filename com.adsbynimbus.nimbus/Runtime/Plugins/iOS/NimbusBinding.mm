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
                                     bool enableSDKInTestMode) {
        [NimbusManager initializeNimbusSDKWithPublisher: GetStringParam(publisher)
                                                 apiKey: GetStringParam(apikey)
                                        enableUnityLogs: enableUnityLogs
                                        enableSDKInTestMode: enableSDKInTestMode];
    }

    void _bannerAd(int adUnitInstanceId, 
                   const char* position,
                   int width,
                   int height,                   
                   int refreshInterval,
                   bool respectSafeArea,
                   int bannerPosition,
                   bool showAd) {
        return strdup([[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            bannerAdWithPosition:GetStringParam(position) width:width height:height refreshInterval:refreshInterval respectSafeArea:respectSafeArea
            bannerPosition:bannerPosition showAd: showAd]);
    }

    void _interstitialAd(int adUnitInstanceId,
                                const char* position,
                                bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            interstitialAdWithPosition:GetStringParam(position) showAd: showAd];
    }
    
    void _rewardedAd(int adUnitInstanceId,
                            const char* position,
                            bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            rewardedAdWithPosition:GetStringParam(position) showAd: showAd];
    }
    
    void _showAd(int adUnitInstanceId,
                 bool respectSafeArea,
                 int bannerPosition) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            showAdWithRespectSafeArea:respectSafeArea bannerPosition: bannerPosition]
    }

    void _destroyAd(int adUnitInstanceId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
    }

    const char* _getUserAgent() {
        return strdup([[NimbusHelper getUserAgent] UTF8String]);
    }

    const char* _getAdvertisingId() {
        return strdup([[NimbusHelper getAdvertisingId] UTF8String]);
    }

    int _getConnectionType() {
        return (int)[NimbusHelper getConnectionType];
    }
    
    const char* _getDeviceModel() {
        return strdup([[NimbusHelper getDeviceModel] UTF8String]);
    }
    
    const char* _getDeviceLanguage() {
        return strdup([[NimbusHelper getDeviceLanguage] UTF8String]);
    }

    const char* _getSystemVersion() {
        return strdup([[NimbusHelper getSystemVersion] UTF8String]);
    }

    int _getAtts() {
        return (int)[NimbusHelper getAtts];
    }

    const char* _getVendorId() {
        return strdup([[NimbusHelper getVendorId] UTF8String]);
    }

    void _setCoppa(bool flag) {
        [NimbusHelper setCoppaWithFlag: flag];
    }

    bool _isLimitAdTrackingEnabled() {
        return [NimbusHelper isLimitAdTrackingEnabled];
    }

    const char* _getVersion() {
        return strdup([[NimbusHelper getVersion] UTF8String]);
    }
    
    const char* _getPlistJSON() {
        return strdup([[NimbusHelper getPlistJSON] UTF8String]);
    }

#if NIMBUS_ENABLE_LIVERAMP
    void _initializeLiveRamp(const char* configId, const char* email, const char* phoneNumber, bool isTestMode, bool hasConsentForNoLegislation) {
        [NimbusManager initializeLiveRampWithConfigId:GetStringParam(configId) email:GetStringParam(email) phoneNumber:GetStringParam(phoneNumber) isTestMode:isTestMode hasConsentForNoLegislation:hasConsentForNoLegislation];
    }
    
    const char* _getLiveRampData() {
        return strdup([[NimbusManager getLiveRampData] UTF8String]);
    }
#endif
}
