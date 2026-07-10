import androidx.test.ext.junit.runners.AndroidJUnit4
import androidx.test.platform.app.InstrumentationRegistry
import androidx.test.uiautomator.UiDevice
import androidx.test.uiautomator.UiSelector
import androidx.test.uiautomator.By
import androidx.test.uiautomator.Until
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith

@RunWith(AndroidJUnit4::class)
class MyFirstUITest {

    private lateinit var device: UiDevice
    private val timeout = 5000L

    @Before
    fun setUp() {
        // Initialize UI Automator instance
        device = UiDevice.getInstance(InstrumentationRegistry.getInstrumentation())
    }

    @Test
    fun testGameLaunchAndInteraction() {
        // 1. Verify the device is on the home screen or the app successfully launched
        assertNotNull(device)

        // 2. Wait for an object with specific text to appear on the Unity screen
        val gameTitleExists = device.wait(Until.hasObject(By.text("MainMenuTitle")), timeout)
        assertTrue("Game did not load the main menu title in time", gameTitleExists)

        // 3. Find a button by its on-screen text and tap it
        val startButton = device.findObject(UiSelector().text("Start"))
        assertTrue("Start button could not be found on screen", startButton.exists())
        startButton.click()

        // 4. Verify that the UI changed after the click
        val gameplayTextExists = device.wait(Until.hasObject(By.text("Score: 0")), timeout)
        assertTrue("Game did not transition to the score screen", gameplayTextExists)
    }
}
