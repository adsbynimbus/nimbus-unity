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

     void _renderAd(int adUnitInstanceId,
                    const char* bidResponse,
                    bool isBlocking,
                    bool isRewarded,
                    double closeButtonDelay) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
            renderAdWithBidResponse:GetStringParam(bidResponse) isBlocking:isBlocking isRewarded:isRewarded closeButtonDelay:closeButtonDelay];
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

    const char* _getSystemVersion() {
        return strdup([[NimbusHelper getSystemVersion] UTF8String]);
    }

    bool _isLimitAdTrackingEnabled() {
        return [NimbusHelper isLimitAdTrackingEnabled];
    }
}
