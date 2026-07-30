// Target: YourAppTests
import Testing
import NimbusKit
import Foundation

struct NimbusUnitTests {
    
    // MARK: - Helper method overall config tests

    @Test("Validates that setCoppa works correctly")
    func testSetCoppa() {
        NimbusHelper.setCoppa(coppa: true)
        #expect(Nimbus.configuration.coppa)
    }
    
    @Test("Validates that setApp works correctly")
    func testSetApp() {
        let appJson =    "{\"bundle\":\"com.test.nimbusUnity\",\"cat\":[],\"domain\":\"nimbus.co\",\"name\":\"testapp\",\"pagecat\":[],\"publisher\":{\"cat\":[],\"domain\":\"nimbus.co\",\"name\":\"nimbus\"},\"sectioncat\":[],\"storeurl\":\"www.nimbus.co\",\"ver\":\"3.0.0\",\"paid\":1,\"privacypolicy\":1}"
        NimbusHelper.setApp(appJsonStr: appJson)
        #expect(Nimbus.configuration.app?.bundle == "com.test.nimbusUnity")
        #expect(Nimbus.configuration.app?.paid == true)
        #expect(Nimbus.configuration.app?.publisher?.domain == URL(string: "nimbus.co"))
    }
    
    @Test("Validates that setUser works correctly")
    func testSetUser() {
        let userJson = "{\"age\":30,\"customData\":\"custom\",\"gender\":\"male\",\"keywords\":\"keywords\"}"
        NimbusHelper.setUser(userJsonStr: userJson)
        #expect(Nimbus.configuration.user?.age == 30)
        #expect(Nimbus.configuration.user?.gender == .male)
    }
    
    @Test("Validates that setBlockedAdvertisingDomains Works Correctly")
    func testSetBlockedAdvertisingDomains() {
        NimbusHelper.setBlockedAdvertisingDomains(domains: "www.reddit.com,https://www.google.com/search")
        if let testDomain = URL(string: "www.reddit.com") {
            #expect(Nimbus.configuration.blockedAdvertisingDomains.contains(testDomain))
        }
        if let testDomain = URL(string: "https://www.google.com/search") {
            #expect(Nimbus.configuration.blockedAdvertisingDomains.contains(testDomain))
        }
    }
    
    @Test("Validates that setting interceptor timeout works correctly")
    func testSetInterceptorTimeout() {
        NimbusHelper.setInterceptorTimeout(timeout: 1000)
        #expect(Nimbus.configuration.interceptorsTimeout == 1000)
    }
    
    @Test("Validates that show mute button setting works correctly")
    func testSetShowMuteButton() {
        NimbusHelper.showMuteButton(show: false)
        #expect(!Nimbus.configuration.showMuteButton)
    }
    
    @Test("Validates that enable swipe protection setting works correctly")
    func testSetEnableSwipeProtection() {
        NimbusHelper.enableSwipeProtection(enable: true)
        #expect(Nimbus.configuration.enableSwipeProtection)
    }
    
    @Test("Validates that enable sk overlay for all units setting works correctly")
    func testSetIsSKOverlayEnabledForAllUnits() {
        NimbusHelper.setIsSKOverlayEnabledForAllUnits(isEnabled: true)
        #expect(Nimbus.configuration.isSKOverlayEnabledForAllUnits)
    }
    
    // MARK: - Unity JSON Tests
    
    @Test("Testing Init Extensions JSON Encoding")
    func testExtensionsJSON() {
        let extensionJson = "{\"aps\":{\"appKey\":\"a10_app_id\",\"slotData\":null},\"adMob\":{\"adUnitIds\":null},\"inMobi\":{\"accountId\":\"fd12345678\"},\"meta\":{\"appId\":\"19246789\",\"forceTestAd\":true},\"mintegral\":{\"appId\":\"12468\",\"appKey\":\"7c238655695865991s\"},\"mobileFuse\":{},\"moloco\":{\"appKey\":\"VSDFKNSDKFNSD:SDKFNDSF345\"},\"unityAds\":{\"gameId\":\"4231234\"},\"vungle\":{\"appId\":\"65eh49test1234\"}}"
        let extensions = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: extensionJson)
        #expect(extensions?.aps?.appKey == "a10_app_id")
        #expect(extensions?.inMobi?.accountId == "fd12345678")
        #expect(extensions?.meta?.appId == "19246789")
    }
    
    @Test("Testing Request Third Party Demand JSON")
    func testRequestThirdPartyDemandJSON() {
        let demandJson = "{\"aps\":{\"appKey\":null,\"slotData\":[{\"slotId\":\"883etest-0bf1-83fc-947c-925babbf3f\",\"adUnitType\":0}]},\"adMob\":{\"adUnitIds\":[\"ca-app-pub-332345679942544/111111107\"]},\"inMobi\":{\"accountId\":null},\"meta\":{\"appId\":null,\"forceTestAd\":false},\"mintegral\":{\"appId\":null,\"appKey\":null},\"mobileFuse\":{},\"moloco\":{\"appKey\":null},\"unityAds\":{\"gameId\":null},\"vungle\":{\"appId\":null}}"
        let thirdPartyDemand = NimbusHelper.extensionsFromJsonString(thirdPartyDemand: demandJson)
        #expect(thirdPartyDemand?.aps?.slotData?[0]?.slotId == "883etest-0bf1-83fc-947c-925babbf3f")
        #expect(thirdPartyDemand?.adMob?.adUnitIds?[1] == "ca-app-pub-332345679942544/111111107")
    }
    
    @Test("Testing Request Modifiers JSON")
    func testRequestModifiersJSON() {
        let requestModifiersJson = "{\"app\":{\"pageCat\":[\"pagecat1\",\"pagecat2\"],\"sectionCat\":[\"sectioncat1\",\"sectioncat2\"]},\"banner\":{\"width\":320,\"height\":50,\"addFormats\":[1,4,2],\"adPosition\":4,\"bidFloor\":0.0,\"battr\":[8]},\"environment\":null,\"location\":{\"latitude\":0.0,\"longitude\":0.0,\"locationType\":1,\"accuracy\":20},\"userKeywords\":\"smart,gaming\",\"video\":{\"adPosition\":6,\"bidFloor\":0.0,\"minDuration\":0,\"maxDuration\":30,\"width\":0,\"height\":0,\"placementType\":3,\"playbackMethod\":[3,4]},\"viewability\":{\"omidPn\":\"omid1\",\"omidPv\":\"omid2\"}}"
        let requestModifiers = NimbusHelper.requestModifiersFromJsonString(requestModifiers: requestModifiersJson)
        #expect(requestModifiers?.userKeywords == "smart,gaming")
        #expect(requestModifiers?.video?.maxDuration == 30)
        #expect(requestModifiers?.banner?.width == 320)
    }
}
