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

     void _renderAd(int adUnitInstanceId,
                    const char* bidResponse,
                    bool isBlocking,
                    bool isRewarded,
                    double closeButtonDelay, 
                    const char* mintegralAdUnitId,
                    const char* mintegralAdUnitPlacementId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            renderAdWithBidResponse:GetStringParam(bidResponse) isBlocking:isBlocking isRewarded:isRewarded closeButtonDelay:closeButtonDelay
            mintegralAdUnitId:GetStringParam(mintegralAdUnitId) mintegralAdUnitPlacementId:GetStringParam(mintegralAdUnitPlacementId)];
    }

    void _destroyAd(int adUnitInstanceId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
    }

    const char* _getSessionId() {
        return strdup([[NimbusHelper getSessionId] UTF8String]);
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
    

#if NIMBUS_ENABLE_APS
    void _initializeAPSRequestHelper(const char* appKey, double timeoutInSeconds, bool enableTestMode) {
          [NimbusManager initializeAPSRequestHelperWithAppKey: GetStringParam(appKey)
                                                  timeoutInSeconds: timeoutInSeconds 
                                                  enableTestMode: enableTestMode];
    }

    void _addAPSSlot(const char* slotUUID, int width, int height, bool isVideo) {
        [NimbusManager addAPSSlotWithSlotUUID: GetStringParam(slotUUID) 
                                        width: width 
                                        height: height 
                                        isVideo: isVideo];
    }

    const char* _fetchAPSParams(int width, int height, bool includeVideo) {
        return strdup([[NimbusManager fetchAPSParamsWithWidth: width height:height includeVideo:includeVideo] UTF8String]);
    }
#endif

#if NIMBUS_ENABLE_VUNGLE
    void _initializeVungle(const char* appKey) {
        [NimbusManager initializeVungleWithAppKey: GetStringParam(appKey)];
    }
    
    const char* _fetchVungleBuyerId() {
        return strdup([[NimbusManager fetchVungleBuyerId] UTF8String]);
    }
#endif

#if NIMBUS_ENABLE_META
    void _initializeMeta(const char* appKey) {
        [NimbusManager initializeMetaWithAppKey: GetStringParam(appKey)];
    }
    
    const char* _fetchMetaBiddingToken() {
        return strdup([[NimbusManager fetchMetaBiddingToken] UTF8String]);
    }
#endif

#if NIMBUS_ENABLE_ADMOB
    void _initializeAdMob() {
        [NimbusManager initializeAdMob];
    }
    
    const char* _getAdMobRequestModifiers(int adUnitType, const char* adUnitId, int width, int height) {
        return strdup([[NimbusManager getAdMobRequestModifiersWithAdUnitType: adUnitType adUnitId: GetStringParam(adUnitId)  width: width height: height] UTF8String]);
    }
#endif

#if NIMBUS_ENABLE_MINTEGRAL
    void _initializeMintegral(const char* appId, const char* appKey) {
        [NimbusManager initializeMintegralWithAppId: GetStringParam(appId) 
                                                appKey: GetStringParam(appKey) ];
    }
    
    const char* _getMintegralRequestModifiers() {
        return strdup([[NimbusManager getMintegralRequestModifiers] UTF8String]);
    }
#endif

#if NIMBUS_ENABLE_UNITY_ADS
    void _initializeUnityAds(const char* gameId) {
        [NimbusManager initializeUnityAdsWithGameId: GetStringParam(gameId)];
    }
    
    const char* _fetchUnityAdsToken() {
        return strdup([[NimbusManager fetchUnityAdsToken] UTF8String]);
    }
#endif
#if NIMBUS_ENABLE_MOBILEFUSE
    void _initializeMobileFuse() {
        [NimbusManager initializeMobileFuse];
    }
    
    const char* _fetchMobileFuseToken() {
        return strdup([[NimbusManager fetchMobileFuseToken] UTF8String]);
    }
#endif

    const char* _getPrivacyStrings() {
        return strdup([[NimbusManager getPrivacyStrings] UTF8String]);
    }
}
