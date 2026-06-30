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
import com.adsbynimbus.request.RequestBuilder
import com.adsbynimbus.request.internal.AdUnitType
import com.adsbynimbus.rtb.CreativeAttribute
import com.adsbynimbus.rtb.Format
import com.adsbynimbus.rtb.Geo
import com.adsbynimbus.rtb.PlacementType
import com.adsbynimbus.rtb.PlaybackMethod
import com.adsbynimbus.rtb.Position
import com.adsbynimbus.unity.NimbusHelper.didReceiveNimbusError
import com.adsbynimbus.unity.NimbusHelper.sendMessageToUnity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlin.math.roundToInt
import org.json.*

object NimbusManager {

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
            val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyJson) ?: return
            NimbusUnityInternal.initNimbus(obj, enableSDKInTestMode, publisherKey, apiKey, extensions)
        }
    }


    @JvmStatic
    fun bannerAd(obj: Any?, instanceId: Int, position: String, adWidth: Int, adHeight: Int, refreshInterval: Int,
                 bidFloor: Float, respectSafeArea: Boolean, bannerPosition: Int, showAd: Boolean,
                 thirdPartyDemand: String, requestModifiersJson: String) {
        var ad: Ad
        if (obj !is Activity) {
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val adSize = getAdSizeFromDimens(adWidth, adHeight)
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
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers)
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
    fun interstitialAd(obj: Any?, instanceId: Int, position: String, bannerFloor: Float,
                       videoFloor: Float, showAd: Boolean, thirdPartyDemand: String, requestModifiersJson: String) {
        if (obj !is Activity) {
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Interstitial, extensions)
            val ad = Nimbus.fullscreenAd(position) {
                banner(AdSize.interstitial, bidFloor = bannerFloor)
                video(bidFloor = videoFloor)
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers)
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
    fun rewardedAd(obj: Any?, instanceId: Int, position: String, bidFloor: Float, showAd: Boolean,
                   thirdPartyDemand: String, requestModifiersJson: String) {
        if (obj !is Activity) {
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Rewarded, extensions)
            val ad = Nimbus.rewardedAd(position, videoBidFloor = bidFloor){
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers)
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

    @JvmStatic
    fun requestModifiersBlock(
        activity: Activity,
        adUnitInstanceId: Int,
        requestModifiers: JSONObject?
    ): RequestBuilder.() -> Unit {
        if (requestModifiers == null) {
            return {}
        }
        return {
            if (!(requestModifiers.isNull("app"))) {
                val appObj = requestModifiers.getJSONObject("app")
                val pageCat = appObj.getJSONArray("pageCat")
                val sectionCat = appObj.getJSONArray("sectionCat")
                app(Array(pageCat.length()) { pageCat.getString(it) }
                    , Array(sectionCat.length()) { sectionCat.getString(it) })
            }
            if (!(requestModifiers.isNull("banner"))) {
                val bannerObj = requestModifiers.getJSONObject("banner")
                var creativeAdSize: AdSize = AdSize.Banner
                if (!(bannerObj.isNull("width")) && !(bannerObj.isNull("height"))) {
                    creativeAdSize =
                        getAdSizeFromDimens(bannerObj.getInt("width"),
                            bannerObj.getInt("height"))
                }
                var addFormats: Set<Format> = emptySet()
                if (!(bannerObj.isNull("addFormats"))) {
                    val jsonArray = bannerObj.getJSONArray("addFormats")
                    addFormats = Array(jsonArray.length()) {
                            i ->
                        when(jsonArray.getInt(i)) {
                            1 -> Format.halfScreen
                            2 -> {
                                val orientation = activity.resources.configuration.orientation
                                when (orientation) {
                                    android.content.res.Configuration.ORIENTATION_LANDSCAPE
                                        -> Format.interstitialLandscape
                                    else ->
                                        Format.interstitialPortrait
                                }
                            }
                            3 -> Format.interstitialLandscape
                            4 -> Format.interstitialPortrait
                            5 -> Format.leaderboard
                            6 -> Format.mrec
                            else -> null
                        }
                    }.filterNotNull().toSet()
                }
                var adPosition: Position = Position.Unknown
                if (!(bannerObj.isNull("adPosition"))) {
                    adPosition = when(bannerObj.getInt("adPosition")) {
                        0 -> Position.AboveTheFold
                        1 -> Position.BelowTheFold
                        2 -> Position.Footer
                        3 -> Position.Fullscreen
                        4 -> Position.Header
                        5 -> Position.Sidebar
                        else ->
                            Position.Unknown
                    }
                }
                var bidFloor: Float = 0.0f
                if (!(bannerObj.isNull("bidFloor"))) {
                    bidFloor = bannerObj.getDouble("bidFloor").toFloat()
                }
                var battr: Set<CreativeAttribute> = emptySet()
                if (!(bannerObj.isNull("battr"))) {
                    val jsonArray = bannerObj.getJSONArray("battr")
                    battr = Array(jsonArray.length()) {
                            i ->
                        when(jsonArray.getInt(i)) {
                            1 -> CreativeAttribute.AdobeFlash
                            2 -> CreativeAttribute.AudioAdAutoPlay
                            3 -> CreativeAttribute.AudioAdUserInitiated
                            4 -> CreativeAttribute.ExpandableAutomatic
                            5 -> CreativeAttribute.ExpandableUserRollover
                            6 -> CreativeAttribute.HasVolumeToggle
                            7 -> CreativeAttribute.HasPopup
                            8 -> CreativeAttribute.BannerVideoAutoPlay
                            9 -> CreativeAttribute.BannerVideoUserInitiated
                            10 -> CreativeAttribute.ProvocativeOrSuggestive
                            11 -> CreativeAttribute.ExtremeAnimation
                            12 -> CreativeAttribute.Surveys
                            13 -> CreativeAttribute.TextOnly
                            14 -> CreativeAttribute.UserInteractiveAndGames
                            15 -> CreativeAttribute.DialogOrAlertStyle
                            else -> null
                        }
                    }.filterNotNull().toSet()
                }
                banner(creativeAdSize, addFormats, adPosition, bidFloor, battr)
            }
            if (!requestModifiers.isNull("environment")) {
                val env = requestModifiers.getJSONObject("environment")
                if (!env.isNull("publisherKey") && !env.isNull("apiKey")) {
                    environment(env.getString("publisherKey"),
                        env.getString("apiKey"))
                }
            }
            if (!requestModifiers.isNull("location")) {
                val location = requestModifiers.getJSONObject("location")
                var accuracy: Int?
                if (!location.isNull("accuracy")) {
                    accuracy = location.getInt("accuracy")
                }
                location(location.getDouble("latitude"),
                    location.getDouble("longitude"),
                    when (location.getInt("locationType")) {
                        1 -> Geo.LocationType.IpLookup
                        2 -> Geo.LocationType.UserProvided
                        else -> Geo.LocationType.Gps
                    }
                )
            }
            if (!requestModifiers.isNull("userKeywords")) {
                user(requestModifiers.getString("userKeywords"))
            }
            if (!requestModifiers.isNull("video")) {
                val video = requestModifiers.getJSONObject("video")
                var adPosition = Position.Unknown;
                if (!video.isNull("adPosition")) {
                    adPosition = when(video.getInt("adPosition"))
                    {
                        0 -> Position.AboveTheFold
                        1 -> Position.BelowTheFold
                        2 -> Position.Footer
                        3 -> Position.Fullscreen
                        4 -> Position.Header
                        5 -> Position.Sidebar
                        else ->
                        Position.Unknown
                    }
                }
                var bidFloor= 0.0f
                if (!video.isNull("bidFloor")) {
                    bidFloor = video.getDouble("bidFloor").toFloat()
                }
                var minDuration= 0
                if (!video.isNull("minDuration")) {
                    minDuration = video.getInt("minDuration")
                }
                var maxDuration = 0
                if (!video.isNull("maxDuration")) {
                    maxDuration = video.getInt("maxDuration")
                }
                var width = 0
                if (!video.isNull("width")) {
                    width = video.getInt("width")
                }
                var height = 0
                if (!video.isNull("height")) {
                    height = video.getInt("height")
                }
                var placementType: PlacementType? = null
                if (!video.isNull("placementType")) {
                    placementType = when(video.getInt("placementType")) {
                        0 -> PlacementType.InArticle
                        1 -> PlacementType.InBanner
                        2 -> PlacementType.InFeed
                        3 -> PlacementType.InStream
                        4 -> PlacementType.InterstitialSliderFloating
                        else -> null
                    }
                }
                var playbackMethod: Set<PlaybackMethod> = emptySet()
                if (!(video.isNull("playbackMethod"))) {
                    val jsonArray = video.getJSONArray("playbackMethod")
                    playbackMethod = Array(jsonArray.length()) {
                            i ->
                        when(jsonArray.getInt(i)) {
                            0 -> PlaybackMethod.ClickSoundOn
                            1 -> PlaybackMethod.EnterViewportSoundOff
                            2 -> PlaybackMethod.EnterViewportSoundOn
                            3 -> PlaybackMethod.MouseOverSoundOn
                            4 -> PlaybackMethod.PageLoadSoundOff
                            6 -> PlaybackMethod.PageLoadSoundOn
                            else -> null
                        }
                    }.filterNotNull().toSet()
                }
                video(adPosition, bidFloor, false, minDuration, maxDuration, width, height,
                    placementType, playbackMethod)
            }
            if (!requestModifiers.isNull("viewability")) {
                val va = requestModifiers.getJSONObject("viewability")
                if (!va.isNull("omidPn") && !va.isNull("omidPv")) {
                    // TODO: needs to be added in a newer version of 3.0
                    // viewability(va.getString("omidPn"), va.getString("omidPv"))
                }
            }
        }
    }

    private fun getAdSizeFromDimens(adWidth: Int, adHeight: Int): AdSize {
        var adSize = AdSize.Banner
        when (adWidth) {
            300 -> adSize = if (adHeight == 600) AdSize.HalfScreen else AdSize.Mrec
            320 -> adSize = if (adHeight == 480) AdSize.InterstitialPortrait else AdSize.Banner
            480 -> adSize = AdSize.InterstitialLandscape
            728 -> adSize = AdSize.Leaderboard
        }
        return adSize
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

}