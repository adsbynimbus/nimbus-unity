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

typedef char* (*VerificationMarkupCallback)(const char* response, NSInteger index);
typedef char* (*VerificationResourceCallback)(const char* response, NSInteger index);

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
                   const char* addFormats,
                   int adPosition,
                   float bidFloor,                   
                   int refreshInterval,
                   int screenPosition,
                   bool respectSafeArea,
                   const char* thirdPartyDemand,
                   const char* requestModifiers,
                   bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            bannerAdWithPosition:GetStringParam(position) width:width height:height addFormats:GetStringParam(addFormats) adPosition: adPosition
                bidFloor: bidFloor refreshInterval:refreshInterval screenPosition:screenPosition respectSafeArea:respectSafeArea
                thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers) showAd:showAd];
    }

    void _dynamicUnit(int adUnitInstanceId,
               const char* position,
               const char* addFormats,
               int orientation,
               int adPosition,
               float bidFloor,
               int refreshInterval,
               int screenPosition,
               bool respectSafeArea,
               const char* thirdPartyDemand,
               const char* requestModifiers,
               bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            dynamicUnitWithPosition:GetStringParam(position) addFormats:GetStringParam(addFormats) orientation: orientation adPosition: adPosition
                bidFloor: bidFloor refreshInterval:refreshInterval screenPosition:screenPosition respectSafeArea:respectSafeArea
                thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers) showAd:showAd];
    }
    void _fullscreenAd(int adUnitInstanceId,
                                const char* position,
                                int orientation,
                                const char* thirdPartyDemand,
                                const char* requestModifiers,
                                bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            fullscreenAdWithPosition:GetStringParam(position) orientation:orientation thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers) showAd:showAd];
    }

    void _interstitialAd(int adUnitInstanceId,
                                const char* position,
                                const char* addFormats,
                                int orientation,
                                float bidFloor,
                                const char* thirdPartyDemand,
                                const char* requestModifiers,
                                bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            interstitialAdWithPosition:GetStringParam(position) addFormats:GetStringParam(addFormats) orientation:orientation bidFloor:bidFloor  thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers) showAd: showAd];
    }
    
    void _rewardedAd(int adUnitInstanceId,
                            const char* position,
                            int orientation,
                            float bidFloor,
                            const char* thirdPartyDemand,
                            const char* requestModifiers,
                            bool showAd) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            rewardedAdWithPosition:GetStringParam(position) orientation:orientation bidFloor:bidFloor thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers) showAd: showAd];
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

    void _setSessionId(const char* sessionId) {
        [NimbusHelper setSessionIdWithSessionId:GetStringParam(sessionId)];
    }

    void _setCoppa(bool coppa) {
        [NimbusHelper setCoppaWithCoppa:coppa];
    }

    void _setApp(const char* appJsonStr) {
        [NimbusHelper setAppWithAppJsonStr:GetStringParam(appJsonStr)];
    }

    void _setUser(const char* userJsonStr) {
        [NimbusHelper setUserWithUserJsonStr:GetStringParam(userJsonStr)];
    }

    void _setBlockedAdvertisingDomains(const char* domains) {
        [NimbusHelper setBlockedAdvertisingDomainsWithDomains:GetStringParam(domains)];
    }

    void _setRequestUrl(const char* url) {
        [NimbusHelper setRequestUrlWithUrl:GetStringParam(url)];
    }

    void _setAdditionalRequestHeaders(const char* headers) {
        [NimbusHelper setAdditionalRequestHeadersWithHeadersJsonStr:GetStringParam(headers)];
    }

    void _setInterceptorTimeout(int timeout) {
        [NimbusHelper setInterceptorTimeoutWithTimeout:timeout];
    }

    void _showMuteButton(bool show) {
        [NimbusHelper showMuteButtonWithShow:show];
    }

    void _enableSwipeProtection(bool enableSwipeProtection) {
        [NimbusHelper enableSwipeProtectionWithEnable:enableSwipeProtection];
    }

    void _setExtendedIds(const char* source, const char* ids) {
        [NimbusHelper setExtendedIdsWithSource:GetStringParam(source) idStr:GetStringParam(ids)];
    }

    void _clearExtendedIds() {
        [NimbusHelper clearExtendedIds];
    }

    void _setIsSkOverlayEnabledForAllUnits(bool isEnabled) {
        [NimbusHelper setIsSKOverlayEnabledForAllUnitsWithIsEnabled:isEnabled];
    }

    void _setVerificationCallbacks(VerificationMarkupCallback markupCallbackPtr, VerificationResourceCallback resourceCallbackPtr, int numCallbacks) {
        [NimbusHelper setVerificationProvidersWithMarkupCallback:markupCallbackPtr resourceCallback:resourceCallbackPtr numCallbacks:numCallbacks];
    }


#if NIMBUS_ENABLE_LIVERAMP
    void _initializeLiveRamp(const char* configId, const char* email, bool hasConsentForNoLegislation, bool testMode) {
        [NimbusManager initializeLiveRampWithConfigId:GetStringParam(configId) email:GetStringParam(email) hasConsentForNoLegislation:hasConsentForNoLegislation testMode:testMode];
    }
#endif
}
