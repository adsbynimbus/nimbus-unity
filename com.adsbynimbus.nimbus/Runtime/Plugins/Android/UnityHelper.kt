package com.adsbynimbus.unity

import android.app.Activity
import android.util.DisplayMetrics
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import androidx.collection.LruCache
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.updateLayoutParams
import com.adsbynimbus.*
import com.adsbynimbus.request.internal.AdUnitType
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlin.math.roundToInt
import org.json.*

object UnityHelper {

    private val adCache = LruCache<Int, Ad>(10)

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
                 bidFloor: Float, respectSafeArea: Boolean, bannerPosition: Int, showAd: Boolean, thirdPartyDemand: String) {
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
                NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Inline, extensions)
            ad = Nimbus.bannerAd(
                position = position,
                size = adSize,
                refreshInterval = refreshInterval,
                bidFloor = bidFloor
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
            adCache.put(instanceId, ad)
        }

    }

    @JvmStatic
    fun interstitialAd(obj: Any?, instanceId: Int, position: String, bannerFloor: Float, videoFloor: Float, showAd: Boolean, thirdPartyDemand: String) {
        if (obj !is Activity) {
            return
        }
        val extensions = extensionsFromJsonString(thirdPartyDemand)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Interstitial, extensions)
            val ad = Nimbus.fullscreenAd(position) {
                banner(AdSize.interstitial, bidFloor = bannerFloor)
                video(bidFloor = videoFloor)
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
            adCache.put(instanceId, ad)
        }
    }

    @JvmStatic
    fun rewardedAd(obj: Any?, instanceId: Int, position: String, bidFloor: Float, showAd: Boolean, thirdPartyDemand: String) {
        if (obj !is Activity) {
            return
        }
        val extensions = extensionsFromJsonString(thirdPartyDemand)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Rewarded, extensions)
            val ad = Nimbus.rewardedAd(position, videoBidFloor = bidFloor){
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
            adCache.put(instanceId, ad)
        }
    }

    @JvmStatic
    fun showAd(obj: Any?, instanceId: Int, adWidth: Int, adHeight: Int, respectSafeArea: Boolean, bannerPosition: Int) {
        val ad = adCache.get(instanceId)
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
    
    fun extensionsFromJsonString(thirdPartyDemand: String): JSONObject? {
        var extensions: JSONObject? = null
        if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty()) {
            try {
                extensions = JSONObject(thirdPartyDemand)
            } catch(e: Exception) {
                didReceiveNimbusError(0, e)
            }
        }
        return extensions
    }

    @JvmStatic
    fun destroyAd(adInstanceId: Int) {
        val ad = adCache.get(adInstanceId)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            ad?.destroy()
        }
        adCache.remove(adInstanceId)
    }

    @JvmStatic
    private fun sendRenderNimbusEvent(adUnitInstanceID: Int) {
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        sendMessageToUnity("OnAdRendered",json.toString())
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
            AdEvent.RewardEarned ->
            eventName = "REWARDEARNED"
            AdEvent.Completed ->
            eventName = "COMPLETED"
            AdEvent.Destroyed ->
            eventName = "DESTROYED"
            else ->
            return
        }
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        json.put("eventName", eventName)
        sendMessageToUnity("OnAdEvent",json.toString())
    }
    fun didReceiveNimbusError(adUnitInstanceID: Int, error: Exception) {
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        json.put("errorMessage", error.message)
        sendMessageToUnity("OnError",json.toString())
    }

    private fun didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError) {
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        json.put("errorMessage", error.message)
        sendMessageToUnity("OnError",json.toString())
    }

    private fun sendMessageToUnity(methodName: String, params: String) {
        val nimbusCallbackGameObject = "NimbusCallbackReceiver"
        // Params: (GameObject name, Method name, String message)
        UnityPlayer.UnitySendMessage(nimbusCallbackGameObject, methodName, params)
    }

}