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
        guard let extensions = extensionsFromJsonString(thirdPartyDemand: thirdPartyJson) else {
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
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
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
                    if let modifiers = NimbusManager.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                        modifiers.components
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
    
    @objc public func interstitialAd(position: String, bannerFloor: Float, videoFloor: Float, showAd: Bool, thirdPartyDemand: String, requestModifiersJson: String){
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
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
                if let modifiers = NimbusManager.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                    modifiers.components
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
                NimbusManager.didReceiveNimbusError(adUnitInstanceID: instanceId, error: error)
            }

        })
    }
    
    @objc public func rewardedAd(position: String, bidFloor: Float, showAd: Bool, thirdPartyDemand: String, requestModifiersJson: String) {
        let extensions = NimbusManager.extensionsFromJsonString(thirdPartyDemand: thirdPartyDemand)
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
                if let modifiers = NimbusManager.requestModifiersFromJsonString(requestModifiers: requestModifiersJson) {
                    modifiers.components
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
    
    public static func didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceID,
                "errorMessage": error.localizedDescription
            ]
        )
    }
    
    public static func didReceiveNimbusError(adUnitInstanceID: Int, error: Error) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceID,
                "errorMessage": error.localizedDescription
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
    
    private static func requestModifiersFromJsonString(requestModifiers: String) -> RequestModifiers? {
        var modifiers: RequestModifiers?
        if (requestModifiers != "" && !requestModifiers.isEmpty) {
            do {
                if let dataFromString = requestModifiers.data(using: .utf8) {
                    modifiers = try JSONDecoder().decode(RequestModifiers.self, from: dataFromString)
                }
            } catch {
                NimbusManager.didReceiveNimbusError(
                    adUnitInstanceID: 0,
                    error: .unitysdk(stage: .request, detail: "Failed to decode request modifiers json: \(error)")
                )
            }
        }
        return modifiers
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

protocol UnityRequestComponent {
    @MainActor var requestComponent: any RequestComponent { get }
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


struct RequestModifiers: Codable, Sendable {
    let app: PerRequestApp?
    let banner: BannerCreative?
    let environment: Env?
    let location: Location?
    let userKeywords: String?
    let video: VideoCreative?
    let viewability: Viewability?
    
    @MainActor
    var components: [RequestComponent] {
        var requestComponents: [RequestComponent] = []
        if let perRequestApp = app {
            requestComponents.append(perRequestApp.requestComponent)
        }
        if let bannerCreative = banner {
            requestComponents.append(bannerCreative.requestComponent)
        }
        if let env = environment {
            requestComponents.append(env.requestComponent)
        }
        if let loc = location {
            requestComponents.append(loc.requestComponent)
        }
        if let userKeywords = userKeywords {
            requestComponents.append(user(keywords: userKeywords))
        }
        if let vid = video {
            requestComponents.append(vid.requestComponent)
        }
        if let v = viewability {
            requestComponents.append(v.requestComponent)
        }
        return requestComponents
    }
}

struct BannerCreative: Codable, UnityRequestComponent {
    let width: Int?
    let height: Int?
    let bidFloor: Float?
    
    let rawAddFormats: [Int]?
    private let rawAdPosition: Int?
    private let rawBattr: [Int]?
    
    enum CodingKeys: String, CodingKey {
        case width, height, bidFloor
        case rawAddFormats = "addFormats"
        case rawAdPosition = "adPosition"
        case rawBattr = "battr"
    }
    
    //this is needed because RTB.Position's declaration is overriding swift's normal decode methods
    var adPosition: RTB.Position? {
        if let position = rawAdPosition {
            return RTB.Position(rawValue: position)
        }
        return nil
    }
    
    //this is needed because RTB.CreativeAttribute's declaration is overriding swift's normal decode methods
    var battr: Set<RTB.CreativeAttribute>? {
        guard let rawBattr = rawBattr else { return nil }
        return Set(rawBattr.compactMap { intValue in
            RTB.CreativeAttribute(rawValue: intValue)
        })
    }
    
    var requestComponent: any RequestComponent {
        var adSize: AdSize? = nil
        if let width = width, let height = height {
            adSize = AdSize(width: width, height: height)
        }
        var addFormats: Set<RTB.Format> = []
        //this has to be done here because RTB.Format.Interstitial is @MainActor locked
        if let rawAddFormats = rawAddFormats {
            addFormats = Set(rawAddFormats.compactMap { intValue in
                //needed because RTB.Format doesnt have an int value
                switch intValue {
                case 0:
                    RTB.Format.banner
                case 1:
                    RTB.Format.halfScreen
                case 2:
                    RTB.Format.interstitial
                case 3:
                    RTB.Format.interstitialLandscape
                case 4:
                    RTB.Format.interstitialPortrait
                case 5:
                    RTB.Format.leaderboard
                case 6:
                    RTB.Format.mrec
                default:
                    nil
                }
            })
        }
        return banner(size: adSize, addFormats: addFormats, adPosition: adPosition ?? .unknown, bidFloor: bidFloor, battr: battr ?? [])
    }
}

struct VideoCreative: Codable, UnityRequestComponent {
    let adPosition: RTB.Position?
    let bidFloor: Float?
    let minDuration: Int?
    let maxDuration: Int?
    let width: Int?
    let height: Int?
    let rawPlacementType: Int?
    let rawPlaybackMethod: [Int]?
    
    enum CodingKeys: String, CodingKey {
        case adPosition, bidFloor, minDuration, maxDuration, width, height
        case rawPlacementType = "placementType"
        case rawPlaybackMethod = "playbackMethod"
    }
    
    //this is needed because RTB.VideoPlacementType's declaration is overriding swift's normal decode methods
    var placementType: RTB.VideoPlacementType? {
        if let placement = rawPlacementType {
            return RTB.VideoPlacementType(rawValue: placement)
        }
        return nil
    }
    
    //this is needed because RTB.PlaybackMethod's declaration is overriding swift's normal decode methods
    var playbackMethod: Set<RTB.PlaybackMethod>? {
        guard let playbackMethod = rawPlaybackMethod else { return nil }
        return Set(playbackMethod.compactMap { intValue in
            RTB.PlaybackMethod(rawValue: intValue)
        })
    }
    
    var requestComponent: any NimbusKit.RequestComponent {
        video(adPosition: adPosition ?? .unknown, bidFloor: bidFloor, minDuration: minDuration, maxDuration: maxDuration, width: width,
              height: height, placementType: placementType, playbackMethod: playbackMethod ?? [])
    }
}

struct Env: Codable, UnityRequestComponent {
    let publisherKey: String
    let apiKey: String
    
    var requestComponent: any NimbusKit.RequestComponent {
        environment(publisherKey: publisherKey, apiKey: apiKey)
    }
}

struct Viewability: Codable, UnityRequestComponent {
    let omidPn: String
    let omidPv: String
    var requestComponent: any NimbusKit.RequestComponent {
        viewability(omidpn: omidPn, omidpv: omidPv)
    }
}

struct PerRequestApp: Codable, UnityRequestComponent {
    let pageCat: Set<String>
    let sectionCat: Set<String>
    var requestComponent: any NimbusKit.RequestComponent {
        app(pagecat: pageCat, sectioncat: sectionCat)
    }
}

struct Location: Codable, UnityRequestComponent {
    let latitude: Double
    let longitude: Double
    let accuracy: Int?
    
    private let rawlocationType: Int
    
    enum CodingKeys: String, CodingKey {
        case latitude, longitude, accuracy
        case rawlocationType = "locationType"
    }
    
    //this is needed because RTB.Geo.LocationType's declaration is overriding swift's normal decode methods
    var locationType: RTB.Geo.LocationType {
        return RTB.Geo.LocationType(rawValue: rawlocationType) ?? RTB.Geo.LocationType.gps
    }
    
    var requestComponent: any NimbusKit.RequestComponent {
        location(latitude: latitude, longitude: longitude, type: locationType, accuracy: accuracy)
    }
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
