//
//
//  NimbusManager.swift
//
//  Created by Jonathan Sligh on 5/7/26.
//  Copyright © 2026 AdsByNimbus. All rights reserved.
//

import Foundation
import NimbusKit
import AppTrackingTransparency
import AdSupport
#if NIMBUS_ENABLE_APS
import NimbusAPSKit
@preconcurrency import DTBiOSSDK
#endif
#if NIMBUS_ENABLE_VUNGLE
import NimbusVungleKit
#endif
#if NIMBUS_ENABLE_META
import FBAudienceNetwork
import NimbusMetaKit
#endif
#if NIMBUS_ENABLE_ADMOB
import GoogleMobileAds
import NimbusAdMobKit
#endif
#if NIMBUS_ENABLE_MINTEGRAL
import NimbusMintegralKit
#endif
#if NIMBUS_ENABLE_UNITY_ADS
import NimbusUnityKit
#endif
#if NIMBUS_ENABLE_LIVERAMP
import NimbusLiveRampKit
#endif
#if NIMBUS_ENABLE_MOLOCO
import NimbusMolocoKit
#endif
#if NIMBUS_ENABLE_INMOBI
import NimbusInMobiKit
#endif
#if NIMBUS_ENABLE_MOBILEFUSE
import NimbusMobileFuseKit
#endif


@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let adUnitInstanceId: Int
    
    var ad: Ad?
    
    // MARK: - Class Functions
    
    @objc public static func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool,
        enableSDKInTestMode: Bool,
        thirdPartyJson: String
    ) {
        guard let extensions = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: thirdPartyJson) else {
            return
        }
        Nimbus.initialize(publisherKey: publisher, apiKey: apiKey)
        {
            NimbusManager.initAPS(appKey: extensions.aps?.appKey ?? "")
            #if NIMBUS_ENABLE_MOBILEFUSE
            MobileFuseExtension()
            #endif
            #if NIMBUS_ENABLE_ADMOB
            AdMobExtension()
            #endif
            #if NIMBUS_ENABLE_INMOBI
            InMobiExtension(accountId: extensions.inMobi?.accountId ?? "")
            #endif
            #if NIMBUS_ENABLE_META
            MetaExtension(appId: extensions.meta?.appId ?? "", forceTestAd: extensions.meta?.forceTestAd ?? false)
            #endif
            #if NIMBUS_ENABLE_MINTEGRAL
            let mintegral = extensions.mintegral
            MintegralExtension(appId: mintegral?.appId ?? "",
                                appKey: mintegral?.appKey ?? "")
            #endif
            #if NIMBUS_ENABLE_MOLOCO
            MolocoExtension(appKey: extensions.moloco?.appKey ?? "")
            #endif
            #if NIMBUS_ENABLE_UNITY_ADS
            UnityExtension(gameId: extensions.unityAds?.gameId ?? "")
            #endif
            #if NIMBUS_ENABLE_VUNGLE
            VungleExtension(appId: extensions.vungle?.appId ?? "")
            #endif
        }
        Nimbus.configuration.testMode = enableSDKInTestMode
    }
    
    @objc private class func initAPS(appKey: String) {
        #if NIMBUS_ENABLE_APS
        if (appKey != "") {
            DTBAds.sharedInstance().setAppKey(appKey)
            DTBAds.sharedInstance().mraidPolicy = CUSTOM_MRAID
            DTBAds.sharedInstance().mraidCustomVersions = ["1.0", "2.0", "3.0"]
            DTBAds.sharedInstance().testMode = Nimbus.configuration.testMode
            DTBAds.sharedInstance().setLogLevel(DTBLogLevelDebug)
            DTBAds.sharedInstance().setAPSPublisherExtendedIdFeatureEnabled(true)
        }
        #endif
    }
    
    #if NIMBUS_ENABLE_LIVERAMP
    @objc public class func initializeLiveRamp(configId: String, email: String, hasConsentForNoLegislation: Bool = true, testMode: Bool = false) {
        let liveRamp = LiveRamp(
            configId: configId,
            email: email,
            hasConsentForNoLegislation: hasConsentForNoLegislation
        )

        // Applies LiveRamp to all future Nimbus requests
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            do {
                try await liveRamp.fetchEnvelope(isTestMode: testMode).applyToNimbus()
            } catch {
                Nimbus.Log.lifecycle.error(error.localizedDescription)
            }
        })
    }
    #endif
    
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
        UnityFramework.getInstance().appController().rootViewController
    }
    
    // MARK: - Public Functions
    
    @objc public func bannerAd(position: String, width: Int, height: Int, refreshInterval: Int, bidFloor: Float, 
    respectSafeArea: Bool, bannerPosition: Int, showAd: Bool, thirdPartyDemand: String, requestModifiersJson: String) {
        let extensions = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            do {
                #if NIMBUS_ENABLE_APS
                    let apsAds = await self.loadAPSAds(from: extensions)
                #endif
                let contentView = UIView()
                let viewController = self.unityViewController() ?? UIViewController()
                contentView.translatesAutoresizingMaskIntoConstraints = false
                viewController.view.addSubview(contentView)
                NSLayoutConstraint.activate(self.constraints(to: contentView, viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
                let instanceId = self.adUnitInstanceId
                var adMobAdUnitId: String = ""
                if let adUnitId = extensions?.adMob?.adUnitIds?.first {
                    adMobAdUnitId = adUnitId ?? ""
                }
                let bannerAd = Nimbus.bannerAd(position: position, size: AdSize(width: width, height: height), bidFloor: bidFloor, refreshInterval: refreshInterval){
                    demand {
                        #if NIMBUS_ENABLE_ADMOB
                        if (!adMobAdUnitId.isEmpty) {
                            admob(bannerAdUnitId: adMobAdUnitId)
                        }
                        #endif
                        #if NIMBUS_ENABLE_APS
                        if (!apsAds.isEmpty) {
                            aps(ads: apsAds)
                        }
                        #endif
                    }
                    if let modifiers = NimbusHelper.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                        modifiers.components
                    }
                }.onEvent { event in
                    NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
                }                .onError { error in
                    NimbusHelper.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
                }
                if (showAd) {
                    try await bannerAd.show(in: contentView)
                    UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": instanceId])
                } else {
                    try await bannerAd.load()
                }
                self.ad = bannerAd
            } catch {
                Nimbus.Log.request.error(error.localizedDescription)
            }
        })
    }
    
    @objc public func interstitialAd(position: String, bannerFloor: Float, videoFloor: Float, showAd: Bool, thirdPartyDemand: String, requestModifiersJson: String){
        let extensions = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: {
            #if NIMBUS_ENABLE_APS
                var apsAds = await self.loadAPSAds(from: extensions)
            #endif
            var adMobAdUnitId: String = ""
            if let adUnitId = extensions?.adMob?.adUnitIds?.first {
                adMobAdUnitId = adUnitId ?? ""
            }
            let instanceId = self.adUnitInstanceId
            let interstitialAd = await Nimbus.fullscreenAd(position: position){
                demand {
                    #if NIMBUS_ENABLE_ADMOB
                    if (!adMobAdUnitId.isEmpty) {
                        admob(interstitialAdUnitId: adMobAdUnitId)
                    }
                    #endif
                    #if NIMBUS_ENABLE_APS
                    if (!apsAds.isEmpty) {
                        aps(ads: apsAds)
                    }
                    #endif
                }
                if let modifiers = NimbusHelper.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                    modifiers.components
                }
            }.onEvent { event in
                NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
            }                .onError { error in
                NimbusHelper.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
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
                NimbusHelper.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
            }

        })
    }
    
    @objc public func rewardedAd(position: String, bidFloor: Float, showAd: Bool, thirdPartyDemand: String, requestModifiersJson: String) {
        let extensions = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: {
            #if NIMBUS_ENABLE_APS
                let apsAds = await self.loadAPSAds(from: extensions)
            #endif
            var adMobAdUnitId: String = ""
            if let adUnitId = extensions?.adMob?.adUnitIds?.first {
                adMobAdUnitId = adUnitId ?? ""
            }
            let instanceId = self.adUnitInstanceId
            let rewardedAd = await Nimbus.rewardedAd(position: position, bidFloor: bidFloor){
                demand {
                    #if NIMBUS_ENABLE_ADMOB
                    if (!adMobAdUnitId.isEmpty) {
                        admob(rewardedAdUnitId: adMobAdUnitId)
                    }
                    #endif
                    #if NIMBUS_ENABLE_APS
                    if (!apsAds.isEmpty) {
                        aps(ads: apsAds)
                    }
                    #endif
                }
                if let modifiers = NimbusHelper.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                    modifiers.components
                }
            }.onEvent { event in
                NimbusManager.didReceiveNimbusEvent(adUnitInstanceID: instanceId, event: event)
            }                .onError { error in
                NimbusHelper.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
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
        })
    }
    
    @objc public func showAd(respectSafeArea: Bool, bannerPosition: Int) {
        let group = DispatchGroup()
        let instanceId = self.adUnitInstanceId
        if let inlineAd = ad as? InlineAd {
            group.wait(for: { @MainActor in
                do {
                    let contentView = UIView()
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
    #if NIMBUS_ENABLE_APS
    private func loadAPSAds(from extensions: Extensions?) async -> [APSAd] {
        var apsAds: [APSAd] = []
        if (!(extensions?.aps?.slotData?.isEmpty ?? false)) {
            for slot in extensions?.aps?.slotData ?? [] {
                if let uuid = slot?.slotId {
                    if (uuid.isEmpty) {
                        continue
                    }
                    switch (slot?.adUnitType) {
                    case .display300X250,.display320X50,.display728X90:
                        let bannerAdRequest = APSAdRequest(
                            slotUUID: uuid,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        bannerAdRequest.setAdFormat(.banner)
                        do {
                            apsAds.append(try await bannerAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    case .interstitialDisplay:
                        let interstitialStaticAdRequest = APSAdRequest(
                            slotUUID: uuid,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialStaticAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialStaticAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    case .interstitialVideo:
                        let interstitialVideoAdRequest = APSAdRequest(
                            slotUUID: uuid,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialVideoAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialVideoAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    case .rewardedVideo:
                        let rewardedAdRequest = APSAdRequest(
                            slotUUID: uuid,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        rewardedAdRequest.setAdFormat(.rewardedVideo)
                        do {
                            apsAds.append(try await rewardedAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    default:
                        continue
                    }
                }
            }
        }
        return apsAds
    }
    #endif
    
    public static func didReceiveNimbusEvent(adUnitInstanceID: Int, event: AdEvent) {
        let eventName: String
        switch event {
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
        default:
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
        ad?.destroy()
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
