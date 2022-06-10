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
}
