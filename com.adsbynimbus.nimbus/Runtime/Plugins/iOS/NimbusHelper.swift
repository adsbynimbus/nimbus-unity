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
    
    @objc public class func getSessionId() -> String {
        Nimbus.shared.sessionId
    }
        
    @objc public class func getUserAgent() -> String {
        Nimbus.shared.userAgentString
    }
    
    @objc public class func getAdvertisingId() -> String {
        ASIdentifierManager.shared().nimbusAdId()
    }
    
    @objc public class func getConnectionType() -> Int {
        Nimbus.shared.connectionType.rawValue
    }
    
    @objc public class func getDeviceModel() -> String {
        UIDevice.current.nimbusModelName
    }
    
    @objc public class func getSystemVersion() -> String {
        UIDevice.current.systemVersion
    }

    @objc public class func getVendorId() -> String? {
        UIDevice.current.identifierForVendor?.uuidString
    }

    @objc public class func getAtts() -> Int {
        if #available(iOS 14.0, *) {
            return Int(ATTrackingManager.trackingAuthorizationStatus.rawValue)
        } else {
            return -1
        }
    }
    
    @objc public class func setCoppa(flag: Bool) {
        Nimbus.shared.coppa = flag;
    }
    
    @objc public class func isLimitAdTrackingEnabled() -> Bool {
        if #available(iOS 14.0, *) {
            return ATTrackingManager.trackingAuthorizationStatus != .authorized
        } else {
            return !ASIdentifierManager.shared().isAdvertisingTrackingEnabled
        }
    }
    
    @objc public class func getPlistJSON() -> String? {
    	guard let infoDict = Bundle.main.infoDictionary,
    	let jsonData = try? JSONSerialization.data(withJSONObject: infoDict, options: []) else { 
    		return nil
    	}
    	return String(data: jsonData, encoding: .ascii)
    }
}
