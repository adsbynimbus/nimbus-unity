namespace AdsByNimbus.RTB
{
    /// <summary>
    /// Adds viewability information to the RTB request.
    /// </summary>
    public struct viewability: RequestComponent
    {
        public string omidPn; //The viewability measurement partner identifier (for example, the vendor or SDK name).
        public string omidPv; //The viewability SDK version string associated with partner.

        public viewability(string omidPn, string omidPv)
        {
            this.omidPn = omidPn;
            this.omidPv = omidPv;
        }
    }

}