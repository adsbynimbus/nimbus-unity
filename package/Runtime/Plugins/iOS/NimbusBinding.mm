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
                    double closeButtonDelay) {
        if (bidResponse) {
            [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId]
             renderAdWithBidResponse:GetStringParam(bidResponse) isBlocking:isBlocking closeButtonDelay:closeButtonDelay];
        } else {
            // TODO: Error
        }
    }

    void _destroyAd(int adUnitInstanceId) {
        [[NimbusManager nimbusManagerForAdUnityInstanceId:adUnitInstanceId] destroyExistingAd];
    }

    void _getSessionId(char* sessionId) {
        sessionId = (char *)[[NimbusHelper getSessionId] UTF8String];
    }

    void _getUserAgent(char* userAgent) {
        userAgent = (char *)[[NimbusHelper getUserAgent] UTF8String];
    }

    void _getAdvertisingId(char* advertisingId) {
        advertisingId = (char *)[[NimbusHelper getAdvertisingId] UTF8String];
    }

    void _getConnectionType(int* connectionType) {
        *connectionType = (int)[NimbusHelper getConnectionType];
    }

    void _getDeviceModel(char* deviceModel) {
        deviceModel = (char *)[[NimbusHelper getDeviceModel] UTF8String];
    }

    void _getSystemVersion(const char* systemVersion) {
        systemVersion = (char *)[[NimbusHelper getSystemVersion] UTF8String];
    }

    void _isLimitAdTrackingEnabled(bool* limitAdTracking) {
        *limitAdTracking = [NimbusHelper isLimitAdTrackingEnabled];
    }
}
