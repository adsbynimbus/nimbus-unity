//
//
//  NimbusManager.swift
//
//  Created by Bruno Bruggemann on 5/7/21.
//  Copyright © 2021 AdsByNimbus. All rights reserved.
//

import Foundation
import NimbusRenderStaticKit
import NimbusRenderVideoKit
import NimbusKit
#if NIMBUS_ENABLE_APS
import NimbusRequestAPSKit
#endif
#if NIMBUS_ENABLE_VUNGLE
import VungleAdsSDK
import NimbusSDK
#endif

@objc public class NimbusManager: NSObject {
    
    private static var managerDictionary: [Int: NimbusManager] = [:]
    
    private let adUnitInstanceId: Int
    private var nimbusAdManager: NimbusAdManager?
    private var adController: AdController?
    private var adView: NimbusAdView?
    private var nimbusAdVC: NimbusAdViewController?
    private lazy var thirdPartyInterstitialAdManager = ThirdPartyInterstitialAdManager()
    private var thirdPartyInterstitialAdController: AdController?
    
    #if NIMBUS_ENABLE_APS
    private static var apsRequestHelper: NimbusAPSRequestHelper?
    #endif
    
    // MARK: - Class Functions
    
    @objc public class func initializeNimbusSDK(
        publisher: String,
        apiKey: String,
        enableUnityLogs: Bool
    ) {
        Nimbus.shared.initialize(publisher: publisher, apiKey: apiKey)
        Nimbus.shared.logLevel = enableUnityLogs ? .debug : .off
        let videoRenderer = NimbusVideoAdRenderer()
        videoRenderer.showMuteButton = true
        Nimbus.shared.renderers = [
            .forAuctionType(.static): NimbusStaticAdRenderer(),
            .forAuctionType(.video): videoRenderer,
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

    #if NIMBUS_ENABLE_APS
    @objc public class func initializeAPSRequestHelper(appKey: String, timeoutInSeconds: Double, enableTestMode: Bool) {
        apsRequestHelper = NimbusAPSRequestHelper(appKey: appKey, timeoutInSeconds: timeoutInSeconds, enableTestMode: enableTestMode)
    }

    @objc public class func addAPSSlot(slotUUID: String, width: Int, height: Int, isVideo: Bool) {
        apsRequestHelper?.addAPSSlot(slotUUID: slotUUID, width: width, height: height, isVideo: isVideo) 
    }

    @objc public class func fetchAPSParams(width: Int, height: Int, includeVideo: Bool) -> String {
        let data = apsRequestHelper?.fetchAPSParams(width: width, height: height, includeVideo: includeVideo);
        if let unwrapped = data {
            return unwrapped;
        }
        return "";
    }
    #endif
    
    #if NIMBUS_ENABLE_VUNGLE
        @objc public class func initializeVungle(appKey: String) {
            let vungleRequestInterceptor = NimbusVungleRequestInterceptor(appId: appKey)
            NimbusRequestManager.requestInterceptors?.append(vungleRequestInterceptor)
            Nimbus.shared.renderers = [.forNetwork("vungle"): NimbusVungleAdRenderer()]
        }
        
        @objc public class func fetchVungleBuyerId() -> String {
            return VungleAds.getBiddingToken()
        }
    #endif
    
    // MARK: - Private Functions
    
    private init(adUnitInstanceId: Int) {
        self.adUnitInstanceId = adUnitInstanceId
    }
    
    private func unityViewController() -> UIViewController? {
        UnityFramework.getInstance().appController().rootViewController
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
            guard let adView = adView else { return }
            adView.delegate = self
            adView.volume = 100
            adView.isBlocking = true
            adView.showsSKOverlay = true
            
            let companionAd: NimbusCompanionAd
            if UIDevice.current.orientation.isLandscape {
                companionAd = NimbusCompanionAd(width: 480, height: 320, renderMode: .endCard)
            } else {
                companionAd = NimbusCompanionAd(width: 320, height: 480, renderMode: .endCard)
            }
            
            if ThirdPartyDemandNetwork.exists(for: nimbusAd) {
                thirdPartyInterstitialAdController = try? thirdPartyInterstitialAdManager.render(
                    ad: nimbusAd,
                    adPresentingViewController: viewController,
                    companionAd: companionAd
                )
                guard let thirdPartyInterstitialAdController else {
                    Nimbus.shared.logger.log("Unable to render ad for network: \(nimbusAd.network)", level: .error)
                    return
                }
                
                thirdPartyInterstitialAdController.delegate = self
                thirdPartyInterstitialAdController.start()
            } else {
                nimbusAdVC = NimbusAdViewController(
                    adView: adView,
                    ad: nimbusAd,
                    companionAd: companionAd,
                    closeButtonDelay: closeButtonDelay,
                    isRewardedAd: isRewarded
                )
                guard let nimbusAdVC = nimbusAdVC else { return }
                
                // Instead of the VC sent in by the publisher, we are creating the blocking VC here
                adView.adPresentingViewController = nimbusAdVC
                nimbusAdVC.modalPresentationStyle = .fullScreen
                nimbusAdVC.delegate = self
                
                viewController.present(nimbusAdVC, animated: true)
                
                nimbusAdVC.renderAndStart()
            }
            
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
        nimbusAdVC?.dismiss(animated: true)
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