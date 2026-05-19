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
        let extensions = extensionsFromJsonString(thirdPartyDemand: thirdPartyJson)
        Nimbus.initialize(publisherKey: publisher, apiKey: apiKey)
        {
            NimbusManager.initAPS(appKey: extensions?.aps?.appKey ?? "")
            #if NIMBUS_ENABLE_MOBILEFUSE
            MobileFuseExtension()
            #endif
            if (extensions != nil) {
                #if NIMBUS_ENABLE_ADMOB
                AdMobExtension()
                #endif
                #if NIMBUS_ENABLE_INMOBI
                InMobiExtension(accountId: extensions?.inMobi?.accountId ?? "")
                #endif
                #if NIMBUS_ENABLE_META
                MetaExtension(appId: extensions?.meta?.appId ?? "", forceTestAd: extensions?.meta?.forceTestAd ?? false)
                #endif
                #if NIMBUS_ENABLE_MINTEGRAL
                let mintegral = extensions?.mintegral
                MintegralExtension(appId: mintegral?.appId ?? "",
                                    appKey: mintegral?.appKey ?? "")
                #endif
                #if NIMBUS_ENABLE_MOLOCO
                MolocoExtension(appKey: extensions?.moloco?.appKey ?? "")
                #endif
                #if NIMBUS_ENABLE_UNITY_ADS
                UnityExtension(gameId: extensions?.unityAds?.gameId ?? "")
                #endif
                #if NIMBUS_ENABLE_VUNGLE
                VungleExtension(appId: extensions?.vungle?.appId ?? "")
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
    
    @objc public func bannerAd(position: String, width: Int, height: Int, refreshInterval: Int, respectSafeArea: Bool, bannerPosition: Int, showAd: Bool, thirdPartyDemand: String) {
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: { @MainActor in
            do {
                #if NIMBUS_ENABLE_APS
                    var apsAds: [APSAd] = []
                if (!(extensions?.aps?.slotData?.isEmpty ?? false))
                {
                    for slot in extensions?.aps?.slotData ?? [] {
                        let bannerAdRequest = APSAdRequest(
                            slotUUID: slot?.slotId ?? "",
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        bannerAdRequest.setAdFormat(.banner)
                        do {
                            apsAds.append(try await bannerAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                }
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
                let bannerAd = Nimbus.bannerAd(position: position, size: AdSize(width: width, height: height), refreshInterval: refreshInterval){
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
    
    @objc public func interstitialAd(position: String, showAd: Bool, thirdPartyDemand: String){
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: {
            #if NIMBUS_ENABLE_APS
                var apsAds: [APSAd] = []
            if (!(extensions?.aps?.slotData?.isEmpty ?? false))
            {
                for slot in extensions?.aps?.slotData ?? [] {
                    if (slot?.adUnitType == .interstitialDisplay) {
                        let interstitialStaticAdRequest = APSAdRequest(
                            slotUUID: slot?.slotId ?? "",
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialStaticAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialStaticAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                    else if (slot?.adUnitType == .interstitialVideo) {
                        let interstitialVideoAdRequest = APSAdRequest(
                            slotUUID: slot?.slotId ?? "",
                            adNetworkInfo: .init(networkName: .nimbus)
                        )
                        interstitialVideoAdRequest.setAdFormat(.interstitial)
                        do {
                            apsAds.append(try await interstitialVideoAdRequest.loadAd())
                        } catch {
                            Nimbus.Log.request.error(error.localizedDescription)
                        }
                    }
                }
            }
            #endif
            var adMobAdUnitId: String = ""
            if let adUnitId = extensions?.adMob?.adUnitIds?.first {
                adMobAdUnitId = adUnitId ?? ""
            }
            let instanceId = self.adUnitInstanceId
            let interstitialAd = await Nimbus.interstitialAd(position: position){
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

        })
    }
    
    @objc public func rewardedAd(position: String, showAd: Bool, thirdPartyDemand: String) {
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
        let group = DispatchGroup()
        group.wait(for: {
            #if NIMBUS_ENABLE_APS
            var apsAds: [APSAd] = []
            if (!(extensions?.aps?.slotData?.isEmpty ?? false))
            {
                for slot in extensions?.aps?.slotData ?? [] {
                    let rewardedAdRequest = APSAdRequest(
                        slotUUID: slot?.slotId ?? "",
                        adNetworkInfo: .init(networkName: .nimbus)
                    )
                    rewardedAdRequest.setAdFormat(.rewardedVideo)
                    do {
                        apsAds.append(try await rewardedAdRequest.loadAd())
                    } catch {
                        Nimbus.Log.request.error(error.localizedDescription)
                    }
                }
            }
            #endif
            var adMobAdUnitId: String = ""
            if let adUnitId = extensions?.adMob?.adUnitIds?.first {
                adMobAdUnitId = adUnitId ?? ""
            }
            let instanceId = self.adUnitInstanceId
            let rewardedAd = await Nimbus.rewardedAd(position: position){
                demand {
                    #if NIMBUS_ENABLE_ADMOB
                        admob(rewardedAdUnitId: adMobAdUnitId)
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
    
    public static func didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError?) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceID, 
                "errorMessage": error?.localizedDescription ?? ""
            ]
        )
    }
    
    private static func extensionsFromJsonString(thirdPartyDemand: String) -> Extensions? {
        var extensions: Extensions?
        if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty) {
            do {
                if let dataFromString = thirdPartyDemand.data(using: .utf8) {
                    extensions = try JSONDecoder().decode(Extensions.self, from: dataFromString)
                }
            } catch {
                NimbusManager.didReceiveNimbusError(
                    adUnitInstanceID: 0,
                    error: .unitysdk(stage: .request, detail: "Failed to decode third party json: \(error)")
                )
            }
        }
        return extensions
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


struct Extensions: Codable {
    let aps: Aps?
    let adMob: AdMob?
    let inMobi: InMobi?
    let meta: Meta?
    let mintegral: Mintegral?
    let mobileFuse: MobileFuse?
    let moloco: Moloco?
    let unityAds: UnityAds?
    let vungle: Vungle?
}

extension Extensions {
    
    struct AdMob: Codable {
        let adUnitIds: [String?]?
    }
    
    struct Aps: Codable {
        let appKey: String?
        let slotData: [ApsSlotData?]?
    }
    
    struct ApsSlotData: Codable {
        let slotId: String?
        let adUnitType: APSAdUnitType?
    }
    
    public enum APSAdUnitType: Int, Codable {
        case display320X50
        case display300X250
        case display728X90
        case interstitialDisplay
        case interstitialVideo
        case rewardedVideo
    }
    
    struct InMobi: Codable {
        let accountId: String?
    }
    
    struct Meta: Codable {
        let appId: String?
        let forceTestAd: Bool
    }
    
    struct Mintegral: Codable {
        let appId: String?
        let appKey: String?
    }
    
    struct MobileFuse: Codable {
    }
    
    struct Moloco: Codable {
        let appKey: String?
    }
    
    struct UnityAds: Codable {
        let gameId: String?
    }
    
    struct Vungle: Codable {
        let appId: String?
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

extension NimbusError.Domain {
    static let unitysdk = Self(rawValue: "unitysdk")
}

extension NimbusError {
    static func unitysdk(reason: Reason = .failure, stage: Stage, detail: String? = nil) -> NimbusError {
        NimbusError(reason: reason, domain: .unitysdk, stage: stage, detail: detail)
    }
}
