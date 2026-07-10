// Target: YourAppTests
import Testing
import NimbusKit

struct NimbusUnitTests {
    
    @Test("Validates that setCoppa works correctly")
    func testSetCoppa() {
        NimbusHelper.setCoppa(coppa: true)
        #expect(Nimbus.configuration.coppa)
    }
}
