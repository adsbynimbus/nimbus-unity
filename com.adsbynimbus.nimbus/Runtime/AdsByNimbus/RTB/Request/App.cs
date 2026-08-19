using System;
using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    /// <summary>
    ///     Adds per-request app categories to the RTB request.
    /// </summary>
    public class app: RequestComponent
    {
        public string[] pageCat; // The RTB pagecat value to apply for this request. This set is written to the RTB App object as page categories.
        public string[] sectionCat; // The RTB sectioncat value to apply for this request. This set is written to the RTB App object as section categories.
        [CanBeNull] public string contentUrl; // Content URL for requests

        public app(string[] pageCat, string[] sectionCat, [CanBeNull] string contentUrl = null)
        {
            this.pageCat = pageCat;
            this.sectionCat = sectionCat;
            this.contentUrl = contentUrl;
        }
        
        /// <summary>
        ///     Adds Content URL to App object for requests
        /// </summary>
        /// <param name="url">
        ///     Publicly accessible web url (needs to include scheme (i.e. https://))
        /// </param>
        public void setContentUrl(string url)
        {
            contentUrl = url;
        }
        
        public app()
        {
            
        }
    }
}