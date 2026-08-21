using System.Collections.Generic;
using System.Drawing;
using AdsByNimbus;
using AdsByNimbus.RTB;

public class InlineAd: Ad
{
    public AdSize AdSize;
    public Position AdPosition;
    public int RefreshInterval;
    public AdScreenPosition AdScreenPosition;
    public int XCoord, YCoord;
    public bool RespectSafeArea;
    internal bool DynamicUnit;
    public int DynamicUnitWidth;
    public int DynamicUnitHeight;

    internal InlineAd(in AdEvents adEvents, string position, AdSize adSize = AdSize.banner, Format[] addFormats = null,
        Position adPosition = Position.unknown, float bidFloor = 0f, int refreshInterval = 0, int dynamicUnitWidth = 0, int dynamicUnitHeight = 0,
        AdScreenPosition adScreenPosition = AdScreenPosition.BOTTOM_CENTER, int xCoord = -1, int yCoord = -1, 
        bool respectSafeArea = false, AdOrientation orientation = AdOrientation.deviceOrientation, bool dynamicUnit = false,
        List<RequestComponent> components = null, List<DemandComponent> demand = null): 
        base(AdType.Inline, adEvents, position, addFormats, bidFloor, orientation,  components, demand)
    {
        AdSize = adSize;
        if (DynamicUnit)
        {
            AddFormats ??= new[] { Format.mrec, Format.halfScreen };
        }
        AdPosition = adPosition;
        RespectSafeArea = respectSafeArea;
        RefreshInterval = refreshInterval;
        AdScreenPosition = adScreenPosition;
        XCoord = xCoord;
        YCoord = yCoord;
        DynamicUnitWidth = dynamicUnitWidth;
        DynamicUnitHeight = dynamicUnitHeight;
        DynamicUnit = dynamicUnit;
    }
}
public enum AdScreenPosition
{
    BOTTOM_CENTER = 0,
    TOP_CENTER = 1,
    CENTER = 2,
    BOTTOM_LEFT = 3,
    BOTTOM_RIGHT = 4,
    TOP_LEFT = 5,
    TOP_RIGHT = 6,
}

public enum AdSize : byte {
    // Standard banner format (320×50)
    banner,
    // Medium rectangle (MREC) format (300×250)
    mrec,
    // Half-screen format (300×600)
    halfScreen,
    // Leaderboard format (728×90)
    leaderboard,
    // Interstitial portrait format (320×480)
    interstitialPortrait,
    // Interstitial landscape format (480×320)
    interstitialLandscape,
}

public static class AdSizesExtension {
    public static Rectangle ToWidthAndHeight(this AdSize isa) {
        switch (isa) {
            case AdSize.banner:
                return new Rectangle(0, 0, 320, 50);
            case AdSize.mrec:
                return new Rectangle(0, 0, 300, 250);
            case AdSize.halfScreen:
                return new Rectangle(0, 0, 300, 600);
            case AdSize.leaderboard:
                return new Rectangle(0, 0, 728, 90);
            case AdSize.interstitialPortrait:
                return new Rectangle(0, 0, 320, 480);
            case AdSize.interstitialLandscape:
                return new Rectangle(0, 0, 480, 320);
            default:
                return new Rectangle(0, 0, 0, 0);
        }
    }
}