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
    private var adView: NimbusAdView?
    
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
    
    @objc public func renderAd(bidResponse: String, isBlocking: Bool, isRewarded: Bool, closeButtonDelay: TimeInterval) {
        guard let data = bidResponse.data(using: .utf8) else {
            Nimbus.shared.logger.log("Unable to get data from bid response", level: .error)
            return
        }
        
        guard let nimbusAd = try? JSONDecoder().decode(NimbusAd.self, from: data) else {
            Nimbus.shared.logger.log("Unable to get NimbusAd from bid response data", level: .error)
            return
        }
        
        guard let viewController = unityViewController() else { return }
        
        if isBlocking {
            adView = NimbusAdView(adPresentingViewController: viewController)
            adView?.delegate = self
            adView?.volume = 100
            guard let adView = adView else { return }
            
            let companionAd: NimbusCompanionAd
            if UIDevice.current.orientation.isLandscape {
                companionAd = NimbusCompanionAd(width: 480, height: 320, renderMode: .endCard)
            } else {
                companionAd = NimbusCompanionAd(width: 320, height: 480, renderMode: .endCard)
            }
            
            let adVC = NimbusAdViewController(
                adView: adView,
                ad: nimbusAd,
                companionAd: companionAd,
                closeButtonDelay: closeButtonDelay,
                isRewardedAd: isRewarded
            )
            // Instead of the VC sent in by the publisher, we are creating the blocking VC here
            adView.adPresentingViewController = adVC
            adVC.modalPresentationStyle = .fullScreen
            adVC.delegate = self
            
            viewController.present(adVC, animated: true)
            
            adVC.renderAndStart()
            
            UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
        } else {
            adView = NimbusAdView(adPresentingViewController: viewController)
            adView?.delegate = self
            guard let adView = adView else { return }
            
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
                        
            adView.render(ad: nimbusAd)
            adView.start()
            
            UnityBinding.sendMessage(methodName: "OnAdRendered", params: ["adUnitInstanceID": adUnitInstanceId])
        }
    }
    
    @objc public func destroyExistingAd() {
        adView?.destroy()
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
            eventName = "RESUMED"
        case .completed:
            eventName = "COMPLETED"
        case .destroyed:
            eventName = "DESTROYED"
            removeReferenceFromManagerDictionary()
        @unknown default:
            print("Ad Event not sent: \(event)")
            return
        }
        
        UnityBinding.sendMessage(
            methodName: "OnAdEvent",
            params: [
                "adUnitInstanceID": adUnitInstanceId,
                "eventName": eventName
            ]
        )
    }
    
    /// Received an error for the ad
    public func didReceiveNimbusError(controller: AdController, error: NimbusError) {
        UnityBinding.sendMessage(
            methodName: "OnError",
            params: [
                "adUnitInstanceID": adUnitInstanceId,
                "errorMessage": error.localizedDescription
            ]
        )
        destroyExistingAd()
    }
}

extension NimbusManager: NimbusAdViewControllerDelegate {
    public func viewWillAppear(animated: Bool) {}
    public func viewDidAppear(animated: Bool) {}
    public func viewWillDisappear(animated: Bool) {}
    public func viewDidDisappear(animated: Bool) {}
    public func didCloseAd(adView: NimbusAdView) { adView.destroy() }
}
