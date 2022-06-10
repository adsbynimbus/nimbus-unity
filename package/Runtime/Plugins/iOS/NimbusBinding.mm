//
//  NimbusBinding.mm
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 Timehop. All rights reserved.
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
                                     bool enableUnityLogs) {
        [NimbusManager initializeNimbusSDKWithPublisher: GetStringParam(publisher)
                                                 apiKey: GetStringParam(apikey)
                                        enableUnityLogs: enableUnityLogs];
    }

    void _showBannerAd(int adUnitInstanceId,
                       const char* position,
                       float bannerFloor) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
         showBannerAdWithPosition:GetStringParam(position) bannerFloor:bannerFloor];
    }

    void _showInterstitialAd(int adUnitInstanceId,
                             const char* position,
                             float bannerFloor,
                             float videoFloor,
                             double closeButtonDelay) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
         showInterstitialAdWithPosition:GetStringParam(position)
         bannerFloor:bannerFloor
         videoFloor:videoFloor
         closeButtonDelay:closeButtonDelay];
    }

    void _showRewardedVideoAd(int adUnitInstanceId,
                              const char* position,
                              float videoFloor,
                              double closeButtonDelay) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
         showRewardedVideoAdWithPosition:GetStringParam(position)
         videoFloor:videoFloor
         closeButtonDelay:closeButtonDelay];
    }

    void _destroyAd(int adUnitInstanceId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
    }

    const char* _getSessionId() {
        return strdup([UnityHelper getSessionId].UTF8String);
    }

    const char* _getUserAgent() {
        return strdup([UnityHelper getUserAgent].UTF8String);
    }

    const char* _getAdvertisingId() {
        return strdup([UnityHelper getAdvertisingId].UTF8String);
    }

    int _getConnectionType() {
        return (int)[UnityHelper getConnectionType];
    }

    const char* _getDeviceModel() {
        return strdup([UnityHelper getDeviceModel].UTF8String);
    }

    const char* _getSystemVersion() {
        return strdup([UnityHelper getSystemVersion].UTF8String);
    }

    bool _isLimitAdTrackingEnabled() {
        return [UnityHelper isLimitAdTrackingEnabled];
    }
}
