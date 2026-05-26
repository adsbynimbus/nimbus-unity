import androidx.collection.LruCache
import com.adsbynimbus.Ad
import java.util.UUID

object NimbusAdCache{

    // Thread-safe maps to store requests and responses
    const val cacheSize: Int = 10 // max number of entries
    private val adCache = LruCache<String, Ad>(cacheSize)

    fun addAd(ad: Ad): String {
        val uniqueKey = UUID.randomUUID().toString()
        synchronized(adCache) {
            adCache.put(uniqueKey, ad)
        }
        return uniqueKey
    }

    fun getAd(key: String): Ad? {
        return synchronized(adCache) {
            adCache[key]
        }
    }

    fun removeAd(key: String) {
        synchronized(adCache) {
            adCache.remove(key)
        }
    }

    fun clearAdCache() {
        synchronized(adCache) {
            adCache.evictAll()
        }
    }
}
