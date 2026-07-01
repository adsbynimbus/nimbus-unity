//
//  NimbusHelper.swift
//  UnityFramework
//
//  Created by Victor Takai on 10/06/22.
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
                NimbusManager.didReceiveNimbusError(
                    adUnitInstanceID: 0,
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
                NimbusManager.didReceiveNimbusError(
                    adUnitInstanceID: 0,
                    error: .unitysdk(stage: .request, detail: "Failed to decode User json: \(error)")
                )
            }
        }
    }
    
    @objc public class func setBlockedAdvertisingDomains(domains: String) {
        var domainArray = domains.components(separatedBy: ",")
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
            NimbusManager.didReceiveNimbusError(
                adUnitInstanceID: 0,
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
            NimbusManager.didReceiveNimbusError(
                adUnitInstanceID: 0,
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
    
    @objc public class func setGdprProperties(gdprApplies: Bool, gdprConsentString: String) {
        Nimbus.IAB.gdprApplies = gdprApplies
        Nimbus.IAB.tcfString = gdprConsentString
    }
    
    @objc public class func setGppProperties(gppSectionId: String, gppConsentString: String) {
        Nimbus.IAB.gppSectionId = gppSectionId
        Nimbus.IAB.gppConsentString = gppConsentString
    }
    
    @objc public class func setUsPrivacyString(usPrivacyString: String) {
        Nimbus.IAB.usPrivacyString = usPrivacyString
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
                    return String(cString: pointer)
                }
            }
            return ""
        }
        
        func verificationResource(response: NimbusKit.NimbusResponse) -> NimbusKit.Configuration.VerificationScriptResource? {
            if let callback = verificationResourceMethodCallback {
                let responseStr = response.bid.adm
                if let pointer = callback(responseStr, index) {
                    let resourceArray = String(cString: pointer).components(separatedBy: ",")
                    if (resourceArray.count == 3) {
                        if let url = URL(string: resourceArray[0]) {
                            return NimbusKit.Configuration.VerificationScriptResource(url: url, vendorKey: resourceArray[1], parameters: resourceArray[2])
                        }
                    }
                }
            }
            return nil
        }
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
}

