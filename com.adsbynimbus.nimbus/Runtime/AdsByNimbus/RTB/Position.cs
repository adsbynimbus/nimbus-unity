namespace AdsByNimbus.RTB
{
    /// <summary>
    ///     Describes the position of the ad as a relative measure of visibility or prominence.
    ///     This OpenRTB table has values derived from the Inventory Quality Guidelines (IQG). Values 4 - 7 apply to apps.
    ///     OpenRTB Section 5.4
    /// </summary>
    public enum Position: byte
    {
        unknown = 0,
        aboveTheFold = 1,
        belowTheFold = 2,
        header = 3,
        footer = 4,
        sidebar = 5,
        fullScreen = 6,
    }
}