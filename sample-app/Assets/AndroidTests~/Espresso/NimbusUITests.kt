package com.adsbynimbus.unity

import androidx.test.platform.app.InstrumentationRegistry
import androidx.test.ext.junit.runners.AndroidJUnit4
import com.adsbynimbus.Nimbus
import com.adsbynimbus.rtb.User

import org.junit.Test
import org.junit.runner.RunWith
import org.junit.Assert.*
import kotlin.time.Duration.Companion.milliseconds

/**
 * Instrumented test, which will execute on an Android device or emulator.
 */
@RunWith(AndroidJUnit4::class)
class NimbusUITests {

    @Test
    fun testSetCoppa() {
        NimbusHelper.setCoppa(true)
        assertTrue(Nimbus.configuration.coppa)
    }

    @Test
    fun testSetApp() {
        val appJson = "{\"bundle\":\"com.test.nimbusUnity\",\"cat\":[],\"domain\":\"nimbus.co\",\"name\":\"testapp\",\"pagecat\":[],\"publisher\":{\"cat\":[],\"domain\":\"nimbus.co\",\"name\":\"nimbus\"},\"sectioncat\":[],\"storeurl\":\"www.nimbus.co\",\"ver\":\"3.0.0\",\"paid\":1,\"privacypolicy\":1}"
        NimbusHelper.setApp(appJsonStr = appJson)
        assert(Nimbus.configuration.app?.bundle == "com.test.nimbusUnity")
        assert(Nimbus.configuration.app?.paid == 1.toByte())
        assert(Nimbus.configuration.app?.publisher?.domain == "nimbus.co")
    }

    @Test
    fun testSetUser() {
        val userJson = "{\"age\":30,\"custom_data\":\"custom\",\"gender\":\"male\",\"keywords\":\"keywords\"}"
        NimbusHelper.setUser(userJsonStr = userJson)
        assert(Nimbus.configuration.user?.age == 30)
        assert(Nimbus.configuration.user?.gender == User.Gender.Male)
    }

    @Test
    fun testSetBlockedAdvertisingDomains() {
        NimbusHelper.setBlockedAdvertisingDomains("www.reddit.com,https://www.google.com/search")
        assert(Nimbus.configuration.blockedAdvertisingDomains.contains("www.reddit.com"))
        assert(Nimbus.configuration.blockedAdvertisingDomains.contains("https://www.google.com/search"))
    }

    @Test
    fun testSetInterceptorTimeout() {
        NimbusHelper.setInterceptorTimeout(1000)
        assert(Nimbus.configuration.interceptorsTimeout == 1000.milliseconds)
    }

    @Test
    fun testSetShowMuteButton() {
        NimbusHelper.showMuteButton(false)
        assertFalse(Nimbus.configuration.showMuteButton)
    }
}