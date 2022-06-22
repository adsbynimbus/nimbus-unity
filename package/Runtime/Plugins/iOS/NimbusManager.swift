//
//
//  NimbusManager.swift
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 Timehop. All rights reserved.
//

import Foundation
import NimbusRenderStaticKit
import NimbusRenderVideoKit
import NimbusKit

@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let adUnitInstanceId: Int
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?
    private var adView: AdView?
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool
    ) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        Nimbus.shared.logLevel = enableUnityLogs ? .debug : .off
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): NimbusVideoAdRenderer()
        ]
    }
    
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
        UIApplication.shared.connectedScenes
            .filter { $0.activationState == .foregroundActive }
            .compactMap { $0 as? UIWindowScene }
            .first?.windows
            .filter { $0.isKeyWindow }
            .first?.rootViewController
    }
        
    // MARK: - Public Functions
    
    @objc public func renderAd(bidResponse: String, isBlocking: Bool, closeButtonDelay: TimeInterval) {
        do {
            guard let data = bidResponse.data(using: .utf8) else {
                print("WTF cannot parse bid response \(bidResponse)")
                // TODO error:
                return
            }
            
            let nimbusAd = try JSONDecoder().decode(NimbusAd.self, from: data)
            
            guard let viewController = unityViewController() else { return }
            
            if isBlocking {
                let adView = NimbusAdView(adPresentingViewController: viewController)
                adView.volume = 100 // Accurate?
                adView.delegate = self

                let adVC = NimbusAdViewController(
                    adView: adView,
                    ad: nimbusAd,
                    companionAd: nil,
                    closeButtonDelay: closeButtonDelay,
                    isRewardedAd: false
                )
                // Instead of the VC sent in by the publisher, we are creating the blocking VC here
                adView.adPresentingViewController = adVC
                adVC.modalPresentationStyle = .fullScreen
                
                viewController.present(adVC, animated: true)
                                
                adVC.renderAndStart()
            } else {
                let adView = NimbusAdView(adPresentingViewController: viewController)
                viewController.view.addSubview(adView)
                
                adView.translatesAutoresizingMaskIntoConstraints = false
                if nimbusAd.isInterstitial {
                    NSLayoutConstraint.activate([
                        adView.topAnchor.constraint(equalTo: viewController.view.topAnchor),
                        adView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor),
                        adView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor),
                        adView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor),
                    ])
                } else {
                    NSLayoutConstraint.activate([
                        adView.centerXAnchor.constraint(equalTo: viewController.view.centerXAnchor),
                        adView.leadingAnchor.constraint(equalTo: viewController.view.leadingAnchor),
                        adView.trailingAnchor.constraint(equalTo: viewController.view.trailingAnchor),
                        adView.bottomAnchor.constraint(equalTo: viewController.view.bottomAnchor),
                    ])
                }
                
                adView.delegate = self

                adView.render(ad: nimbusAd)
                adView.start()
            }
        } catch {
            // TODO: Error
        }
    }
    
    @objc public func destroyExistingAd() {
        adController?.destroy()
        adView?.removeFromSuperview()
        adView = nil
        removeReferenceFromManagerDictionary()
    }
    
    private func removeReferenceFromManagerDictionary() {
        NimbusManager.managerDictionary.removeValue(forKey: adUnitInstanceId)
    }
}

// MARK: - AdControllerDelegate implementation

extension NimbusManager: AdControllerDelegate {
    
    public func didReceiveNimbusEvent(controller: AdController, event: NimbusEvent) {
        let eventName: String
        switch event {
        case .loaded, .loadedCompanionAd, .firstQuartile, .midpoint, .thirdQuartile, .skipped:
            return // Unity doesn't handle these events
        case .impression:
            eventName = "IMPRESSION"
        case .clicked:
            eventName = "CLICKED"
        case .paused:
            eventName = "PAUSED"
        case .resumed:
            eventName = "RESUME"
        case .completed:
            eventName = "COMPLETED"
        case .destroyed:
            eventName = "DESTROYED"
            removeReferenceFromManagerDictionary()
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "eventName": eventName
        ]
        UnityBinding.sendMessage(methodName: "OnAdEvent", params: params)
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        let params: [String: Any] = [
            "adUnitInstanceID": adUnitInstanceId,
            "errorMessage": error.localizedDescription
        ]
        UnityBinding.sendMessage(methodName: "OnError", params: params)
        destroyExistingAd()
    }
}