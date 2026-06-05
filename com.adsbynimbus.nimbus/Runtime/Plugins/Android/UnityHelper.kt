package com.adsbynimbus.unity

import NimbusAdCache
import android.app.Activity
import android.util.DisplayMetrics
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.updateLayoutParams
import com.adsbynimbus.*
import com.adsbynimbus.request.internal.AdUnitType
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.serialization.KSerializer
import kotlinx.serialization.Serializable
import kotlinx.serialization.descriptors.PrimitiveKind
import kotlinx.serialization.descriptors.PrimitiveSerialDescriptor
import kotlinx.serialization.descriptors.SerialDescriptor
import kotlinx.serialization.encoding.Decoder
import kotlinx.serialization.encoding.Encoder
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.JsonPrimitive
import kotlinx.serialization.json.buildJsonObject
import kotlin.math.roundToInt


class UnityHelper {
    companion object {

        @JvmStatic
        fun initNimbusAndThirdParties(obj: Any?, publisherKey: String, apiKey: String, 
            enableSDKInTestMode: Boolean,
            thirdPartyJson: String
        ) {
            if (obj is Activity) {
                // TODO: REMOVE BELOW URL ONCE DONE TESTING
                Nimbus.configuration.requestUrl = "https://dev-sdk.adsbynimbus.com/rta/test"
                Nimbus.configuration.testMode = enableSDKInTestMode
                val extensions = extensionsFromJsonString(thirdPartyJson) ?: return
                NimbusUnityInternal.initNimbus(obj, enableSDKInTestMode, publisherKey, apiKey, extensions)
            }
        }


        @JvmStatic
        fun bannerAd(obj: Any?, instanceId: Int, position: String, adWidth: Int, adHeight: Int, refreshInterval: Int,
                     respectSafeArea: Boolean, bannerPosition: Int, showAd: Boolean, thirdPartyDemand: String) {
            var ad: Ad
            if (obj !is Activity) {
                return
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            var adSize = AdSize.Banner
            when (adWidth) {
                300 -> adSize = if (adHeight == 600) AdSize.HalfScreen else AdSize.Mrec
                320 -> adSize = if (adHeight == 480) AdSize.InterstitialPortrait else AdSize.Banner
                480 -> adSize = AdSize.InterstitialLandscape
                728 -> adSize = AdSize.Leaderboard
            }
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                val demandBlock =
                    NimbusUnityInternal.demandBlock(obj, AdUnitType.Inline, extensions)
                ad = Nimbus.bannerAd(
                    position = position,
                    size = adSize,
                    refreshInterval = refreshInterval
                ) {
                    demand {
                        demandBlock()
                    }
                }
                    .onEvent { event ->
                        didReceiveNimbusEvent(instanceId, event)
                    }.onError { error ->
                        didReceiveNimbusError(instanceId, error)
                    }
                if (showAd) {
                    showBannerAd(obj, ad, adWidth, adHeight, respectSafeArea, bannerPosition)
                    sendRenderNimbusEvent(instanceId)
                }
                NimbusAdCache.addAd(ad, instanceId)
            }

        }

        @JvmStatic
        fun interstitialAd(obj: Any?, instanceId: Int, position: String, showAd: Boolean, thirdPartyDemand: String) {
            if (obj !is Activity) {
                return
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                val demandBlock = NimbusUnityInternal.demandBlock(obj, AdUnitType.Interstitial, extensions)
                val ad = Nimbus.interstitialAd(position){
                    demand {
                        demandBlock()
                    }
                }.onEvent { event ->
                    didReceiveNimbusEvent(instanceId, event)
                }.onError { error ->
                    didReceiveNimbusError(instanceId, error)
                }
                if (showAd) {
                    ad.show(obj)
                    sendRenderNimbusEvent(instanceId)
                } else {
                    ad.load(obj)
                }
                NimbusAdCache.addAd(ad, instanceId)
            }
        }

        @JvmStatic
        fun rewardedAd(obj: Any?, instanceId: Int, position: String, showAd: Boolean, thirdPartyDemand: String) {
            if (obj !is Activity) {
                return
            }
            val extensions = extensionsFromJsonString(thirdPartyDemand)
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                val demandBlock = NimbusUnityInternal.demandBlock(obj, AdUnitType.Rewarded, extensions)
                val ad = Nimbus.rewardedAd(position){
                    demand {
                        demandBlock()
                    }
                }.onEvent { event ->
                    didReceiveNimbusEvent(instanceId, event)
                }.onError { error ->
                    didReceiveNimbusError(instanceId, error)
                }
                if (showAd) {
                    ad.show(obj)
                    sendRenderNimbusEvent(instanceId)
                } else {
                    ad.load(obj)
                }
                NimbusAdCache.addAd(ad, instanceId)
            }
        }

        @JvmStatic
        fun showAd(obj: Any?, instanceId: Int, adWidth: Int, adHeight: Int, respectSafeArea: Boolean, bannerPosition: Int) {
            val ad = NimbusAdCache.getAd(instanceId)
            if (obj !is Activity) {
                return
            }
            if (ad == null) {
                return
            }
            val scope = CoroutineScope(Dispatchers.Main)
            scope.launch {
                when (ad) {
                    is InlineAd -> {
                        showBannerAd(obj, ad, adWidth, adHeight, respectSafeArea, bannerPosition)
                    }
                    is InterstitialAd -> {
                        ad.show(obj)
                    }
                    is RewardedAd -> {
                        ad.show(obj)
                    }
                    else -> return@launch
                }
            }
            sendRenderNimbusEvent(instanceId)
        }

        private fun dpToPx(dp: Int, activity: Activity): Int {
            val displayMetrics: DisplayMetrics = activity.resources.displayMetrics
            return (dp * (displayMetrics.xdpi / DisplayMetrics.DENSITY_DEFAULT)).roundToInt()
        }

        @JvmStatic
        suspend fun showBannerAd(obj: Activity, ad: InlineAd, adWidth: Int, adHeight: Int, respectSafeArea: Boolean, bannerPosition: Int) {
            val adFrame = FrameLayout(obj)
            obj.addContentView(
                adFrame, FrameLayout.LayoutParams(
                    dpToPx(adWidth, obj),
                    dpToPx(adHeight, obj)))
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
        
        fun extensionsFromJsonString(thirdPartyDemand: String): Extensions? {
            var extensions: Extensions? = null
            if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty()) {
                try {
                    extensions = Json.decodeFromString<Extensions>(thirdPartyDemand)
                } catch(e: Exception) {
                    didReceiveNimbusError(0, e)
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

        @JvmStatic
        private fun sendRenderNimbusEvent(adUnitInstanceID: Int) {
            val jsonString = buildJsonObject {
                put("adUnitInstanceID", JsonPrimitive(adUnitInstanceID))
            }.toString()
            sendMessageToUnity("OnAdRendered",jsonString)
        }


        @JvmStatic
        private fun didReceiveNimbusEvent(adUnitInstanceID: Int, event: AdEvent) {
            var eventName: String
            when (event) {
                AdEvent.Loaded ->
                eventName = "LOADED"
                AdEvent.Impression ->
                eventName = "IMPRESSION"
                AdEvent.Clicked ->
                eventName = "CLICKED"
                AdEvent.Paused ->
                eventName = "PAUSED"
                AdEvent.Resumed ->
                eventName = "RESUMED"
                AdEvent.Completed ->
                eventName = "COMPLETED"
                AdEvent.Destroyed ->
                eventName = "DESTROYED"
                else ->
                return
            }
            val jsonString = buildJsonObject {
                put("adUnitInstanceID", JsonPrimitive(adUnitInstanceID))
                put("eventName", JsonPrimitive(eventName))
            }.toString()
            sendMessageToUnity("OnAdEvent",jsonString)
        }
        private fun didReceiveNimbusError(adUnitInstanceID: Int, error: Exception) {
            val jsonString = buildJsonObject {
                put("adUnitInstanceID", JsonPrimitive(adUnitInstanceID))
                put("errorMessage", JsonPrimitive(error.message))
            }.toString()
            sendMessageToUnity("OnError",jsonString)
        }

        private fun didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError) {
            val jsonString = buildJsonObject {
                put("adUnitInstanceID", JsonPrimitive(adUnitInstanceID))
                put("errorMessage", JsonPrimitive(error.message))
            }.toString()
            sendMessageToUnity("OnError",jsonString)
        }

        private fun sendMessageToUnity(methodName: String, params: String) {
            val nimbusCallbackGameObject = "NimbusCallbackReceiver"
            // Params: (GameObject name, Method name, String message)
            UnityPlayer.UnitySendMessage(nimbusCallbackGameObject, methodName, params)
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
@Serializable(with = APSAdUnitType.Companion::class)
enum class APSAdUnitType(val value: Int) {
    display320X50(0),
    display300X250(1),
    display728X90(2),
    interstitialDisplay(3),
    interstitialVideo(4),
    rewardedVideo(5);
    companion object : KSerializer<APSAdUnitType> {
        fun from(value: Int): APSAdUnitType = APSAdUnitType.entries.first { it.value == value }

        override val descriptor: SerialDescriptor =
            PrimitiveSerialDescriptor("adUnitType", PrimitiveKind.INT)

        override fun deserialize(decoder: Decoder): APSAdUnitType =
            from(decoder.decodeInt())

        override fun serialize(encoder: Encoder, value: APSAdUnitType) {
            encoder.encodeInt(value.value)
        }
    }
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