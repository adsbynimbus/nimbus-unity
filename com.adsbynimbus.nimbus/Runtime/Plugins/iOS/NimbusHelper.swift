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
    
    
    @objc public class func getDeviceLanguage() -> String? {
        guard let preferred = Locale.preferredLanguages.first else {
            // Edge case fallback as I saw some old ObjC case where preferredLanguages
            // returned an empty array (causing a crash) and Apple Docs don't say this array must not
            // be empty despite not being able to delete all preferred languages on an iOS device.
            return Locale.current.languageCode
        }
            
        return Locale(identifier: preferred).languageCode
    }

    @objc public class func getAtts() -> Int {
        if #available(iOS 14.0, *) {
            return Int(ATTrackingManager.trackingAuthorizationStatus.rawValue)
        } else {
            return -1
        }
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
