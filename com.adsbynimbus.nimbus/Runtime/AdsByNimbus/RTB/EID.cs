namespace AdsByNimbus.RTB
{
    
    /// <summary>
    ///     OpenRTB EID Object
    /// </summary>
    public class EID
    {
        public string source;
        public UID[] uids;
        
        public EID(string source, UID[] ids)
        {
            this.source = source;
            uids = ids;
        }
    }
}