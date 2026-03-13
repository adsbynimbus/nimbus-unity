import Foundation
import NimbusKit
import NimbusSDK

@MainActor
public class NimbusAdCache {

    public static let shared = NimbusAdCache()

    private var adCache: [String: Ad] = [:]

    // Add an ad to the cache
    public func addAd(_ nimbusAd: Ad, uniqueKey: String){
        self.adCache[uniqueKey] = nimbusAd
    }

    // Retrieve an ad from the cache
    public func getAd(forKey key: String) -> Ad? {
        return adCache[key]
    }

    // Remove an ad from the cache
    public func removeAd(forKey key: String) {
        self.adCache.removeValue(forKey: key)
    }

    // Clear all cached ads
    public func clearAdCache() {
        self.adCache.removeAll()
    }
}

