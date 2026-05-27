package com.adsbynimbus.unity

import android.app.Activity
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.view.ViewGroup.LayoutParams.WRAP_CONTENT
import android.widget.FrameLayout
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.marginEnd
import androidx.core.view.updateLayoutParams
import androidx.lifecycle.LifecycleCoroutineScope
import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.coroutineScope
import com.adsbynimbus.Ad
import com.adsbynimbus.AdSize
import com.adsbynimbus.InlineAd
import com.adsbynimbus.InterstitialAd
import com.adsbynimbus.Nimbus
import com.adsbynimbus.RewardedAd
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.serialization.*
import kotlinx.serialization.json.*
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors

class UnityHelper {
    companion object {

        @JvmStatic
        fun initNimbusAndThirdParties(obj: Any?, publisherKey: String, apiKey: String, 
            enableSDKInTestMode: Boolean,
            thirdPartyJson: String
        ) {
            if (obj is Activity) {
                Nimbus.configuration.requestUrl = "https://dev-sdk.adsbynimbus.com/rta/test"
                val extensions = extensionsFromJsonString(thirdPartyJson) ?: return
                Nimbus.initialize(obj, publisherKey, apiKey)
            }
        }


        @JvmStatic
        fun bannerAd(obj: Any?, position: String, adWidth: Int, adHeight: Int, refreshInterval: Int,
                     respectSafeArea: Boolean, bannerPosition: Int, showAd: Boolean, thirdPartyDemand: String): String {
            var ad: Ad
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return ""
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            /* TODO: Deal with APS / AdMob stuff from Extensions obj*/
            var adSize = AdSize.Banner
            when (adWidth) {
                300 -> adSize = if (adHeight == 600) AdSize.HalfScreen else AdSize.Mrec
                320 -> adSize = if (adHeight == 480) AdSize.InterstitialPortrait else AdSize.Banner
                480 -> adSize = AdSize.InterstitialLandscape
                728 -> adSize = AdSize.Leaderboard
            }
            ad = Nimbus.bannerAd(position = position, size = adSize)
                .onEvent {
                    /*TODO: Pass Event back to Unity*/
                }.onError {
                    /*TODO: Pass Error back to Unity*/
                }
            if (showAd) {
                showBannerAd(obj, ad, respectSafeArea, bannerPosition)
            }
            return if (!showAd) NimbusAdCache.addAd(ad) else ""
        }

        @JvmStatic
        fun interstitialAd(obj: Any?, position: String, showAd: Boolean, thirdPartyDemand: String): String {
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return ""
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            /* TODO: Deal with APS / AdMob stuff from Extensions obj*/
            val ad = Nimbus.interstitialAd(position).onEvent { event ->
                /*TODO: Pass Event back to Unity*/
            }.onError {error ->  /*TODO: Pass ERROR back to Unity*/ }
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                if (showAd) {
                    ad.show(obj)
                } else {
                    ad.load(obj)
                }
            }
            return if (!showAd) NimbusAdCache.addAd(ad) else ""
        }

        @JvmStatic
        fun rewardedAd(obj: Any?, position: String, showAd: Boolean, thirdPartyDemand: String): String {
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return ""
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            /* TODO: Deal with APS / AdMob stuff from Extensions obj*/
            val ad = Nimbus.rewardedAd(position).onEvent { event ->
                /*TODO: Pass Event back to Unity*/
            }.onError {error ->  /*TODO: Pass ERROR back to Unity*/ }
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                if (showAd) {
                    ad.show(obj)
                } else {
                    ad.load(obj)
                }
            }
            return if (!showAd) NimbusAdCache.addAd(ad) else ""
        }

        @JvmStatic
        fun showAd(obj: Any?, adId: String, respectSafeArea: Boolean, bannerPosition: Int) {
            val ad = NimbusAdCache.getAd(adId)
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return
            }
            if (ad == null) {
                /*TODO: Pass Error back to Unity*/
                return
            }
            if (ad is InlineAd) {
                showBannerAd(obj, ad, respectSafeArea, bannerPosition)
            } else {
                val scope = CoroutineScope(Dispatchers.Main)
                scope.launch {
                    if (ad is InterstitialAd) {
                        ad.show(obj)
                    } else if (ad is RewardedAd) {
                        ad.show(obj)
                    }
                }
            }
        }

        @JvmStatic
        fun showBannerAd(obj: Activity, ad: InlineAd, respectSafeArea: Boolean, bannerPosition: Int) {
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                val adFrame = FrameLayout(obj)
                obj.addContentView(
                    adFrame, ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT
                    )
                )
                var bannerGravity = 0
                when (bannerPosition) {
                    0 -> bannerGravity = Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL
                    1 -> bannerGravity = Gravity.TOP or Gravity.CENTER_HORIZONTAL
                    2 -> bannerGravity = Gravity.CENTER
                    3 -> bannerGravity = Gravity.BOTTOM or Gravity.START
                    4 -> bannerGravity = Gravity.BOTTOM or Gravity.END
                    5 -> bannerGravity = Gravity.TOP or Gravity.START
                    6 -> bannerGravity = Gravity.TOP or Gravity.END
                }
                ad.show(adFrame).also {
                    // TODO: Fix this as the lefts and rights arent working
                    it.adView?.updateLayoutParams<FrameLayout.LayoutParams> {
                        gravity = bannerGravity
                        height = WRAP_CONTENT
                    }
                    if (respectSafeArea) {
                        // TODO: Fix this as the safe area stuff isnt working
                        ViewCompat.setOnApplyWindowInsetsListener(it.adView ?: View(obj)) { view, insets ->
                            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
                            val mlp = view.layoutParams as ViewGroup.MarginLayoutParams
                            mlp.leftMargin = systemBars.left
                            mlp.bottomMargin = systemBars.bottom
                            mlp.rightMargin = systemBars.right
                            mlp.topMargin = systemBars.top
                            view.layoutParams = mlp
                            insets
                        }
                    }
                }
            }
        }

        fun extensionsFromJsonString(thirdPartyDemand: String): Extensions? {
            var extensions: Extensions? = null
            if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty()) {
                try {
                    extensions = Json.decodeFromString<Extensions>(thirdPartyDemand)
                } catch(e: Exception) {
                    // TODO: Pass Error back to Unity
                }
            }
            return extensions
        }

        @JvmStatic
        fun render(
            obj: Any?,
            isBlocking: Boolean,
            isRewarded: Boolean,
            closeButtonDelay: Int,
            listener: Any?,
            mintegralAdUnitId: String?,
            mintegralAdUnitPlacementId: String?,
            molocoAdUnitId: String?,
            inMobiPlacementId: String?,
            respectSafeArea: Boolean,
            adPosition: Int
        ) {
            if (obj is Activity) {
                bannerAd(obj, "unityTest", 320, 50, 30, true, 0, true, "")
                //interstitialAd(obj, "unityTest", true, "")
                //rewardedAd(obj, "unityTest", true, "")
            }
        }

    }
}

@Serializable
data class Extensions(
    val aps: Aps?,
    val adMob: AdMob?,
    val inMobi: InMobi?,
    val meta: Meta?,
    val mintegral: Mintegral?,
    val mobileFuse: JsonObject?,
    val moloco: Moloco?,
    val unityAds: UnityAds?,
    val vungle: Vungle?)
@Serializable
data class AdMob (val adUnitIds: Array<String?>?)
@Serializable
data class Aps (val appKey : String?, val slotData: Array<ApsSlotData?>?)
@Serializable
data class ApsSlotData(val slotId: String?, val adUnitType: APSAdUnitType?)
@Serializable
enum class APSAdUnitType(val i: Int) {
    display320X50(0),
    display300X250(1),
    display728X90(2),
    interstitialDisplay(3),
    interstitialVideo(4),
    rewardedVideo(5),
}
@Serializable
data class InMobi(val accountId: String?)
@Serializable
data class Meta(val appId: String?, val forceTestAd: Boolean)
@Serializable
data class Mintegral(val appId: String?, val appKey : String?)
@Serializable
data class Moloco(val appKey: String?)
@Serializable
data class UnityAds(val gameId: String?)
@Serializable
data class Vungle(val appId:String?)