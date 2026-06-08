using System;
using System.IO;
using System.Text;

namespace Nimbus.Editor
{
    internal class AndroidThirdPartyCreator
    {
        private const string Imports = @"package com.adsbynimbus.unity

        import android.app.Activity
        import com.adsbynimbus.request.DemandBuilder
        import com.adsbynimbus.*
        import com.adsbynimbus.extension.*
        import com.adsbynimbus.request.internal.AdUnitType";
        private const string BeginningOfClass = @"
        class NimbusUnityInternal {
            companion object {
        @JvmStatic
        fun initNimbus(activity: Activity, testMode: Boolean, publisherKey: String, apiKey: String, extensions: Extensions) {
         Nimbus.initialize(activity, publisherKey, apiKey) {";
        
        internal static void WriteDemandFile(string path)
        {
            var builder = new StringBuilder();
            //start init function
            builder.AppendLine(Imports);
            #if NIMBUS_ENABLE_APS_ANDROID
            builder.AppendLine(@"import com.amazon.device.ads.AdRegistration
                            import com.amazon.device.ads.MRAIDPolicy
                            import com.amazon.device.ads.DTBAdNetwork
                            import com.amazon.device.ads.DTBAdNetworkInfo
                            import com.amazon.device.ads.DTBAdRequest
                            import com.amazon.device.ads.DTBAdSize");
            #endif
            #if NIMBUS_ENABLE_LIVERAMP_ANDROID
            builder.AppendLine(@"import kotlinx.coroutines.CoroutineScope
                                import kotlinx.coroutines.Dispatchers
                                import kotlinx.coroutines.launch");
            #endif
            builder.AppendLine(BeginningOfClass);
            #if NIMBUS_ENABLE_ADMOB_ANDROID
                builder.AppendLine(@"AdMobExtension()");
            #endif
            #if NIMBUS_ENABLE_INMOBI_ANDROID
                builder.AppendLine(@"extensions.inMobi?.accountId.let {
                    InMobiExtension(it)
                }");
            #endif
            #if NIMBUS_ENABLE_META_ANDROID
                builder.AppendLine(@"extensions.meta?.appId.let {
                    MetaExtension.forceTestAd = extensions.meta?.forceTestAd ?: false
                    MetaExtension(it)
                }");
            #endif
            #if NIMBUS_ENABLE_MINTEGRAL_ANDROID
                builder.AppendLine(@"extensions.mintegral?.appId?.let { appId ->
                        extensions.mintegral.appKey?.let { appKey ->
                            MintegralExtension(appId, appKey)
                        }
                    }");
            #endif
            #if NIMBUS_ENABLE_MOBILEFUSE_ANDROID
                builder.AppendLine(@"MobileFuseExtension()");
            #endif
            #if NIMBUS_ENABLE_MOLOCO_ANDROID
                builder.AppendLine(@"extensions.moloco?.appKey.let {
                        MolocoExtension(it)
                    }");
            #endif
            #if NIMBUS_ENABLE_UNITY_ADS_ANDROID
                builder.AppendLine(@"extensions.unityAds?.gameId.let {
                    UnityExtension(it)
                }"); 
            #endif
            #if NIMBUS_ENABLE_VUNGLE_ANDROID
                builder.AppendLine(@"extensions.vungle?.appId.let {
                        VungleExtension(it)
                    }");
            #endif
            builder.AppendLine(@"}");
            //init aps outside of init block
            #if NIMBUS_ENABLE_APS_ANDROID
                builder.AppendLine(@"extensions.aps?.appKey?.let {
                    initAPS(activity, it, testMode)
                }");
            #endif
            builder.AppendLine(@"}");
            //end init function
            //APS Init function
            #if NIMBUS_ENABLE_APS_ANDROID
                builder.AppendLine(@"@JvmStatic
                fun initAPS(activity: Activity, appKey: String, testMode: Boolean) {
                    AdRegistration.getInstance(appKey, activity)

                    AdRegistration.setMRAIDSupportedVersions(arrayOf(""1.0"", ""2.0"", ""3.0""))
                    AdRegistration.setMRAIDPolicy(MRAIDPolicy.CUSTOM)

                    AdRegistration.addCustomAttribute(""omidPartnerName"", Nimbus.sdkName)
                    AdRegistration.addCustomAttribute(""omidPartnerVersion"", Nimbus.version)

                    AdRegistration.enableLogging(testMode)
                    AdRegistration.enableTesting(testMode)
                }");
            #endif

            builder.AppendLine(@"@JvmStatic
                suspend fun demandBlock(activity: Activity, adType: AdUnitType, extensions: Extensions?): DemandBuilder.() -> Unit {
                    if (extensions == null) {
                        return {}
                    }");
            #if NIMBUS_ENABLE_APS_ANDROID
                builder.AppendLine(@"val aps = apsDemand(activity, extensions)
                    return { 
                        aps()");
            #else
                builder.AppendLine(@"return{");
            #endif
            #if NIMBUS_ENABLE_ADMOB_ANDROID
            builder.AppendLine(@"adMobDemand(adType, extensions)()");
            #endif
            builder.AppendLine(@"}
                }");
            #if NIMBUS_ENABLE_APS_ANDROID
            builder.AppendLine(@"@JvmStatic
                suspend fun apsDemand(activity: Activity, extensions: Extensions): DemandBuilder.() -> Unit {
                    if (extensions.aps?.slotData?.isNotEmpty() ?: false) {
                        val apsRequests: ArrayList<DTBAdRequest> = arrayListOf()
                        for (slot in extensions.aps.slotData) {
                            slot?.slotId.let { uuid ->
                                when(slot?.adUnitType) {
                                    APSAdUnitType.display320X50 -> apsRequests.add(
                                        DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(DTBAdSize(320, 50, uuid))
                                    })
                                    APSAdUnitType.display300X250 -> apsRequests.add(
                                        DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                            setSizes(DTBAdSize(300, 250, uuid))
                                        })
                                    APSAdUnitType.display728X90 -> apsRequests.add(
                                        DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                            setSizes(DTBAdSize(728, 90, uuid))
                                        })
                                    APSAdUnitType.interstitialDisplay -> apsRequests.add(
                                        DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                            setSizes(DTBAdSize.DTBInterstitialAdSize(uuid))
                                        })
                                    APSAdUnitType.interstitialVideo, APSAdUnitType.rewardedVideo -> apsRequests.add(
                                        DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                            setSizes(DTBAdSize.DTBVideo(
                                                activity.resources.displayMetrics.widthPixels,
                                                activity.resources.displayMetrics.heightPixels, uuid))
                                        })
                                    null -> continue
                                }
                            }
                        }
                        val params = APSFetcher(*apsRequests.toTypedArray()).fetchAds()
                        return {
                            aps(params, apsRequests)
                        }
                    }
                    return {}
                }");
            #endif
            #if NIMBUS_ENABLE_ADMOB_ANDROID
            builder.AppendLine(@"@JvmStatic
                fun adMobDemand(adType: AdUnitType, extensions: Extensions): DemandBuilder.() -> Unit {
                    extensions.adMob?.let {
                        extensions.adMob.adUnitIds?.let { 
                            if (it.isNotEmpty()) {
                                when (adType){
                                    AdUnitType.Inline ->  { 
                                        if (!it.first().isNullOrEmpty()) {
                                            return {admobBanner(it.first() ?: """")} }
                                        }
                                    AdUnitType.Interstitial -> {
                                        if (!it.first().isNullOrEmpty()) {
                                            return {admobInterstitial(it.first() ?: """")} }
                                    }
                                    AdUnitType.Rewarded -> {
                                        if (!it.first().isNullOrEmpty()) {
                                            return {admobRewarded(it.first() ?: """")} }
                                    }
                                    else -> {
                                        return {}
                                    }
                                }
                            }
                        }
                    }
                    return {}
                }");
            #endif
            
            //LiveRamp Init Function
            #if NIMBUS_ENABLE_LIVERAMP_ANDROID
            builder.AppendLine(@"
                @JvmStatic
                fun initLiveRamp(configId: String, email: String, hasConsentForNoLegislation: Boolean, isTestMode: Boolean) {
                    val scope = CoroutineScope(Dispatchers.Main)
                    scope.launch {
                        LiveRamp(
                            configId = configId,
                            email = email,
                            hasConsentForNoLegislation = hasConsentForNoLegislation
                        ).fetchEnvelope(isTestMode)?.applyToNimbus()
                    }
                }");
            #endif
            builder.AppendLine("}}");
            File.WriteAllText(path, builder.ToString());
        }
    }
}