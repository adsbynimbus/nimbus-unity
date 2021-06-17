//
//  UnityBinding.swift
//  Unity-iPhone
//
//  Created by Bruno Bruggemann on 6/16/21.
//

import Foundation

class UnityBinding {
    
    private static let kCallbackTarget = "NimbusIOSAdManager"

    class func sendMessage(methodName: String, params: [String: Any]) {
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: params, options: JSONSerialization.WritingOptions())
            let jsonString = String(data: jsonData, encoding: .utf8)
            
            UnitySendMessage(kCallbackTarget, methodName, jsonString);
        } catch {
            print("Error creating json object: \(error)")
            return
        }
    }
}
