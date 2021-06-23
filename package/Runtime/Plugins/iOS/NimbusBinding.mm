//
//  NimbusBinding.mm
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 Timehop. All rights reserved.
//

#import "NimbusSDK-Swift.h"

#pragma mark - Helpers

// Converts C style string to NSString
#define GetStringParam(_x_) ((_x_) != NULL ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""])
#define GetNullableStringParam(_x_) ((_x_) != NULL ? [NSString stringWithUTF8String:_x_] : nil)

#pragma mark - C interface

extern "C" {

void _initializeSDKWithPublisher(const char* publisher,
                                 const char* apikey,
                                 bool enableSDKInTestMode,
                                 bool enableUnityLogs) {
    NSString* publishString = GetStringParam(publisher);
    NSString* apiKeyString = GetStringParam(apikey);
    
    [NimbusManager initializeNimbusSDKWithPublisher: publishString
                                             apiKey: apiKeyString
                                enableSDKInTestMode: enableSDKInTestMode
                                    enableUnityLogs: enableUnityLogs];
}

void _showBannerAd(int adUnitInstanceId, const char* position, float bannerFloor) {
    NSString* positionString = GetStringParam(position);
    [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] showBannerAdWithPosition:positionString
                                                                                     bannerFloor:bannerFloor];
}

void _showInterstitialAd(int adUnitInstanceId, const char* position, float bannerFloor, float videoFloor, double closeButtonDelay) {
    NSString* positionString = GetStringParam(position);
    [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] showInterstitialAdWithPosition:positionString
                                                                                           bannerFloor:bannerFloor
                                                                                            videoFloor:videoFloor
                                                                                      closeButtonDelay:closeButtonDelay];
}

void _showRewardedVideoAd(int adUnitInstanceId, const char* position, float videoFloor, double closeButtonDelay) {
    NSString* positionString = GetStringParam(position);
    [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] showRewardedVideoAdWithPosition:positionString
                                                                                             videoFloor:videoFloor
                                                                                       closeButtonDelay:closeButtonDelay];
}

void _setGDPRConsentString(const char* consent) {
    NSString* consentString = GetStringParam(consent);
    [NimbusManager setGDPRConsentStringWithConsent: consentString];
}

void _destroyAd(int adUnitInstanceId) {
    [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
}

}
