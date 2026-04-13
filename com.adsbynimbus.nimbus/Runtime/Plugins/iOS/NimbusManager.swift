//
//
//  NimbusManager.swift
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2025 AdsByNimbus. All rights reserved.
//

import Foundation
import NimbusKit
import AppTrackingTransparency
import AdSupport
#if NIMBUS_ENABLE_APS
import NimbusRequestAPSKit
import DTBiOSSDK
#endif
#if NIMBUS_ENABLE_VUNGLE
import VungleAdsSDK
#endif
#if NIMBUS_ENABLE_META
import FBAudienceNetwork
#endif
#if NIMBUS_ENABLE_ADMOB
import GoogleMobileAds
#endif
#if NIMBUS_ENABLE_MINTEGRAL
import MTGSDK
import MTGSDKBidding
#endif
#if NIMBUS_ENABLE_UNITY_ADS
import UnityAds
#endif
#if NIMBUS_ENABLE_LIVERAMP
import NimbusLiveRampKit
import LRAtsSDK
#endif
#if NIMBUS_ENABLE_MOLOCO
import MolocoSDK
#endif
#if NIMBUS_ENABLE_INMOBI
import InMobiSDK
#endif


@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let adUnitInstanceId: Int
    
    var ad: Ad?
    
        
    #if NIMBUS_ENABLE_APS
    private static var apsRequestHelper: NimbusAPSRequestHelper?
    #endif
    
    #if NIMBUS_ENABLE_LIVERAMP
    private static var liveRampInterceptor: NimbusLiveRampInterceptor?
    private static var liveRampEmail: String?
    private static var liveRampPhone: String?
    #endif
 
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool,
        enableSDKInTestMode: Bool,
        thirdPartyJson: String
    ) {
        Nimbus.initialize(publisher: publisher, apiKey: apiKey)
        Nimbus.configuration.testMode = enableSDKInTestMode
    }
    
    @objc public class func nimbusManager(forAdUnityInstanceId adUnityInstanceId: Int) -> NimbusManager {
        guard let manager = managerDictionary[adUnityInstanceId] else {
            let manager = NimbusManager(adUnitInstanceId: adUnityInstanceId)
            managerDictionary[adUnityInstanceId] = manager
            return manager
        }
        return manager
    }
    
    #if NIMBUS_ENABLE_LIVERAMP
        @objc public class func initializeLiveRamp(configId: String, email: String, phoneNumber: String, isTestMode: Bool,
                                               hasConsentForNoLegislation: Bool) {
            if (phoneNumber != "") {
                liveRampInterceptor = NimbusLiveRampInterceptor(configId: configId, phoneNumber: phoneNumber, hasConsentForNoLegislation: hasConsentForNoLegislation, isTestMode: isTestMode)
                liveRampPhone = phoneNumber
            } else {
                liveRampInterceptor = NimbusLiveRampInterceptor(configId: configId, email: email, hasConsentForNoLegislation: hasConsentForNoLegislation, isTestMode: isTestMode)
                liveRampEmail = email
            }
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
    
    @objc public func bannerAd(position: String, width: Int, height: Int, refreshInterval: Int, respectSafeArea: Bool, bannerPosition: Int, showAd: Bool){                
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            do {
                let contentView = UIView()
                let viewController = self.unityViewController() ?? UIViewController()
                contentView.translatesAutoresizingMaskIntoConstraints = false
                viewController.view.addSubview(contentView)
                NSLayoutConstraint.activate(self.constraints(to: contentView, viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
                let instanceId = self.adUnitInstanceId
                let bannerAd = try Nimbus.bannerAd(position: position, size: AdSize(width: width, height: height), refreshInterval: 30).onEvent { event in
                    NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
                }                .onError { error in
                    NimbusManager.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
                }
                if (showAd) {
                    try await bannerAd.show(in: contentView)
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                } else {
                    try await bannerAd.fetch()
                }
                self.ad = bannerAd
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
            }
        })
    }
    
    @objc public func interstitialAd(position: String, showAd: Bool){
        let group = DispatchGroup()
        group.wait(for: {
            do {
                let instanceId = self.adUnitInstanceId
                let interstitialAd = try await Nimbus.interstitialAd(position: position).onEvent { event in
                    NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
                }                .onError { error in
                    NimbusManager.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
                }
                do {
                    if (showAd) {
                        try await interstitialAd.show(from: self.unityViewController())
                        UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                    } else {
                        try await interstitialAd.load()
                    }
                    self.ad = interstitialAd
                }
                catch {
                    Nimbus.Log.ad.error(error.localizedDescription)
                }
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
            }
        })
    }
    
    @objc public func rewardedAd(position: String, showAd: Bool) {
        let group = DispatchGroup()
        group.wait(for: {
            do {
                let instanceId = self.adUnitInstanceId
                let rewardedAd = try await Nimbus.rewardedAd(position: position).onEvent { event in
                    NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
                }                .onError { error in
                    NimbusManager.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
                }
                do {
                    if (showAd) {
                        try await rewardedAd.show(from: self.unityViewController())
                        UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                    } else {
                        try await rewardedAd.load()
                    }
                    self.ad = rewardedAd
                } catch {
                    Nimbus.Log.ad.error(error.localizedDescription)
                }
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
            }
        })
    }
    
    @objc public func showAd(respectSafeArea: Bool, bannerPosition: Int) {
        let group = DispatchGroup()
        let instanceId = self.adUnitInstanceId
        if let inlineAd = ad as? InlineAd {
            group.wait(for: { @MainActor in
                do {
                    var contentView = UIView()
                    let viewController = self.unityViewController() ?? UIViewController()
                    contentView.translatesAutoresizingMaskIntoConstraints = false
                    viewController.view.addSubview(contentView)
                    NSLayoutConstraint.activate(self.constraints(to: contentView , viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
                    try await inlineAd.show(in: contentView)
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                } catch {
                    Nimbus.Log.ad.error(error.localizedDescription)
                }
            })
        }
        else if let fullscreenAd = ad as? FullscreenAd {
            group.wait(for: {
                do {
                    try await fullscreenAd.show(from: self.unityViewController())
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                } catch {
                    Nimbus.Log.ad.error(error.localizedDescription)
                }
            })
        } else {
            UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId, 
            "errorMessage": "Attempted to call show() on an invalid ad type"])
            Nimbus.Log.ad.error("Attempted to show invalid ad type.")
        }
        
    }
    
    public static func didReceiveNimbusEvent(adUnitInstanceID: Int, event: NimbusEvent) {
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
        case .endCardImpression:
            eventName = "END_CARD_IMPRESSION"
        @unknown default:
            Nimbus.Log.ad.error("Ad Event not sent: \(event)")
            return
        }
        
        UnityBinding.sendMessage(
            methodName: "OnAdEvent",
            params: [
                "adUnitInstanceID": adUnitInstanceID, 
                "eventName": eventName
            ]
        )
    }
    
    public static func didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceID, 
                "errorMessage": error.localizedDescription
            ]
        )
    }
    
    private func constraints(to contentView: UIView, viewController: UIViewController,respectSafeArea: Bool, adScreenPosition: Int) -> [NSLayoutConstraint] {
        switch (adScreenPosition) {
            // Center Top
            case 1:
                return [
                    contentView.centerXAnchor.constraint(equalTo: viewController.view.centerXAnchor),
                    contentView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor(respectSafeArea)),
                    contentView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor(respectSafeArea)),
                    contentView.topAnchor.constraint(equalTo: viewController.view.topAnchor(respectSafeArea))
                ]
            // Center
            case 2:
                return [
                    contentView.centerXAnchor.constraint(equalTo: viewController.view.centerXAnchor),
                    contentView.centerYAnchor.constraint(equalTo: viewController.view.centerYAnchor)
                ]
            // Bottom Left
            case 3:
                return [
                    contentView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor(respectSafeArea)),
                    contentView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor(respectSafeArea))
                ]
            // Bottom Right
            case 4:
                return [
                    contentView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor(respectSafeArea)),
                    contentView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor(respectSafeArea))
                ]
            // Top Left
            case 5:
                return [
                    contentView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor(respectSafeArea)),
                    contentView.topAnchor.constraint(equalTo: viewController.view.topAnchor(respectSafeArea))
                ]
            // Top Right
            case 6:
                return [
                    contentView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor(respectSafeArea)),
                    contentView.topAnchor.constraint(equalTo: viewController.view.topAnchor(respectSafeArea))
                ]
            // Center Bottom (Case 0)
            default:
                return [
                    contentView.centerXAnchor.constraint(equalTo: viewController.view.centerXAnchor),
                    contentView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor(respectSafeArea)),
                    contentView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor(respectSafeArea)),
                    contentView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor(respectSafeArea))
                ]
        }
    }
    
    @objc public func destroyExistingAd() {
        ad = nil;
        removeReferenceFromManagerDictionary()
    }
    
    private func removeReferenceFromManagerDictionary() {
        NimbusManager.managerDictionary.removeValue(forKey: adUnitInstanceId)
    }
}

extension UIView {
    func leadingAnchor(_ respectSafeArea: Bool) -> NSLayoutXAxisAnchor {
        respectSafeArea ? safeAreaLayoutGuide.leadingAnchor : leadingAnchor
    }
    func trailingAnchor(_ respectSafeArea: Bool) -> NSLayoutXAxisAnchor {
        respectSafeArea ? safeAreaLayoutGuide.trailingAnchor : trailingAnchor
    }
    func bottomAnchor(_ respectSafeArea: Bool) -> NSLayoutYAxisAnchor {
        respectSafeArea ? safeAreaLayoutGuide.bottomAnchor : bottomAnchor
    }
    func topAnchor(_ respectSafeArea: Bool) -> NSLayoutYAxisAnchor {
        respectSafeArea ? safeAreaLayoutGuide.topAnchor : topAnchor
    }
}

extension DispatchGroup {
    func wait(for task: @escaping () async throws -> Void) {
        enter()
        
        Task {
            defer { self.leave() }
            try await task()
        }
        
        _ = wait(timeout: .now() + 0.5)
    }
}
