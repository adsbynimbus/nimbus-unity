// Target: YourAppTests
import Testing
import NimbusKit
import Foundation

struct NimbusUnitTests {
    
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

}
