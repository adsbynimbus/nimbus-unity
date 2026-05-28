package com.adsbynimbus.unity

import android.app.Activity
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.view.ViewGroup.LayoutParams.WRAP_CONTENT
import android.widget.FrameLayout
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.updateLayoutParams
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
        fun bannerAd(obj: Any?, instanceId: Int, position: String, adWidth: Int, adHeight: Int, refreshInterval: Int,
                     respectSafeArea: Boolean, bannerPosition: Int, showAd: Boolean, thirdPartyDemand: String) {
            var ad: Ad
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return
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
            ad = Nimbus.bannerAd(position = position, size = adSize, refreshInterval = refreshInterval)
                .onEvent {
                    /*TODO: Pass Event back to Unity*/
                }.onError {
                    /*TODO: Pass Error back to Unity*/
                }
            if (showAd) {
                showBannerAd(obj, ad, respectSafeArea, bannerPosition)
            }
            NimbusAdCache.addAd(ad, instanceId)

        }

        @JvmStatic
        fun interstitialAd(obj: Any?, instanceId: Int, position: String, showAd: Boolean, thirdPartyDemand: String) {
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return
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
            NimbusAdCache.addAd(ad, instanceId)
        }

        @JvmStatic
        fun rewardedAd(obj: Any?, instanceId: Int, position: String, showAd: Boolean, thirdPartyDemand: String) {
            if (obj !is Activity) {
                /*TODO: Pass Error back to Unity*/
                return
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
            NimbusAdCache.addAd(ad, instanceId)
        }

        @JvmStatic
        fun showAd(obj: Any?, instanceId: Int, respectSafeArea: Boolean, bannerPosition: Int) {
            val ad = NimbusAdCache.getAd(instanceId)
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
                    adFrame, FrameLayout.LayoutParams(
                        WRAP_CONTENT,
                        WRAP_CONTENT
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
                    adFrame.updateLayoutParams<FrameLayout.LayoutParams> {
                        gravity = bannerGravity
                        height = WRAP_CONTENT
                    }
                    if (respectSafeArea) {
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
        fun destroyAd(adInstanceId: Int) {
            val ad = NimbusAdCache.getAd(adInstanceId)
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                ad?.destroy()
            }
            NimbusAdCache.removeAd(adInstanceId)
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