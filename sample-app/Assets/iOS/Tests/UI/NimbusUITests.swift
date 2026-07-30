import XCTest

class NimbusUITests: XCTestCase {

    override func setUpWithError() throws {
        // Stop running tests immediately if a failure happens
        continueAfterFailure = false
        
        // Launch the Unity application
        let app = XCUIApplication()
        app.launch()
    }

    func testGameLaunchAndButtonClick() throws {
        let app = XCUIApplication()
        
        // 1. Wait for Unity to fully boot up (e.g., look for an element or give it a 5-second buffer)
        let gameScreenLoaded = app.staticTexts["MainMenuTitle"].waitForExistence(timeout: 5.0)
        XCTAssertTrue(gameScreenLoaded, "The game main menu did not load in time.")

        // 2. Find a button by its accessibility identifier/text and tap it
        let startButton = app.buttons["StartButton"]
        XCTAssertTrue(startButton.exists, "The Start Button was not found on screen.")
        startButton.tap()
        
        // 3. Verify a change happened (e.g., checking if the score counter appears)
        let scoreLabel = app.staticTexts["Score: 0"]
        XCTAssertTrue(scoreLabel.exists, "The game did not transition to the gameplay screen.")
    }
}
