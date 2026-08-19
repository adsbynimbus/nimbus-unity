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
import com.adsbynimbus.rtb.Content
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
            Nimbus.configuration.testMode = enableSDKInTestMode
            val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyJson) ?: return
            NimbusUnityInternal.initNimbus(obj, enableSDKInTestMode, publisherKey, apiKey, extensions)
        }
    }


    @JvmStatic
    fun bannerAd(obj: Any?, instanceId: Int, position: String, adWidth: Int, adHeight: Int,
                 addFormats: String, adPosition: Int, bidFloor: Float, refreshInterval: Int,
                 bannerPosition: Int, xCoord: Int, yCoord: Int, respectSafeArea: Boolean, thirdPartyDemand: String,
                 requestModifiersJson: String, showAd: Boolean) {
        var ad: Ad
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "bannerAd: object sent for first param is not an Activity"))
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
                addFormats = getAddFormatsFromString(obj, addFormats),
                adPosition = getAdPositionFromInt(adPosition),
                refreshInterval = refreshInterval,
                bidFloor = bidFloor
            ) {
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers).invoke(this)
                }
            }
                .onEvent { event ->
                    didReceiveNimbusEvent(instanceId, event)
                }.onError { error ->
                    didReceiveNimbusError(instanceId, error)
                }
            adCache.put(instanceId, ad)
            if (showAd) {
                showBannerAd(obj, ad, adWidth, adHeight, respectSafeArea, bannerPosition, xCoord, yCoord)
                sendRenderNimbusEvent(instanceId)
            } else {
                ad.load()
            }
        }
    }

    @JvmStatic
    fun dynamicUnit(obj: Any?, instanceId: Int, position: String,
                    addFormats: String, adPosition: Int, bidFloor: Float, refreshInterval: Int,
                    bannerPosition: Int,  xCoord: Int, yCoord: Int, respectSafeArea: Boolean, thirdPartyDemand: String,
                    requestModifiersJson: String, showAd: Boolean) {
        var ad: Ad
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "dynamicUnit: object sent for first param is not an Activity"))
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock =
                NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Inline, extensions)
            ad = Nimbus.dynamicUnit(
                position = position,
                addFormats = getAddFormatsFromString(obj, addFormats),
                adPosition = getAdPositionFromInt(adPosition),
                refreshInterval = refreshInterval,
                bidFloor = bidFloor
            ) {
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers).invoke(this)
                }
            }
                .onEvent { event ->
                    didReceiveNimbusEvent(instanceId, event)
                }.onError { error ->
                    didReceiveNimbusError(instanceId, error)
                }
            adCache.put(instanceId, ad)
            if (showAd) {
                showBannerAd(obj, ad, 0, 480, respectSafeArea, bannerPosition, xCoord, yCoord, true)
                sendRenderNimbusEvent(instanceId)
            } else {
                ad.load()
            }
        }
    }

    @JvmStatic
    fun interstitialAd(obj: Any?, instanceId: Int, position: String, addFormats: String,
                       orientation: Int, bidFloor: Float, thirdPartyDemand: String,
                       requestModifiersJson: String, showAd: Boolean) {
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "interstitialAd: object sent for first param is not an Activity"))
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,
                AdUnitType.Interstitial, extensions)
            val ad = Nimbus.interstitialAd(
                position = position,
                addFormats = getAddFormatsFromString(obj, addFormats),
                orientation = getOrientationFromInt(orientation),
                bidFloor = bidFloor
            ) {
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers).invoke(this)
                }
            }.onEvent { event ->
                didReceiveNimbusEvent(instanceId, event)
            }.onError { error ->
                didReceiveNimbusError(instanceId, error)
            }
            adCache.put(instanceId, ad)
            if (showAd) {
                ad.show(obj)
                sendRenderNimbusEvent(instanceId)
            } else {
                ad.load(obj)
            }
        }
    }

    @JvmStatic
    fun fullScreenAd(obj: Any?, instanceId: Int, position: String,
                     orientation: Int, thirdPartyDemand: String,
                     requestModifiersJson: String, showAd: Boolean) {
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "fullScreenAd: object sent for first param is not an Activity"))
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,
                AdUnitType.Interstitial, extensions)
            val ad = Nimbus.fullscreenAd(
                position = position,
                orientation = getOrientationFromInt(orientation),
            ) {
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers).invoke(this)
                }
            }.onEvent { event ->
                didReceiveNimbusEvent(instanceId, event)
            }.onError { error ->
                didReceiveNimbusError(instanceId, error)
            }
            adCache.put(instanceId, ad)
            if (showAd) {
                ad.show(obj)
                sendRenderNimbusEvent(instanceId)
            } else {
                ad.load(obj)
            }
        }
    }

    @JvmStatic
    fun rewardedAd(obj: Any?, instanceId: Int, position: String, orientation: Int, bidFloor: Float,
                   thirdPartyDemand: String, requestModifiersJson: String, showAd: Boolean) {
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "rewardedAd: object sent for first param is not an Activity"))
            return
        }
        val extensions = NimbusHelper.jsonObjFromJsonString(thirdPartyDemand)
        val requestModifiers = NimbusHelper.jsonObjFromJsonString(requestModifiersJson)
        val scope = CoroutineScope(Dispatchers.Main)
        scope.launch {
            val demandBlock = NimbusUnityInternal.demandBlock(obj, instanceId,AdUnitType.Rewarded, extensions)
            val ad = Nimbus.rewardedAd(
                position = position,
                bidFloor = bidFloor,
                orientation = getOrientationFromInt(orientation)){
                demand {
                    demandBlock()
                }
                if (requestModifiers != null) {
                    requestModifiersBlock(obj, instanceId, requestModifiers).invoke(this)
                }
            }.onEvent { event ->
                didReceiveNimbusEvent(instanceId, event)
            }.onError { error ->
                didReceiveNimbusError(instanceId, error)
            }
            adCache.put(instanceId, ad)
            if (showAd) {
                ad.show(obj)
                sendRenderNimbusEvent(instanceId)
            } else {
                ad.load(obj)
            }
        }
    }

    @JvmStatic
    fun showAd(obj: Any?, instanceId: Int, adWidth: Int, adHeight: Int, respectSafeArea: Boolean, bannerPosition: Int, 
        xCoord: Int, yCoord: Int) {
        val ad = adCache[instanceId] ?: return
        if (obj !is Activity) {
            didReceiveNimbusError(-1, NimbusError(
                "showAd: object sent for first param is not an Activity"))
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
    suspend fun showBannerAd(obj: Activity, ad: InlineAd, adWidth: Int, adHeight: Int,
                             respectSafeArea: Boolean, bannerPosition: Int, xCoord: Int?, yCoord: Int,
                             dynamicUnit: Boolean = false) {
        val adFrame = FrameLayout(obj)
        val width = if (adWidth == 0)  obj.resources.displayMetrics.widthPixels else dpToPx(adWidth, obj)
        val height = if (adHeight == 0) obj.resources.displayMetrics.heightPixels else dpToPx(adHeight, obj)
        obj.addContentView(
            adFrame, FrameLayout.LayoutParams(
                width,
                height))
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
                            1 -> Format.banner
                            2 -> Format.mrec
                            3 -> Format.halfScreen
                            4 -> Format.leaderboard
                            5 -> Format.interstitialPortrait
                            6 -> Format.interstitialLandscape
                            7 -> {
                                val orientation = activity.resources.configuration.orientation
                                when (orientation) {
                                    android.content.res.Configuration.ORIENTATION_LANDSCAPE
                                        -> Format.interstitialLandscape
                                    else ->
                                        Format.interstitialPortrait
                                }
                            }
                            else -> null
                        }
                    }.filterNotNull().toSet()
                }
                var adPosition: Position = Position.Unknown
                if (!(bannerObj.isNull("adPosition"))) {
                    adPosition = when(bannerObj.getInt("adPosition")) {
                        1 -> Position.AboveTheFold
                        2 -> Position.BelowTheFold
                        3 -> Position.Header
                        4 -> Position.Footer
                        5 -> Position.Sidebar
                        6 -> Position.Fullscreen
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
                            1 -> CreativeAttribute.AudioAdAutoPlay
                            2 -> CreativeAttribute.AudioAdUserInitiated
                            3 -> CreativeAttribute.ExpandableAutomatic
                            4 -> CreativeAttribute.ExpandableUserClick
                            5 -> CreativeAttribute.ExpandableUserRollover
                            6 -> CreativeAttribute.BannerVideoAutoPlay
                            7 -> CreativeAttribute.BannerVideoUserInitiated
                            8 -> CreativeAttribute.HasPopup
                            9 -> CreativeAttribute.ProvocativeOrSuggestive
                            10 -> CreativeAttribute.ExtremeAnimation
                            11 -> CreativeAttribute.Surveys
                            12 -> CreativeAttribute.TextOnly
                            13 -> CreativeAttribute.UserInteractiveAndGames
                            14 -> CreativeAttribute.DialogOrAlertStyle
                            15 -> CreativeAttribute.HasVolumeToggle
                            16 -> CreativeAttribute.HasSkipButton
                            17 -> CreativeAttribute.AdobeFlash
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
                        2 -> Geo.LocationType.IpLookup
                        3 -> Geo.LocationType.UserProvided
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
                        1 -> Position.AboveTheFold
                        2 -> Position.BelowTheFold
                        3 -> Position.Header
                        4 -> Position.Footer
                        5 -> Position.Sidebar
                        6 -> Position.Fullscreen
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
                        1 -> PlacementType.InStream
                        2 -> PlacementType.InBanner
                        3 -> PlacementType.InArticle
                        4 -> PlacementType.InFeed
                        5 -> PlacementType.InterstitialSliderFloating
                        else -> null
                    }
                }
                var playbackMethod: Set<PlaybackMethod> = emptySet()
                if (!(video.isNull("playbackMethod"))) {
                    val jsonArray = video.getJSONArray("playbackMethod")
                    playbackMethod = Array(jsonArray.length()) {
                            i ->
                        when(jsonArray.getInt(i)) {
                            1 -> PlaybackMethod.PageLoadSoundOn
                            2 -> PlaybackMethod.PageLoadSoundOff
                            3 -> PlaybackMethod.ClickSoundOn
                            4 -> PlaybackMethod.MouseOverSoundOn
                            5 -> PlaybackMethod.EnterViewportSoundOn
                            6 -> PlaybackMethod.EnterViewportSoundOff
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
                    viewability(va.getString("omidPn"), va.getString("omidPv"))
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

    private fun getAddFormatsFromString(activity: Activity, addFormatsStr: String): Set<Format> {
        if (addFormatsStr.isBlank()) return emptySet()

        return addFormatsStr
            .split(",")                         // 1. Split by comma
            .mapNotNull { it.trim().toIntOrNull() } // 2. Trim spaces and safely convert to Int
            .mapNotNull {
                when(it) {
                    1 -> Format.banner
                    2 -> Format.mrec
                    3 -> Format.halfScreen
                    4 -> Format.leaderboard
                    5 -> Format.interstitialPortrait
                    6 -> Format.interstitialLandscape
                    7 -> {
                        val orientation = activity.resources.configuration.orientation
                        when (orientation) {
                            android.content.res.Configuration.ORIENTATION_LANDSCAPE
                                -> Format.interstitialLandscape
                            else ->
                                Format.interstitialPortrait
                        }
                    }
                    else -> null
                }
            }     // 3. Look up Enum, ignore if not found
            .toSet()
    }

    private fun getAdPositionFromInt(adPosition: Int): Position{
        return when(adPosition) {
            1 -> Position.AboveTheFold
            2 -> Position.BelowTheFold
            3 -> Position.Header
            4 -> Position.Footer
            5 -> Position.Sidebar
            6 -> Position.Fullscreen
            else ->
                Position.Unknown
        }
    }

    private fun getOrientationFromInt(orientation: Int): AdOrientation {
        return when(orientation) {
            0 -> AdOrientation.Portrait
            1 -> AdOrientation.Landscape
            else ->
                AdOrientation.DeviceOrientation
        }
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