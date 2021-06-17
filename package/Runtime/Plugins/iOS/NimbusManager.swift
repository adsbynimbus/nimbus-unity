//
//  NimbusManager.swift
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 Timehop. All rights reserved.
//

import Foundation
import NimbusRenderStaticKit
import NimbusRenderVideoKit
import NimbusKit

@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let kCallbackTarget = "NimbusIOSAdManager"
    
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?
    
    private var adView: AdView?
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(publisher: String,
                                          apiKey: String,
                                          enableSDKInTestMode: Bool,
                                          enableUnityLogs: Bool) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        
        Nimbus.shared.logLevel = enableUnityLogs ? .debug : .off
        Nimbus.shared.testMode = enableSDKInTestMode
        
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): NimbusVideoAdRenderer()
        ]
    }
    
    @objc public class func nimbusManager(forAdUnityInstanceId adUnityInstanceId: Int) -> NimbusManager {
        guard let manager = managerDictionary[adUnityInstanceId] else {
            let manager = NimbusManager(adUnitInstanceId: adUnityInstanceId)
            managerDictionary[adUnityInstanceId] = manager
            return manager
        }
        return manager
    }
    
    @objc public class func setGDPRConsentString(consent: String) {
        var user = NimbusRequestManager.user ?? NimbusUser()
        user.configureGdprConsent(didConsent: true, consentString: consent)
        NimbusRequestManager.user = user
    }
        
    // MARK: - Private Functions
    
    private init(adUnitInstanceId: Int) {
        self.adUnitInstanceId = adUnitInstanceId
    }
    
    @objc public func unityViewController() -> UIViewController? {
        return UIApplication.shared.windows.first { $0.isKeyWindow }?.rootViewController
    }
    
    @objc public func showBannerAd(position: String, bannerFloor: Float) {
        guard let viewController = unityViewController() else { return }
        
        let adFormat = NimbusAdFormat.banner320x50
        let adPosition = NimbusPosition.footer
        
        let request = NimbusRequest.forBannerAd(position: position,
                                                format: adFormat,
                                                adPosition: adPosition)
        request.impressions[0].bidFloor = bannerFloor
        
        let view = AdView(bannerFormat: adFormat)
        self.adView = view
        
        view.attachToView(parentView: viewController.view, position: adPosition)
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showAd(request: request,
                                container: view,
                                adPresentingViewController: viewController)
    }
    
    @objc public func showInterstitialAd(position: String, bannerFloor: Float, videoFloor: Float, closeButtonDelay: Double) {
        guard let viewController = unityViewController() else { return }
        
        let request = NimbusRequest.forInterstitialAd(position: position)
        request.impressions[0].banner?.bidFloor = bannerFloor
        request.impressions[0].video?.bidFloor = videoFloor
        
        (Nimbus.shared.renderers[.forAuctionType(.video)] as? NimbusVideoAdRenderer)?.showMuteButton = false // false by default
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showRewardedAd(request: request,
                                        closeButtonDelay: closeButtonDelay,
                                        adPresentingViewController: viewController)
    }
    
    @objc public func showRewardedVideoAd(position: String, videoFloor: Float, closeButtonDelay: Double) {
        guard let viewController = unityViewController() else { return }
        
        let request = NimbusRequest.forVideoAd(position: position)
        request.impressions[0].video?.bidFloor = videoFloor
        
        (Nimbus.shared.renderers[.forAuctionType(.video)] as? NimbusVideoAdRenderer)?.showMuteButton = false // false by default
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showRewardedAd(request: request,
                                        closeButtonDelay: closeButtonDelay,
                                        adPresentingViewController: viewController)
    }
    
    @objc public func destroyExistingAd() {
        adController?.destroy()
        adView?.removeFromSuperview()
        adView = nil
    }
    
}

// MARK: - NimbusAdManagerDelegate implementation

extension NimbusManager: NimbusAdManagerDelegate {
    
    public func didCompleteNimbusRequest(request: NimbusRequest, ad: NimbusAd) {
        // do nothing
    }
    
    public func didFailNimbusRequest(request: NimbusRequest, error: NimbusError) {
        UnitySendMessage(kCallbackTarget, "OnError", error.localizedDescription);
    }
    
    public func didRenderAd(request: NimbusRequest, ad: NimbusAd, controller: AdController) {
        self.adController = controller
        self.adController?.delegate = self
        UnitySendMessage(kCallbackTarget, "OnAdRendered", "");
    }
    
}

// MARK: - AdControllerDelegate implementation

extension NimbusManager: AdControllerDelegate {
    
    public func didReceiveNimbusEvent(controller: AdController, event: NimbusEvent) {
        var method = "OnAdEvent", eventName = ""
        switch event {
        case .loaded:
            eventName = "LOADED"
        case .loadedCompanionAd(width: _, height: _):
            return // Unity doesn't handle this event
        case .impression:
            eventName = "IMPRESSION"
        case .clicked:
            eventName = "CLICKED"
        case .paused:
            eventName = "PAUSED"
        case .resumed:
            eventName = "RESUME"
        case .firstQuartile:
            eventName = "FIRST_QUARTILE"
        case .midpoint:
            eventName = "MIDPOINT"
        case .thirdQuartile:
            eventName = "THIRD_QUARTILE"
        case .completed:
            eventName = "COMPLETED"
        case .destroyed:
            eventName = "DESTROYED"
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        UnitySendMessage(kCallbackTarget, method, eventName);
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        UnitySendMessage(kCallbackTarget, "OnError", error.localizedDescription);
        destroyExistingAd()
    }
}
