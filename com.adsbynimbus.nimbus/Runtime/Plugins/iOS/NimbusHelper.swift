//
//  NimbusHelper.swift
//  UnityFramework
//
//  Created by Jonathan Sligh on 05/07/26.
//  Copyright © 2026 AdsByNimbus. All rights reserved.
//

import Foundation
import NimbusKit
import AdSupport
import AppTrackingTransparency

@objc public class NimbusHelper: NSObject {
    
    public static var verificationMarkupMethodCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?)?
    public static var verificationResourceMethodCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?)?
    
    @objc public class func setSessionId(sessionId: String) {
        Nimbus.configuration.sessionId = sessionId
    }
    
    @objc public class func setCoppa(coppa: Bool) {
        Nimbus.configuration.coppa = coppa
    }
    
    @objc public class func setApp(appJsonStr: String) {
        if (appJsonStr != "" && !appJsonStr.isEmpty) {
            do {
                if let dataFromString = appJsonStr.data(using: .utf8) {
                    Nimbus.configuration.app = try JSONDecoder().decode(RTB.App.self, from: dataFromString)
                }
            } catch {
                didReceiveNimbusError(
                    adUnitInstanceID: -1,
                    error: .unitysdk(stage: .request, detail: "Failed to decode App json: \(error)")
                )
            }
        }
    }
    
    @objc public class func setUser(userJsonStr: String) {
        if (userJsonStr != "" && !userJsonStr.isEmpty) {
            do {
                if let dataFromString = userJsonStr.data(using: .utf8) {
                    Nimbus.configuration.user = try JSONDecoder().decode(RTB.User.self, from: dataFromString)
                }
            } catch {
                didReceiveNimbusError(
                    adUnitInstanceID: -1,
                    error: .unitysdk(stage: .request, detail: "Failed to decode User json: \(error)")
                )
            }
        }
    }
    
    @objc public class func setBlockedAdvertisingDomains(domains: String) {
        let domainArray = domains.components(separatedBy: ",")
        var blockedDomains: Set<URL> = []
        for domain in domainArray {
            if let url = URL(string: domain) {
                blockedDomains.insert(url)
            }
        }
        Nimbus.configuration.blockedAdvertisingDomains = blockedDomains
    }
    
    @objc public class func setRequestUrl(url: String) {
        if let url = URL(string: url) {
            Nimbus.configuration.requestUrl = url
        }
    }
    
    @objc public class func setAdditionalRequestHeaders(headersJsonStr: String) {
        guard let jsonData = headersJsonStr.data(using: .utf8) else {
            didReceiveNimbusError(
                adUnitInstanceID: -1,
                error: .unitysdk(stage: .request, detail: "Failed to decode Headers JSON")
            )
            return
        }
        do {
            if let jsonObject = try JSONSerialization.jsonObject(with: jsonData) as? [String: String] {
                // Access fields safely using downcasting
                Nimbus.configuration.additionalRequestHeaders = jsonObject
            }
        } catch {
            didReceiveNimbusError(
                adUnitInstanceID: -1,
                error: .unitysdk(stage: .request, detail: "Failed to decode Headers JSON: \(error)")
            )
        }
    }
    
    @objc public class func setInterceptorTimeout(timeout: Int) {
        Nimbus.configuration.interceptorsTimeout = timeout
    }
    
    @objc public class func showMuteButton(show: Bool) {
        Nimbus.configuration.showMuteButton = show
    }
    
    @objc public class func enableSwipeProtection(enable: Bool) {
        Nimbus.configuration.enableSwipeProtection = enable
    }
    
    @objc public class func setIsSKOverlayEnabledForAllUnits(isEnabled: Bool) {
        Nimbus.configuration.isSKOverlayEnabledForAllUnits = isEnabled
    }
    
    final class VerificationProviderHelper: NimbusKit.Configuration.VerificationProvider {
        let index: Int
        
        init(index: Int) {
            self.index = index
        }
        
        func verificationMarkup(response: NimbusKit.NimbusResponse) -> String {
            if let callback = verificationMarkupMethodCallback {
                let responseStr = response.bid.adm
                if let pointer = callback(responseStr, index) {
                    defer{free(pointer)}
                    return String(cString: pointer)
                }
            }
            return ""
        }
        
        func verificationResource(response: NimbusKit.NimbusResponse) -> NimbusKit.Configuration.VerificationScriptResource? {
            if let callback = verificationResourceMethodCallback {
                let responseStr = response.bid.adm
                if let pointer = callback(responseStr, index) {
                    do {
                        if let dataFromString = String(cString: pointer).data(using: .utf8) {
                            let resource: VerificationScriptResource? = try JSONDecoder().decode(VerificationScriptResource.self, from: dataFromString)
                            if let res = resource
                            {
                                if let url = URL(string: res.url) {
                                    return NimbusKit.Configuration.VerificationScriptResource(url: url, vendorKey: res.vendorKey, parameters: res.parameters)
                                } else {
                                    didReceiveNimbusError(
                                        adUnitInstanceID: -1,
                                        error: .unitysdk(stage: .request, detail: "VerificationScriptResource URL was incorrectly formed, \(res.url) is not a valid URL")
                                    )
                                }
                            } else {
                                didReceiveNimbusError(
                                    adUnitInstanceID: -1,
                                    error: .unitysdk(stage: .request, detail: "VerificationScriptResource was null")
                                )
                            }
                        }
                    } catch {
                        didReceiveNimbusError(
                            adUnitInstanceID: -1,
                            error: .unitysdk(stage: .request, detail: "Failed to decode VerificationScriptResource JSON: \(error)")
                        )
                    }
                }
            }
            return nil
        }
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

    @objc public class func setVerificationProviders(markupCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?), resourceCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?), numCallbacks: Int) {
        verificationMarkupMethodCallback = markupCallback
        verificationResourceMethodCallback = resourceCallback
        var providers = [NimbusKit.Configuration.VerificationProvider]()
        for i in 0..<numCallbacks {
            providers.append(VerificationProviderHelper(index: i))
        }
        Nimbus.configuration.verificationProviders = providers
    }
    
    struct VerificationScriptResource: Codable
    {
        let url: String
        let vendorKey: String
        let parameters: String
    }
    
    
    public static func extensionsFromJsonString(thirdPartyDemand: String) -> Extensions? {
        var extensions: Extensions?
        if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty) {
            do {
                if let dataFromString = thirdPartyDemand.data(using: .utf8) {
                    extensions = try JSONDecoder().decode(Extensions.self, from: dataFromString)
                }
            } catch {
                NimbusHelper.didReceiveNimbusError(
                    adUnitInstanceID: -1,
                    error: .unitysdk(stage: .request, detail: "Failed to decode third party json: \(error)")
                )
            }
        }
        return extensions
    }

    @objc public class func setVerificationProviders(markupCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?), resourceCallback: (@convention(c) (UnsafePointer<CChar>, Int) -> UnsafeMutablePointer<CChar>?), numCallbacks: Int) {
        verificationMarkupMethodCallback = markupCallback
        verificationResourceMethodCallback = resourceCallback
        var providers = [NimbusKit.Configuration.VerificationProvider]()
        for i in 0..<numCallbacks {
            providers.append(VerificationProviderHelper(index: i))
        }
        Nimbus.configuration.verificationProviders = providers
    }
    
    public static func requestModifiersFromJsonString(requestModifiers: String) -> RequestModifiers? {
        var modifiers: RequestModifiers?
        if (requestModifiers != "" && !requestModifiers.isEmpty) {
            do {
                if let dataFromString = requestModifiers.data(using: .utf8) {
                    modifiers = try JSONDecoder().decode(RequestModifiers.self, from: dataFromString)
                }
            } catch {
                NimbusHelper.didReceiveNimbusError(
                    adUnitInstanceID: -1,
                    error: .unitysdk(stage: .request, detail: "Failed to decode request modifiers json: \(error)")
                )
            }
        }
        return modifiers
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


public struct Extensions: Codable {
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


public struct RequestModifiers: Codable, Sendable {
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

public struct BannerCreative: Codable, UnityRequestComponent, Sendable {
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
                case 1:
                    RTB.Format.banner
                case 2:
                    RTB.Format.mrec
                case 3:
                    RTB.Format.halfScreen
                case 4:
                    RTB.Format.leaderboard
                case 5:
                    RTB.Format.interstitialPortrait
                case 6:
                    RTB.Format.interstitialLandscape
                case 7:
                    RTB.Format.leaderboard
                default:
                    nil
                }
            })
        }
        return banner(size: adSize, addFormats: addFormats, adPosition: adPosition ?? .unknown, bidFloor: bidFloor, battr: battr ?? [])
    }
}


protocol UnityRequestComponent {
    @MainActor var requestComponent: any RequestComponent { get }
}

public struct VideoCreative: Codable, UnityRequestComponent, Sendable {
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

public struct Env: Codable, UnityRequestComponent, Sendable {
    let publisherKey: String
    let apiKey: String
    
    var requestComponent: any NimbusKit.RequestComponent {
        environment(publisherKey: publisherKey, apiKey: apiKey)
    }
}

public struct Viewability: Codable, UnityRequestComponent, Sendable {
    let omidPn: String
    let omidPv: String
    var requestComponent: any NimbusKit.RequestComponent {
        viewability(omidpn: omidPn, omidpv: omidPv)
    }
}

public struct PerRequestApp: Codable, UnityRequestComponent, Sendable {
    let pageCat: Set<String>
    let sectionCat: Set<String>
    var requestComponent: any NimbusKit.RequestComponent {
        app(pagecat: pageCat, sectioncat: sectionCat)
    }
}

public struct Location: Codable, UnityRequestComponent, Sendable {
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

