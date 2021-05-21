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

    private lazy var nimbusAdManager: NimbusAdManager? = nil
    private lazy var adController: AdController? = nil

    @objc public func initializeNimbusSDK(publisher: String,
                                          apiKey: String,
                                          enableSDKInTestMode: Bool,
                                          logLevel: Int,
                                          appName: String,
                                          appDomain: String,
                                          bundleId: String,
                                          storeUrlString: String,
                                          showMuteButton: Bool) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        print("initializeNimbusSDK called in the swift code with publisher: \(publisher) - apiKey: \(apiKey)")

        Nimbus.shared.logLevel = NimbusLogLevel(rawValue: logLevel) ?? .off
        Nimbus.shared.testMode = enableSDKInTestMode
        // Nimbus.shared.logger = // TODO: add logged


        let videoRenderer = NimbusVideoAdRenderer()
        videoRenderer.showMuteButton = showMuteButton
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): videoRenderer
        ]

        guard let domainUrl = URL(string: appDomain),
              let storeUrl = URL(string: storeUrlString) else {
            print("Error initializing Nimbus SDK. A valid URL is required for app domain and store url. Received appDomain: \(appDomain) | storeUrl: \(storeUrlString)")
            return
        }

        NimbusAdManager.app = NimbusApp(
            name: appName,
            domain: domainUrl,
            bundle: bundleId,
            storeUrl: storeUrl
        )
        NimbusAdManager.user = NimbusUser(age: 20, gender: .male)

        nimbusAdManager = NimbusAdManager()
        nimbusAdManager?.delegate = self
    }

    @objc public func unityViewController() -> UIViewController? {
        return UIApplication.shared.windows.first { $0.isKeyWindow }?.rootViewController
    }

    @objc public func showBannerAd(position: String) {
        guard let viewController = unityViewController() else { return }

        nimbusAdManager?.showAd(request: NimbusRequest.forBannerAd(position: position),
                                container: viewController.view,
                                adPresentingViewController: viewController)
    }

    @objc public func showInterstitialAd(position: String) {
        guard let viewController = unityViewController() else { return }

        nimbusAdManager?.showAd(request: NimbusRequest.forInterstitialAd(position: position),
                                container: viewController.view,
                                adPresentingViewController: viewController)
    }

    @objc public func showRewardedVideoAd(position: String) {
        guard let viewController = unityViewController() else { return }

        nimbusAdManager?.showRewardedVideoAd(request: NimbusRequest.forRewardedVideo(position: position),
                                             adPresentingViewController:viewController)
    }

    @objc public func setGDPRConsentString(consent: String) {
        var user = NimbusRequestManager.user ?? NimbusUser()
        user.configureGdprConsent(didConsent: true, consentString: consent)
        NimbusRequestManager.user = user
    }

    func sendEvent() {
        UnitySendMessage(kCallbackTarget, "OnIOSEventReceived", "params");
    }
}

extension NimbusManager: NimbusAdManagerDelegate {

    public func didCompleteNimbusRequest(request: NimbusRequest, ad: NimbusAd) {
        print("didCompleteNimbusRequest")
    }

    public func didFailNimbusRequest(request: NimbusRequest, error: NimbusError) {
        print("didFailNimbusRequest: \(error.localizedDescription)")
    }

    public func didRenderAd(request: NimbusRequest, ad: NimbusAd, controller: AdController) {
        self.adController = controller
        self.adController?.delegate = self
        UnitySendMessage(kCallbackTarget, "OnAdRendered", "");
    }

}

extension NimbusManager: AdControllerDelegate {

    public func didReceiveNimbusEvent(controller: AdController, event: NimbusEvent) {
        var method = "OnAdEvent", eventName = ""
        switch event {
        case .loaded:
            eventName = "loaded"
        case .loadedCompanionAd(width: _, height: _):
            eventName = "loadedCompanionAd"
        case .impression:
            eventName = "impression"
        case .clicked:
            eventName = "clicked"
        case .paused:
            eventName = "paused"
        case .resumed:
            eventName = "resumed"
        case .firstQuartile:
            eventName = "firstQuartile"
        case .midpoint:
            eventName = "midpoint"
        case .thirdQuartile:
            eventName = "thirdQuartile"
        case .completed:
            eventName = "completed"
        case .destroyed:
            eventName = "destroyed"
        @unknown default:
            print("Ad Event not sent: \(event)")
        }
        UnitySendMessage(kCallbackTarget, method, eventName.uppercased());
    }

    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        // Errors thrown related to ad rendering
        print("localizedDescription: \(error.localizedDescription)")
        print("    errorDescription: \(error.errorDescription)")
        print("       failureReason: \(error.failureReason)")
        print("  recoverySuggestion: \(error.recoverySuggestion)")
        UnitySendMessage(kCallbackTarget, "OnError", error.localizedDescription);
    }
}
