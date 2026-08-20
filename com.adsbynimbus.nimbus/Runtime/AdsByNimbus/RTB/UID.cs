using System.Collections.Generic;

namespace AdsByNimbus.RTB
{
    /// <summary>
    ///     OpenRTB UID Object
    /// </summary>
    public class UID
    {
        public string id;
        public int? atype;
        public Dictionary<string, string> ext;

        public UID(string id, int? atype = null, Dictionary<string, string> ext = null)
        {
            this.id = id;
            this.atype = atype;
            if (ext == null)
            {
                this.ext = new Dictionary<string, string>();
            }
            else
            {
                this.ext = ext;
            }
        }
    }
}