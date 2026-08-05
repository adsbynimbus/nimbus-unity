namespace AdsByNimbus.Internal.Utility
{
    /// <summary>
    ///     Modifiers Added to an Ad on a per-request basis
    /// </summary>
    internal struct RequestModifiers
    {
        // Adds per-request app categories to the RTB request.
        public app? app;
        // A banner creative to be attached to the ad request.
        public banner? banner;
        // Overrides the environment for a single ad.
        public Environment? environment;
        // Adds device geolocation to the RTB request.
        public Location? location;
        // Adds per-request user keywords to the RTB request.
        //A comma-separated keyword string to assign to the RTB User object. 
        [CanBeNull] public String userKeywords;
        // Attaches a video creative to the ad request.
        public video? video;
        // Adds viewability information to the RTB request.
        public Viewability? viewability;


        public RequestModifiers(app? app = null, banner? banner = null, 
            Environment? environment = null, Location? location = null, [CanBeNull] string userKeywords = null, 
            video? video = null, Viewability? viewability = null)
        {
            this.app = app;
            this.banner = banner;
            this.environment = environment;
            this.location = location;
            this.userKeywords = userKeywords;
            this.video = video;
            this.viewability = viewability;
        }
    }
}