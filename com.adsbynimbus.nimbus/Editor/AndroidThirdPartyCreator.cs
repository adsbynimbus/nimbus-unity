using System.IO;
using System.Text;

namespace AdsByNimbus.Editor
{
    internal class AndroidThirdPartyCreator
    {
        private const string Imports = @"package com.adsbynimbus.unity

        import android.app.Activity
        import com.adsbynimbus.request.DemandBuilder
        import com.adsbynimbus.*
        import com.adsbynimbus.extension.*
        import com.adsbynimbus.request.internal.AdUnitType
        import org.json.JSONException
        import org.json.JSONObject";
        private const string BeginningOfClass = @"
        object NimbusUnityInternal {
        @JvmStatic
        fun initNimbus(activity: Activity, testMode: Boolean, publisherKey: String, apiKey: String, extensions: JSONObject) {
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
                builder.AppendLine(@"try {
                    InMobiExtension(extensions.getString(""inMobiAccountId""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }");
            #endif
            #if NIMBUS_ENABLE_META_ANDROID
                builder.AppendLine(@"try {
                    MetaExtension.forceTestAd = extensions.getBoolean(""metaForceTestAd"")
                    MetaExtension(extensions.getString(""metaAppId""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }");
            #endif
            #if NIMBUS_ENABLE_MINTEGRAL_ANDROID
                builder.AppendLine(@"try {
                    MintegralExtension(extensions.getString(""mintegralAppId""), 
                        extensions.getString(""mintegralAppKey""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }");
            #endif
            #if NIMBUS_ENABLE_MOBILEFUSE_ANDROID
                builder.AppendLine(@"MobileFuseExtension()");
            #endif
            #if NIMBUS_ENABLE_MOLOCO_ANDROID
                builder.AppendLine(@"try {
                    MolocoExtension(extensions.getString(""molocoAppKey""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }");
            #endif
            #if NIMBUS_ENABLE_UNITY_ADS_ANDROID
                builder.AppendLine(@"try {
                    UnityExtension(extensions.getString(""unityAdsGameId""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }"); 
            #endif
            #if NIMBUS_ENABLE_VUNGLE_ANDROID
                builder.AppendLine(@"try {
                    VungleExtension(extensions.getString(""vungleAppId""))
                } catch (e: JSONException) {
                    NimbusHelper.didReceiveNimbusError(0, e)
                }");
            #endif
            builder.AppendLine(@"}");
            //init aps outside of init block
            #if NIMBUS_ENABLE_APS_ANDROID
                builder.AppendLine(@"try {
                initAPS(activity, extensions.getString(""apsAppKey""), testMode)
            } catch (e: JSONException) {
                NimbusHelper.didReceiveNimbusError(0, e)
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
                suspend fun demandBlock(
                    activity: Activity,
                    adUnitInstanceId: Int,
                    adType: AdUnitType,
                    extensions: JSONObject?
                ): DemandBuilder.() -> Unit {
                    if (extensions == null) {
                        return {}
                    }");
            #if NIMBUS_ENABLE_APS_ANDROID
                builder.AppendLine(@"val aps = apsDemand(activity, adUnitInstanceId, extensions)
                    return { 
                        aps()");
            #else
                builder.AppendLine(@"return{");
            #endif
            #if NIMBUS_ENABLE_ADMOB_ANDROID
            builder.AppendLine(@"adMobDemand(adUnitInstanceId, adType, extensions)()");
            #endif
            builder.AppendLine(@"}
                }");
            #if NIMBUS_ENABLE_APS_ANDROID
            builder.AppendLine(@"@JvmStatic
            suspend fun apsDemand(
                activity: Activity,
                adUnitInstanceId: Int,
                extensions: JSONObject
            ): DemandBuilder.() -> Unit {
                try {
                    if (extensions.has(""apsSlotData"")) {
                        val apsRequests: ArrayList<DTBAdRequest> = arrayListOf()
                        val apsSlotData = extensions.getJSONArray(""apsSlotData"")
                        for (i in 0 until apsSlotData.length()) {
                            val slot = apsSlotData.getJSONObject(i)
                            when (slot.getInt(""adUnitType"")) {
                                0 -> apsRequests.add(
                                    DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(DTBAdSize(320, 50, slot.getString(""slotId"")))
                                    })

                                1 -> apsRequests.add(
                                    DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(DTBAdSize(300, 250, slot.getString(""slotId"")))
                                    })

                                2 -> apsRequests.add(
                                    DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(DTBAdSize(728, 90, slot.getString(""slotId"")))
                                    })

                                3 -> apsRequests.add(
                                    DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(DTBAdSize.DTBInterstitialAdSize(slot.getString(""slotId"")))
                                    })

                                4, 5 -> apsRequests.add(
                                    DTBAdRequest(DTBAdNetworkInfo(DTBAdNetwork.NIMBUS)).apply {
                                        setSizes(
                                            DTBAdSize.DTBVideo(
                                                activity.resources.displayMetrics.widthPixels,
                                                activity.resources.displayMetrics.heightPixels,
                                                slot.getString(""slotId"")
                                            )
                                        )
                                    })

                            }
                        }
                        val params = APSFetcher(*apsRequests.toTypedArray()).fetchAds()
                        return {
                            aps(params, apsRequests)
                        }
                    }
                } catch (e: Exception) {
                    NimbusHelper.didReceiveNimbusError(adUnitInstanceId, e)
                }
                return {}
            }");
            #endif
            #if NIMBUS_ENABLE_ADMOB_ANDROID
            builder.AppendLine(@"@JvmStatic
            fun adMobDemand(adUnitInstanceId: Int, adType: AdUnitType, extensions: JSONObject): DemandBuilder.() -> Unit {
                try {
                    if (extensions.has(""adMobAdUnitIds"")) {
                        val jsonArray = extensions.getJSONArray(""adMobAdUnitIds"")
                        val adMobAdUnitIds = Array(jsonArray.length()) { i ->
                            jsonArray.getString(i)
                        }
                        if (adMobAdUnitIds.isNotEmpty()) {
                            when (adType) {
                                AdUnitType.Inline -> {
                                    if (adMobAdUnitIds.first().isNullOrEmpty()) {
                                        return { admobBanner(adMobAdUnitIds.first() ?: """") }
                                    }
                                }

                                AdUnitType.Interstitial -> {
                                    if (adMobAdUnitIds.first().isNullOrEmpty()) {
                                        return { admobInterstitial(adMobAdUnitIds.first() ?: """") }
                                    }
                                }

                                AdUnitType.Rewarded -> {
                                    if (adMobAdUnitIds.first().isNullOrEmpty()) {
                                        return { admobRewarded(adMobAdUnitIds.first() ?: """") }
                                    }
                                }

                                else -> {
                                    return {}
                                }
                            }
                        }
                    }
                } catch (e: Exception) {
                    NimbusHelper.didReceiveNimbusError(adUnitInstanceId, e)
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
            builder.AppendLine("}");
            File.WriteAllText(path, builder.ToString());
        }
    }
}