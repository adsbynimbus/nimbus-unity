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
    
    @objc public func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool,
        enableSDKInTestMode: Bool,
        thirdPartyJson: String
    ) {
        var thirdPartyDemand: [ThirdPartyDemand] = []

        if (thirdPartyJson != "" && !thirdPartyJson.isEmpty) {
            do {
                if let dataFromString = thirdPartyJson.data(using: .utf8) {
                    thirdPartyDemand = try JSONDecoder().decode([ThirdPartyDemand].self, from: dataFromString)
                }
            } catch {
                Nimbus.Log.lifecycle.error(error.localizedDescription)
                var adUnitInstance = self.adUnitInstanceId
                NimbusManager.didReceiveNimbusError(adUnitInstanceID: adUnitInstance, error: nil, errorMessage: "Failed to decode third party json")
                
            }
        }
        Nimbus.initialize(publisherKey: publisher, apiKey: apiKey)
        {
            NimbusManager.initAPS(appKey: thirdPartyDemand.first(where: {$0.demandType == .Aps})?.firstKey ?? "")
            #if NIMBUS_ENABLE_MOBILEFUSE
            MobileFuseExtension()
            #endif
            if (!thirdPartyDemand.isEmpty) {
                #if NIMBUS_ENABLE_ADMOB
                AdMobExtension(autoInitialize: thirdPartyDemand.first(where: {$0.demandType == .AdMob})?.autoInit ?? false)
                #endif
                #if NIMBUS_ENABLE_INMOBI
                InMobiExtension(accountId: thirdPartyDemand.first(where: {$0.demandType == .InMobi})?.firstKey)
                #endif
                #if NIMBUS_ENABLE_META
                MetaExtension(appId: thirdPartyDemand.first(where: {$0.demandType == .Meta})?.firstKey ?? "", forceTestAd: thirdPartyDemand.first(where: {$0.demandType == .Meta})?.testMode ?? false)
                #endif
                #if NIMBUS_ENABLE_MINTEGRAL
                let mintegral = thirdPartyDemand.first(where: {$0.demandType == .Mintegral})
                MintegralExtension(appId: mintegral?.firstKey ?? "",
                                    appKey: mintegral?.secondKey ?? "")
                #endif
                #if NIMBUS_ENABLE_MOLOCO
                MolocoExtension(appKey: thirdPartyDemand.first(where: {$0.demandType == .Moloco})?.firstKey ?? "")
                #endif
                #if NIMBUS_ENABLE_UNITY_ADS
                UnityExtension(gameId: thirdPartyDemand.first(where: {$0.demandType == .UnityAds})?.firstKey ?? "")
                #endif
                #if NIMBUS_ENABLE_VUNGLE
                VungleExtension(appId: thirdPartyDemand.first(where: {$0.demandType == .Vungle})?.firstKey ?? "")
                #endif
            }
        }
        Nimbus.configuration.testMode = enableSDKInTestMode
    }
    
    #if NIMBUS_ENABLE_APS
    @objc private class func initAPS(appKey: String) {
        #if NIMBUS_ENABLE_APS
        if (appKey != ""){
            DTBAds.sharedInstance().setAppKey(appKey)
            DTBAds.sharedInstance().mraidPolicy = CUSTOM_MRAID
            DTBAds.sharedInstance().mraidCustomVersions = ["1.0", "2.0", "3.0"]
            DTBAds.sharedInstance().testMode = Nimbus.configuration.testMode
            DTBAds.sharedInstance().setLogLevel(DTBLogLevelDebug)
            DTBAds.sharedInstance().setAPSPublisherExtendedIdFeatureEnabled(true)
        }
        #endif
    }
    #endif
    
    #if NIMBUS_ENABLE_ADMOB
    @objc public static func initAdMob() {
        MobileAds.shared.start()
    }
    #endif
    
    #if NIMBUS_ENABLE_LIVERAMP
    @objc public class func initializeLiveRamp(configId: String, email: String, hasConsentForNoLegislation: Bool = true) {
        let liveRamp = LiveRamp(
            configId: configId,
            email: email,
            hasConsentForNoLegislation: hasConsentForNoLegislation
        )

        // Applies LiveRamp to all future Nimbus requests
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            try await liveRamp.fetchEnvelope().applyToNimbus()
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
    
    @objc public func bannerAd(position: String, width: Int, height: Int, refreshInterval: Int, respectSafeArea: Bool, bannerPosition: Int, showAd: Bool, apsAdUnitId: String, adMobAdUnitId: String){
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            do {
                #if NIMBUS_ENABLE_APS
                    var apsAds: [APSAd] = []
                    if (apsAdUnitId != "")
                    {
                        let bannerAdRequest = APSAdRequest(
                            slotUUID: apsAdUnitId,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        bannerAdRequest.setAdFormat(.banner)
                        do {
                            apsAds.append(try await bannerAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                #endif
                let contentView = UIView()
                let viewController = self.unityViewController() ?? UIViewController()
                contentView.translatesAutoresizingMaskIntoConstraints = false
                viewController.view.addSubview(contentView)
                NSLayoutConstraint.activate(self.constraints(to: contentView, viewController: viewController, respectSafeArea: respectSafeArea, adScreenPosition: bannerPosition))
                let instanceId = self.adUnitInstanceId
                let bannerAd = try Nimbus.bannerAd(position: position, size: AdSize(width: width, height: height), refreshInterval: refreshInterval){
                    demand {
                        #if NIMBUS_ENABLE_ADMOB
                        admob(bannerAdUnitId: adMobAdUnitId)
                        #endif
                        #if NIMBUS_ENABLE_APS
                        aps(ads: apsAds)
                        #endif
                    }
                }.onEvent { event in
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
    
    @objc public func interstitialAd(position: String, showAd: Bool, apsStaticAdUnitId: String,  apsVideoAdUnitId: String, adMobAdUnitId: String){
        let group = DispatchGroup()
        group.wait(for: {
            do {
                #if NIMBUS_ENABLE_APS
                    var apsAds: [APSAd] = []
                    if (apsStaticAdUnitId != "") {
                        let interstitialStaticAdRequest = APSAdRequest(
                            slotUUID: apsStaticAdUnitId,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialStaticAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialStaticAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                    if (apsVideoAdUnitId != "") {
                        let interstitialVideoAdRequest = APSAdRequest(
                            slotUUID: apsVideoAdUnitId,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialVideoAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialVideoAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                #endif
                let instanceId = self.adUnitInstanceId
                let interstitialAd = try await Nimbus.interstitialAd(position: position){
                    demand {
                        #if NIMBUS_ENABLE_ADMOB
                            admob(bannerAdUnitId: adMobAdUnitId)
                        #endif
                        #if NIMBUS_ENABLE_APS
                            aps(ads: apsAds)
                        #endif
                    }
                }.onEvent { event in
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
    
    @objc public func rewardedAd(position: String, showAd: Bool, apsAdUnitId: String, adMobAdUnitId: String) {
        let group = DispatchGroup()
        group.wait(for: {
            do {
                #if NIMBUS_ENABLE_APS
                    var apsAds: [APSAd] = []
                    if (apsAdUnitId != "")
                    {
                        let rewardedAdRequest = APSAdRequest(
                            slotUUID: apsAdUnitId,
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        rewardedAdRequest.setAdFormat(.rewardedVideo)
                        do {
                            apsAds.append(try await rewardedAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                #endif
                let instanceId = self.adUnitInstanceId
                let rewardedAd = try await Nimbus.rewardedAd(position: position){
                    demand {
                        #if NIMBUS_ENABLE_ADMOB
                            admob(bannerAdUnitId: adMobAdUnitId)
                        #endif
                        #if NIMBUS_ENABLE_APS
                            aps(ads: apsAds)
                        #endif
                    }
                }.onEvent { event in
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
    
    public static func didReceiveNimbusEvent(adUnitInstanceID: Int, event: AdEvent) {
        let eventName: String
        switch event {
        case .loaded, .firstQuartile, .midpoint, .thirdQuartile, .skipped:
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
    
    public static func didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError?, errorMessage: String = "") {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceID, 
                "errorMessage": error?.localizedDescription ?? errorMessage
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
    
    enum ThirdPartyDemandEnum: Int, Codable
    {
        case AdMob = 0
        case Aps = 1
        case InMobi = 2
        case Meta = 3
        case Mintegral = 4
        case MobileFuse = 5
        case Moloco = 6
        case UnityAds = 7
        case Vungle = 8
    }
    
    struct ThirdPartyDemand: Codable {
        var demandType: ThirdPartyDemandEnum = .AdMob
        var firstKey: String?
        var secondKey: String?
        var testMode: Bool = false
        var autoInit: Bool = false
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
