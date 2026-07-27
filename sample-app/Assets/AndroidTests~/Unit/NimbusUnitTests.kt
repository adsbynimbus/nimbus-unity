package com.adsbynimbus.unity

import org.junit.Test
import org.junit.Assert.*

/**
 * A simple local unit test that executes on the development machine (host).
 */
class NimbusUnitTests {
    
    @Test
    fun addition_isCorrect() {
        // A generic math check to ensure the JUnit runner is executing
        assertEquals(4, 2 + 2)
    }
    
    @Test
    fun stringFormatting_isCorrect() {
        val testString = "Hello Unity"
        assertTrue(testString.contains("Unity"))
    }
}