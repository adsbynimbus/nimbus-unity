//
//  UnityBinding.swift
//  Unity-iPhone
//
//  Created by Bruno Bruggemann on 6/16/21.
//

import Foundation

class UnityBinding {
    
    private static let kCallbackTarget = "NimbusCallbackReceiver"
    
    // Created this way so it can be mocked out for unit tests
    public static var sendAction: (String, String, String) -> Void = { callbackTarget, methodName, jsonParamString in
        UnitySendMessage(callbackTarget, methodName, jsonParamString)
    }

    class func sendMessage(methodName: String, params: [String: Any]) {
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: params, options: JSONSerialization.WritingOptions())
            if let jsonString = String(data: jsonData, encoding: .utf8) {
                sendAction(kCallbackTarget, methodName, jsonString)
            }
        } catch {
            print("Error creating json object: \(error)")
            return
        }
    }

}
