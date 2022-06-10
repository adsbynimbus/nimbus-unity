//
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
    
    private let adUnitInstanceId: Int
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?
    private var adView: AdView?
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool
    ) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        Nimbus.shared.logLevel = enableUnityLogs ? .debug : .off
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
    
    // MARK: - Private Functions
    
    private init(adUnitInstanceId: Int) {
        self.adUnitInstanceId = adUnitInstanceId
    }
    
    private func unityViewController() -> UIViewController? {
        UIApplication.shared.connectedScenes
            .filter { $0.activationState == .foregroundActive }
            .compactMap { $0 as? UIWindowScene }
            .first?.windows
            .filter { $0.isKeyWindow }
            .first?.rootViewController
    }
    
    // MARK: - Public Functions
    
    @objc public func showBannerAd(position: String, bannerFloor: Float) {
        guard let viewController = unityViewController() else { return }
        
        let adFormat = NimbusAdFormat.banner320x50
        let adPosition = NimbusPosition.footer
        let request = NimbusRequest.forBannerAd(
            position: position,
            format: adFormat,
            adPosition: adPosition
        )
        request.impressions[0].bidFloor = bannerFloor
        
        let view = AdView(bannerFormat: adFormat)
        adView = view
        view.attachToView(parentView: viewController.view, position: adPosition)
        
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showAd(
            request: request,
            container: view,
            adPresentingViewController: viewController
        )
    }
    
    @objc public func showInterstitialAd(position: String, bannerFloor: Float, videoFloor: Float, closeButtonDelay: Double) {
        guard let viewController = unityViewController() else { return }
        
        let request = NimbusRequest.forInterstitialAd(position: position)
        request.impressions[0].banner?.bidFloor = bannerFloor
        request.impressions[0].video?.bidFloor = videoFloor
        
        let adFormat = UIDevice.current.orientation.isLandscape ? 
            NimbusAdFormat.interstitialLandscape : NimbusAdFormat.interstitialPortrait
        let banner = NimbusBanner(
            width: adFormat.width,
            height: adFormat.height,
            companionAdRenderMode: .endCard
        )
        
        // Forces the request to show the end card
        var impression = request.impressions[0]
        impression.video?.companionAds = [banner]
        request.impressions[0] = impression
                
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showBlockingAd(
            request: request,
            closeButtonDelay: closeButtonDelay,
            adPresentingViewController: viewController
        )
    }
    
    @objc public func showRewardedVideoAd(position: String, videoFloor: Float, closeButtonDelay: Double) {
        guard let viewController = unityViewController() else { return }
        
        let request = NimbusRequest.forVideoAd(position: position)
        request.impressions[0].video?.bidFloor = videoFloor
                
        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
        nimbusAdManager?.showRewardedAd(
            request: request,
            closeButtonDelay: closeButtonDelay,
            adPresentingViewController: viewController
        )
    }
    
    @objc public func destroyExistingAd() {
        adController?.destroy()
        adView?.removeFromSuperview()
        adView = nil
        removeReferenceFromManagerDictionary()
    }
    
    private func removeReferenceFromManagerDictionary() {
        NimbusManager.managerDictionary.removeValue(forKey: adUnitInstanceId)
    }
    
}

// MARK: - NimbusAdManagerDelegate implementation

extension NimbusManager: NimbusAdManagerDelegate {
    
    public func didCompleteNimbusRequest(request: NimbusRequest, ad: NimbusAd) {
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "auctionId": ad.auctionId,
            "bidRaw": ad.bidRaw,
            "bidInCents": ad.bidInCents,
            "network": ad.network,
            "placementId": ad.placementId ?? ""
        ]
        UnityBinding.sendMessage(methodName: "OnAdResponse", params: params)
    }
    
    public func didFailNimbusRequest(request: NimbusRequest, error: NimbusError) {
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "errorMessage": error.localizedDescription
        ]
        UnityBinding.sendMessage(methodName: "OnError", params: params)
        destroyExistingAd()
    }
    
    public func didRenderAd(request: NimbusRequest, ad: NimbusAd, controller: AdController) {
        self.adController = controller
        self.adController?.delegate = self
        
        let params: [String: Any] = ["adUnitInstanceID": adUnitInstanceId]
        UnityBinding.sendMessage(methodName: "OnAdResponse", params: params)
    }
    
}

// MARK: - AdControllerDelegate implementation

extension NimbusManager: AdControllerDelegate {
    
    public func didReceiveNimbusEvent(controller: AdController, event: NimbusEvent) {
        let eventName: String
        switch event {
        case .loaded, .loadedCompanionAd, .firstQuartile, .midpoint, .thirdQuartile:
            return // Unity doesn't handle these events
        case .impression:
            eventName = "IMPRESSION"
        case .clicked:
            eventName = "CLICKED"
        case .paused:
            eventName = "PAUSED"
        case .resumed:
            eventName = "RESUME"
        case .completed:
            eventName = "COMPLETED"
        case .destroyed:
            eventName = "DESTROYED"
            removeReferenceFromManagerDictionary()
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "eventName": eventName
        ]
        UnityBinding.sendMessage(methodName: "OnAdEvent", params: params)
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "errorMessage": error.localizedDescription
        ]
        UnityBinding.sendMessage(methodName: "OnError", params: params)
        destroyExistingAd()
    }
}
