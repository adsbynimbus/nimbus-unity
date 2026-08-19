namespace AdsByNimbus.RTB
{
    /// <summary>
    ///     Adds device geolocation to the RTB request.
    /// </summary>
    public struct location: RequestComponent
    {
        public double latitude; // The latitude in decimal degrees. Valid range is -90...90.
        public double longitude; // The longitude in decimal degrees. Valid range is -180...180.
        public LocationType locationType; // Source of location data, e.g. GPS
        public int? accuracy; // The estimated horizontal accuracy radius in meters. Pass nil to omit. Values less than or equal to 0 are treated as unknown and omitted.

        public location(double latitude, double longitude, LocationType locationType, int? accuracy = null)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;
            this.accuracy = accuracy;
        }
    }

    public enum LocationType : byte {
        gps = 1,
        ipLookup = 2,
        userProvided = 3
    }
}