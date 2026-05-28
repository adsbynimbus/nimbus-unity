import androidx.collection.LruCache
import com.adsbynimbus.Ad
import java.util.UUID

object NimbusAdCache{

    // Thread-safe maps to store ads
    const val cacheSize: Int = 10 // max number of entries
    private val adCache = LruCache<Int, Ad>(cacheSize)

    fun addAd(ad: Ad, key: Int) {
        synchronized(adCache) {
            adCache.put(key, ad)
        }
    }

    fun getAd(key: Int): Ad? {
        return synchronized(adCache) {
            adCache[key]
        }
    }

    fun removeAd(key: Int) {
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
