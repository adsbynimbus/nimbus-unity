namespace AdsByNimbus.RTB
{
    /// <summary>
    ///     Supported ad format with a width and height
    /// </summary>
    public enum Format: byte
    {
        banner = 1, //Standard banner format (320×50).
        mrec = 2, // Medium rectangle (MREC) format (300×250).
        halfScreen = 3, // Half-screen format (300×600).
        leaderboard = 4, // Leaderboard format (728×90).
        interstitialPortrait = 5, // Interstitial portrait format (320×480).
        interstitialLandscape = 6, // Interstitial landscape format (480×320).
        interstitial = 7, // Interstitial format chosen for the current device orientation.
    }
}