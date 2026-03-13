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
import NimbusAdCache
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
    
    var bannerAd: InlineAd?
    var interstitialAd: InterstitialAd?
    var rewardedAd: RewardedAd?
        
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
        enableSDKInTestMode: Bool
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
    
    @objc public func bannerAd(position: String, size: Int, refreshInterval: Int, respectSafeArea: Bool, bannerPosition: Int, showAd: Bool) -> String {                
        let group = DispatchGroup()
        group.wait(for: {
            do {
                let uniqueKey = UUID().uuidString
                var bannerAd = try await Nimbus.bannerAd(position: position, size: .banner, refreshInterval: 30).onEvent { event in
                    didReceiveNimbusEvent(adUnitInstanceID: adUnitInstanceID, event: event, adId: uniqueKey)
                }                .onError { error in
                    didReceiveNimbusError(adUnitInstanceID: adUnitInstanceID, nimbusError: error, adId: uniqueKey)
                }
                NimbusAdCache.shared.addAd(bannerAd, uniqueKey)
                if (showAd) {
                    let contentView = UIView()
                    contentView.translatesAutoresizingMaskIntoConstraints = false
                    viewController.view.addSubview(contentView)
                    NSLayoutConstraint.activate(constraints(to: contentView, viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
                    bannerAd.show(in: contentView)
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
                } else {
                    bannerAd.load()
                }
                return uniqueKey
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
                return ""
            }
        })
    }
    
    @objc public func interstitialAd(position: String, showAd: Bool) -> String {
        let group = DispatchGroup()
        group.wait(for: {
            do {
                let uniqueKey = UUID().uuidString
                var interstitialAd = try await Nimbus.interstitialAd(position:"unity_test").onEvent { event in
                    didReceiveNimbusEvent(adUnitInstanceID: adUnitInstanceID, event: event, adId: uniqueKey)
                }                .onError { error in
                    didReceiveNimbusError(adUnitInstanceID: adUnitInstanceID, nimbusError: error, adId: uniqueKey)
                }
                NimbusAdCache.shared.addAd(interstitialAd, uniqueKey)
                if (showAd) {
                    interstitialAd.show()
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
                } else {
                    interstitialAd.load()
                }
                return uniqueKey
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
                return ""
            }
        })
    }
    
    @objc public func rewardedAd(position: String, showAd: Bool) -> String {
        let group = DispatchGroup()
        group.wait(for: {
            do {
                let uniqueKey = UUID().uuidString
                var rewardedAd = try await Nimbus.RewardedAd(position:"unity_test").onEvent { event in
                    didReceiveNimbusEvent(adUnitInstanceID: adUnitInstanceID, event: event, adId: uniqueKey)
                }                .onError { error in
                    didReceiveNimbusError(adUnitInstanceID: adUnitInstanceID, nimbusError: error, adId: uniqueKey)
                }
                NimbusAdCache.shared.addAd(rewardedAd, uniqueKey)
                if (showAd) {
                    rewardedAd.show()
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
                } else {
                    rewardedAd.load()
                }
                return uniqueKey
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
                return ""
            }
        })
    }
    
    @objc public func showAd(adId: String, respectSafeArea: Bool, bannerPosition: Int) {
        let ad = NimbusAdCache.shared.getAd(forKey: adId)
        if (ad.type == .inline) {
            let contentView = UIView()
            contentView.translatesAutoresizingMaskIntoConstraints = false
            viewController.view.addSubview(contentView)
            NSLayoutConstraint.activate(constraints(to: contentView, viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
            ad.show(in: contentView)
        } else {
            ad.show()
        }
        UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
    }
    
    public static func didReceiveNimbusEvent(adUnitInstanceID: String, event: NimbusEvent, adId: String) {
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
            NimbusAdCache.shared.removeAd(forKey: adId)
        case .endCardImpression:
            eventName = "END_CARD_IMPRESSION"
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        
        UnityBinding.sendMessage(
            methodName: "OnAdEvent",
            params: [
                "adUnitId": adId,
                "adUnitInstanceID": adUnitInstanceID, 
                "eventName": eventName
            ]
        )
    }
    
    public static func didReceiveNimbusError(adUnitInstanceID: String, error: NimbusError, adId: String) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitId": adId,
                "adUnitInstanceID": adUnitInstanceID, 
                "errorMessage": error.localizedDescription
            ]
        )
        NimbusAdCache.shared.removeAd(forKey: adId)
    }
    
    private func constraints(to contentView: UIView, viewController: UIViewController,respectSafeArea: Bool, adScreenPosition: Int) -> [NSLayoutConstraint] {
        switch (position) {
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
        bannerAd = nil;
        interstitialAd = nil;
        rewardedAd = nil;
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
