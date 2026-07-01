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
                   int refreshInterval,
                   float bidFloor,
                   bool respectSafeArea,
                   int bannerPosition,
                   bool showAd,
                   const char* thirdPartyDemand,
                   const char* requestModifiers) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            bannerAdWithPosition:GetStringParam(position) width:width height:height refreshInterval:refreshInterval bidFloor: bidFloor respectSafeArea:respectSafeArea
            bannerPosition:bannerPosition showAd: showAd thirdPartyDemand:GetStringParam(thirdPartyDemand)
        requestModifiersJson:GetStringParam(requestModifiers)];
    }

    void _interstitialAd(int adUnitInstanceId,
                                const char* position,
                                float bannerFloor,
                                float videoFloor,
                                bool showAd,
                                const char* thirdPartyDemand,
                                const char* requestModifiers) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            interstitialAdWithPosition:GetStringParam(position) bannerFloor:bannerFloor videoFloor:videoFloor showAd: showAd thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers)];
    }
    
    void _rewardedAd(int adUnitInstanceId,
                            const char* position,
                            float bidFloor,
                            bool showAd,
                            const char* thirdPartyDemand,
                            const char* requestModifiers) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            rewardedAdWithPosition:GetStringParam(position) bidFloor:bidFloor showAd: showAd thirdPartyDemand:GetStringParam(thirdPartyDemand) requestModifiersJson:GetStringParam(requestModifiers)];
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

    void _setIsSkOverlayEnabledForAllUnits(bool isEnabled) {
        [NimbusHelper setIsSKOverlayEnabledForAllUnitsWithIsEnabled:isEnabled];
    }

    void _setGdprProperties(bool gdprApplies, const char* gdprConsentString) {
        [NimbusHelper setGdprPropertiesWithGdprApplies:gdprApplies gdprConsentString:GetStringParam(gdprConsentString)];
    }

    void _setGppProperties(const char* gppSectionId, const char* gppConsentString) {
        [NimbusHelper setGppPropertiesWithGppSectionId:GetStringParam(gppSectionId) gppConsentString:GetStringParam(gppConsentString)];
    }

    void _setUsPrivacyString(const char* usPrivacyString) {
        [NimbusHelper setUsPrivacyStringWithUsPrivacyString:GetStringParam(usPrivacyString)];
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
