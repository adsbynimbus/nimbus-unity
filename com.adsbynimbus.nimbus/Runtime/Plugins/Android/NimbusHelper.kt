package com.adsbynimbus.unity

import com.adsbynimbus.Configuration
import com.adsbynimbus.Nimbus
import com.adsbynimbus.NimbusError
import com.adsbynimbus.NimbusResponse
import com.adsbynimbus.rtb.App
import com.adsbynimbus.rtb.Publisher
import com.adsbynimbus.rtb.User
import com.iab.omid.library.adsbynimbus.adsession.VerificationScriptResource
import com.unity3d.player.UnityPlayer
import org.json.JSONObject
import java.net.URL
import kotlin.time.Duration.Companion.milliseconds

object NimbusHelper {

    @JvmStatic
    fun setSessionId(sessionId: String) {
        Nimbus.configuration.sessionId = sessionId
    }

    @JvmStatic
    fun setCoppa(coppa: Boolean) {
        Nimbus.configuration.coppa = coppa
    }

    @JvmStatic
    fun setApp(appJsonStr: String) {
        val appJson = jsonObjFromJsonString(appJsonStr)
        val app = App()
        if (appJson?.isNull("bundle") == false) {
            app.bundle = appJson.getString("bundle")
        }
        if (appJson?.isNull("cat") == false) {
            val catArray = appJson.getJSONArray("cat")
            app.cat = Array(catArray.length()) { i -> catArray.getString(i) }
        }
        if (appJson?.isNull("domain") == false) {
            app.domain = appJson.getString("domain")
        }
        if (appJson?.isNull("name") == false) {
            app.name = appJson.getString("name")
        }
        if (appJson?.isNull("pagecat") == false) {
            val catArray = appJson.getJSONArray("pagecat")
            app.pagecat = Array(catArray.length()) { i -> catArray.getString(i) }
        }
        if (appJson?.isNull("paid") == false) {
            app.paid = if (appJson.getBoolean("paid")) 1 else 0
        }
        if (appJson?.isNull("privacypolicy") == false) {
            app.privacypolicy = if (appJson.getBoolean("privacypolicy")) 0 else 1
        }
        if (appJson?.isNull("publisher") == false) {
            val pubJson = appJson.getJSONObject("publisher")
            val publisher = Publisher()
            if (!pubJson.isNull("cat")) {
                val catArray = appJson.getJSONArray("cat")
                publisher.cat = Array(catArray.length()) { i -> catArray.getString(i) }
            }
            if (!pubJson.isNull("domain")) {
                publisher.domain = pubJson.getString("domain")
            }
            if (!pubJson.isNull("name")) {
                publisher.name = pubJson.getString("name")
            }
            app.publisher = publisher
        }
        if (appJson?.isNull("sectioncat") == false) {
            val catArray = appJson.getJSONArray("sectioncat")
            app.sectioncat = Array(catArray.length()) { i -> catArray.getString(i) }
        }
        if (appJson?.isNull("storeurl") == false) {
            app.storeurl = appJson.getString("storeurl")
        }
        if (appJson?.isNull("ver") == false) {
            app.ver = appJson.getString("ver")
        }
        Nimbus.configuration.app = app
    }

    @JvmStatic
    fun setUser(userJsonStr: String) {
        val userJson = jsonObjFromJsonString(userJsonStr)
        val user = User()
        if (userJson?.isNull("age") == false) {
            user.age = userJson.getInt("age")
        }
        if (userJson?.isNull("customData") == false) {
            user.custom_data = userJson.getString("customData")
        }
        if (userJson?.isNull("gender") == false) {
            val genderStr = userJson.getString("gender")
            user.gender = when (genderStr) {
                "M" -> User.Gender.Male
                "F" -> User.Gender.Female
                "O" -> User.Gender.Other
                else -> null
            }
        }
        if (userJson?.isNull("keywords") == false) {
            user.keywords = userJson.getString("keywords")
        }
        Nimbus.configuration.user = user
    }

    @JvmStatic
    // comma delimited string coming from c#
    fun setBlockedAdvertisingDomains(blockedAdvertisingDomains: String) {
        Nimbus.configuration.blockedAdvertisingDomains = blockedAdvertisingDomains.split(",").toSet()
    }

    @JvmStatic
    fun setRequestUrl(requestUrl: String) {
        Nimbus.configuration.requestUrl = requestUrl
    }

    @JvmStatic
    fun setAdditionalRequestHeaders(additionalRequestHeadersJson: String) {
        val mutableMap = mutableMapOf<String, String>()
        val headers = jsonObjFromJsonString(additionalRequestHeadersJson)
        headers?.keys()?.forEach { key ->
            mutableMap[key] = headers.getString(key) ?: ""
        }
        Nimbus.configuration.additionalRequestHeaders += mutableMap
    }


    @JvmStatic
    fun setInterceptorTimeout(interceptorTimeout: Int) {
        Nimbus.configuration.interceptorsTimeout = interceptorTimeout.milliseconds
    }

    @JvmStatic
    fun showMuteButton(showMuteButton: Boolean) {
        Nimbus.configuration.showMuteButton = showMuteButton
    }

    @JvmStatic
    fun setGdprProperties(gdprApplies: Boolean, consentString: String) {
        Nimbus.IAB.gdprApplies = gdprApplies
        Nimbus.IAB.tcfString = consentString
    }

    @JvmStatic
    fun setGppProperties(gppSectionId: String, gppConsentString: String) {
        Nimbus.IAB.gppSID = gppSectionId
        Nimbus.IAB.gppString = gppConsentString
    }

    @JvmStatic
    fun setUsPrivacyString(privacyString: String) {
        Nimbus.IAB.usPrivacyString = privacyString
    }

    @JvmStatic
    fun setVerificationProviders(callbacks: Array<VerificationProviderCallbackInterface>) {
        val vps: MutableList<Configuration.VerificationProvider> = mutableListOf()
        for (callback in callbacks) {
            val vProvider = object: Configuration.VerificationProvider {
                override fun verificationMarkup(ad: NimbusResponse): String {
                    return callback._verificationMarkupCallback(ad.bid.adm)
                }

                override fun verificationResource(ad: NimbusResponse): VerificationScriptResource {
                    val results = jsonObjFromJsonString(callback._verificationResourceCallback(ad.bid.adm))
                    return VerificationScriptResource.createVerificationScriptResourceWithParameters(results?.getString("vendorKey"),
                        URL(results?.getString("url")), results?.getString("parameters"))
                }
            }
            vps.add(vProvider)
        }
        Nimbus.configuration.verificationProviders.addAll(vps)
    }

    fun jsonObjFromJsonString(thirdPartyDemand: String): JSONObject? {
        var extensions: JSONObject? = null
        if (thirdPartyDemand != "" && !thirdPartyDemand.isEmpty()) {
            try {
                extensions = JSONObject(thirdPartyDemand)
            } catch(e: Exception) {
                didReceiveNimbusError(-1, e)
            }
        }
        return extensions
    }

    fun didReceiveNimbusError(adUnitInstanceID: Int, error: Exception) {
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        json.put("errorMessage", error.message)
        sendMessageToUnity("OnError",json.toString())
    }

    fun didReceiveNimbusError(adUnitInstanceID: Int, error: NimbusError) {
        val json = JSONObject()
        json.put("adUnitInstanceID", adUnitInstanceID)
        json.put("errorMessage", error.message)
        sendMessageToUnity("OnError",json.toString())
    }

    fun sendMessageToUnity(methodName: String, params: String) {
        val nimbusCallbackGameObject = "NimbusCallbackReceiver"
        // Params: (GameObject name, Method name, String message)
        UnityPlayer.UnitySendMessage(nimbusCallbackGameObject, methodName, params)
    }

}

//methods are named this way because they have to explicitly match the method names in c#
interface VerificationProviderCallbackInterface {
    fun _verificationMarkupCallback(response: String): String
    fun _verificationResourceCallback(response: String) : String
}



