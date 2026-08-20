using AdsByNimbus;

namespace AdsByNimbus.RTB.Request
{
    
    /// <summary>
    ///     Adds Content URL to App object for requests
    /// </summary>
    /// <param name="url">
    ///     Publicly accessible web url (needs to include scheme (i.e. https://))
    /// </param>
    public class content: RequestComponent
    {
        public string url; 

        public content(string url)
        {
            this.url = url;
        }
    }
}