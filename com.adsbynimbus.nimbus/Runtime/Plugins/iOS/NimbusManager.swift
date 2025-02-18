//
//
//  NimbusManager.swift
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 AdsByNimbus. All rights reserved.
//

import Foundation
import NimbusRenderStaticKit
import NimbusRenderVideoKit
import NimbusKit
import AppTrackingTransparency
import AdSupport
#if NIMBUS_ENABLE_APS
import NimbusRequestAPSKit
import DTBiOSSDK
#endif
#if NIMBUS_ENABLE_VUNGLE
import VungleAdsSDK
import NimbusSDK
#endif
#if NIMBUS_ENABLE_META
import FBAudienceNetwork
#endif
#if NIMBUS_ENABLE_ADMOB
import GoogleMobileAds
import NimbusRequestKit
#endif

@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let adUnitInstanceId: Int
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?

    
    #if NIMBUS_ENABLE_APS
    private static var apsRequestHelper: NimbusAPSRequestHelper?
    #endif
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool,
        enableSDKInTestMode: Bool
    ) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        Nimbus.shared.logLevel = enableUnityLogs ? .debug : .off
        let videoRenderer = NimbusVideoAdRenderer()
        videoRenderer.showMuteButton = true
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): videoRenderer,
        ]
        Nimbus.shared.testMode = enableSDKInTestMode
    }
    
    @objc public class func nimbusManager(forAdUnityInstanceId adUnityInstanceId: Int) -> NimbusManager {
        guard let manager = managerDictionary[adUnityInstanceId] else {
            let manager = NimbusManager(adUnitInstanceId: adUnityInstanceId)
            managerDictionary[adUnityInstanceId] = manager
            return manager
        }
        return manager
    }

    #if NIMBUS_ENABLE_APS
    @objc public class func initializeAPSRequestHelper(appKey: String, timeoutInSeconds: Double, enableTestMode: Bool) {
        apsRequestHelper = NimbusAPSRequestHelper(appKey: appKey, timeoutInSeconds: timeoutInSeconds)
        DTBAds.sharedInstance().testMode = enableTestMode
    }

    @objc public class func addAPSSlot(slotUUID: String, width: Int, height: Int, isVideo: Bool) {
        apsRequestHelper?.addAPSSlot(slotUUID: slotUUID, width: width, height: height, isVideo: isVideo) 
    }

    @objc public class func fetchAPSParams(width: Int, height: Int, includeVideo: Bool) -> String {
        let data = apsRequestHelper?.fetchAPSParams(width: width, height: height, includeVideo: includeVideo);
        if let unwrapped = data {
            return unwrapped;
        }
        return "";
    }
    #endif
    
    #if NIMBUS_ENABLE_VUNGLE
        @objc public class func initializeVungle(appKey: String) {
            let vungleRequestInterceptor = NimbusVungleRequestInterceptor(appId: appKey)
            NimbusRequestManager.requestInterceptors?.append(vungleRequestInterceptor)
            Nimbus.shared.renderers[.vungle] = NimbusVungleAdRenderer()
        }
        
        @objc public class func fetchVungleBuyerId() -> String {
            return VungleAds.getBiddingToken()
        }
    #endif
    
    #if NIMBUS_ENABLE_META
        @objc public class func initializeMeta(appKey: String) {
            FBAdSettings.setMediationService("Ads By Nimbus")
            Nimbus.shared.renderers[.facebook] = NimbusFANAdRenderer()
            if #available(iOS 14.5, *), ATTrackingManager.trackingAuthorizationStatus == .authorized {
                FBAdSettings.setAdvertiserTrackingEnabled(true)
            }
        }
        @objc public class func fetchMetaBiddingToken() -> String {
            return FBAdSettings.bidderToken
        }    
    #endif
    
    #if NIMBUS_ENABLE_ADMOB
        @objc public class func initializeAdMob() {
            Nimbus.shared.renderers[.forNetwork("admob")] = NimbusAdMobAdRenderer()
        }
        @objc public class func getAdMobRequestModifiers(adUnitType: Int, adUnitId: String, width: Int, height: Int) -> String {    
            switch adUnitType {
               case 0, 1:
                   let request = NimbusRequest.forBannerAd(position: "banner", format: NimbusAdFormat(width: width, height: height))
                          .withAdMobBanner(adUnitId: adUnitId)
                   request.interceptors?.first?.modifyRequest(request: request)
                   return request.user?.extensions?["admob_gde_signals"]?.value as? String ?? ""
               case 2:
                   let request = NimbusRequest.forInterstitialAd(position: "interstitial")
                       .withAdMobInterstitial(adUnitId: adUnitId)
                   request.interceptors?.first?.modifyRequest(request: request)
                   return request.user?.extensions?["admob_gde_signals"]?.value as? String ?? ""
               case 3:
                   let request = NimbusRequest.forRewardedVideo(position: "rewarded")
                       .withAdMobRewarded(adUnitId: adUnitId)
                   request.interceptors?.first?.modifyRequest(request: request)
                   return request.user?.extensions?["admob_gde_signals"]?.value as? String ?? ""
               default:
                   break
           }
           return ""
        }
    #endif
    
    // MARK: - Private Functions
    
    private init(adUnitInstanceId: Int) {
        self.adUnitInstanceId = adUnitInstanceId
    }
    
    private func unityViewController() -> UIViewController? {
        UnityFramework.getInstance().appController().rootViewController
    }
    
    // MARK: - Public Functions
    
    @objc public func renderAd(bidResponse: String, isBlocking: Bool, isRewarded: Bool, closeButtonDelay: TimeInterval) {
        guard let data = bidResponse.data(using: .utf8) else {
            Nimbus.shared.logger.log("Unable to get data from bid response", level: .error)
            return
        }
        
        guard
            let decodedData = Data(base64Encoded: data)
        else {
            Nimbus.shared.logger.log("Unable to decode base64Encoded data", level: .error)
            return
        }
        
        guard let nimbusAd = try? JSONDecoder().decode(NimbusAd.self, from: decodedData) else {
            Nimbus.shared.logger.log("Unable to get NimbusAd from bid response data", level: .error)
            return
        }
        
        guard let viewController = unityViewController() else { return }
        
        if isBlocking {
            let companionAd: NimbusCompanionAd
            if UIDevice.current.orientation.isLandscape {
                companionAd = NimbusCompanionAd(width: 480, height: 320, renderMode: .endCard)
            } else {
                companionAd = NimbusCompanionAd(width: 320, height: 480, renderMode: .endCard)
            }
            
            do {
                adController = try Nimbus.loadBlocking(ad: nimbusAd, presentingViewController: viewController, delegate: self, isRewarded: isRewarded, companionAd: companionAd)
                adController?.volume = 100
                adController?.start()
            } catch {
                Nimbus.shared.logger.log(error.localizedDescription, level: .error)
                return
            }
        } else {
            let contentView = UIView()
                contentView.translatesAutoresizingMaskIntoConstraints = false
                viewController.view.addSubview(contentView)
                
                NSLayoutConstraint.activate([
                    contentView.centerXAnchor.constraint(equalTo: viewController.view.centerXAnchor),
                    contentView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor),
                    contentView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor),
                    contentView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor),
                ])
                            
            adController = Nimbus.load(ad: nimbusAd, container: contentView, adPresentingViewController: viewController, delegate: self)
        }
        
        UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
    }
    
    @objc public func destroyExistingAd() {
        adController?.destroy()
        removeReferenceFromManagerDictionary()
    }
    
    private func removeReferenceFromManagerDictionary() {
        NimbusManager.managerDictionary.removeValue(forKey: adUnitInstanceId)
    }
}

// MARK: - AdControllerDelegate implementation

extension NimbusManager: AdControllerDelegate {
    
    public func didReceiveNimbusEvent(controller: AdController, event: NimbusEvent) {
        let eventName: String
        switch event {
        case .loaded, .loadedCompanionAd, .firstQuartile, .midpoint, .thirdQuartile, .skipped:
            return // Unity doesn't handle these events
        case .impression:
            eventName = "IMPRESSION"
        case .clicked:
            eventName = "CLICKED"
        case .paused:
            eventName = "PAUSED"
        case .resumed:
            eventName = "RESUMED"
        case .completed:
            eventName = "COMPLETED"
        case .destroyed:
            eventName = "DESTROYED"
            removeReferenceFromManagerDictionary()
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        
        UnityBinding.sendMessage(
            methodName: "OnAdEvent",
            params: [
                "adUnitInstanceID": adUnitInstanceId,
                "eventName": eventName
            ]
        )
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceId,
                "errorMessage": error.localizedDescription
            ]
        )
        destroyExistingAd()
    }
}