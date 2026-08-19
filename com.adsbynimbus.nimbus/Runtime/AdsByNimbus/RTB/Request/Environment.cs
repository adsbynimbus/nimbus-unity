namespace AdsByNimbus.RTB
{
    /// <summary>
    ///     Overrides the environment for a single ad.
    /// </summary>
    public struct environment: RequestComponent
    {
        public string publisherKey; // Publisher key to be used for this ad
        public string apiKey; // API Key to be used for this ad

        public environment(string publisherKey, string apiKey)
        {
            this.publisherKey = publisherKey;
            this.apiKey = apiKey;
        }
    }
}