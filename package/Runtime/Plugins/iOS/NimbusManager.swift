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
    
    @objc public static let shared = NimbusManager()
    
    private let kCallbackTarget = "IOSAdManager"
    
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?
    
    private var adView: AdView?
    
    @objc public func initializeNimbusSDK(publisher: String,
                                          apiKey: String,
                                          enableSDKInTestMode: Bool,
                                          logLevel: Int) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        
        Nimbus.shared.logLevel = NimbusLogLevel(rawValue: logLevel) ?? .off
        Nimbus.shared.testMode = enableSDKInTestMode
        
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): NimbusVideoAdRenderer()
        ]
        
        NimbusAdManager.user = NimbusUser(age: 20, gender: .male)
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
        
        self.destroyExistingAd() // TODO: confirm if this is the expected
        
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
        
        self.destroyExistingAd() // TODO: confirm if this is the expected
        
        let request = NimbusRequest.forInterstitialAd(position: position)
        request.impressions[0].banner?.bidFloor = bannerFloor
        request.impressions[0].video?.bidFloor = videoFloor
        
        (Nimbus.shared.renderers[.forAuctionType(.video)] as? NimbusVideoAdRenderer)?.showMuteButton = true // true by default
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showRewardedVideoAd(request: request,
                                             closeButtonDelay: closeButtonDelay,
                                             adPresentingViewController: viewController)        
    }
    
    @objc public func showRewardedVideoAd(position: String, videoFloor: Float, closeButtonDelay: Double) {
        guard let viewController = unityViewController() else { return }
        
        self.destroyExistingAd() // TODO: confirm if this is the expected
        
        let request = NimbusRequest.forRewardedVideo(position: position)
        request.impressions[0].video?.bidFloor = videoFloor
        
        (Nimbus.shared.renderers[.forAuctionType(.video)] as? NimbusVideoAdRenderer)?.showMuteButton = true // true by default
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showRewardedVideoAd(request: request,
                                             closeButtonDelay: closeButtonDelay,
                                             adPresentingViewController: viewController)
    }
    
    @objc public func setGDPRConsentString(consent: String) {
        var user = NimbusRequestManager.user ?? NimbusUser()
        user.configureGdprConsent(didConsent: true, consentString: consent)
        NimbusRequestManager.user = user
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
            eventName = "LOADED_COMPANION_AD"
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
        }
        UnitySendMessage(kCallbackTarget, method, eventName);
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        UnitySendMessage(kCallbackTarget, "OnError", error.localizedDescription);
        destroyExistingAd()
    }
}
