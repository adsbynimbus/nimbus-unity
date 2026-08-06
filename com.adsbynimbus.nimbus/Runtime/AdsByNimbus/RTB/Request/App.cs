using System;

namespace AdsByNimbus.RTB.Request
{
    /// <summary>
    ///     Adds per-request app categories to the RTB request.
    /// </summary>
    public class app: RequestComponent
    {
        public string[] pageCat; // The RTB pagecat value to apply for this request. This set is written to the RTB App object as page categories.
        public string[] sectionCat; // The RTB sectioncat value to apply for this request. This set is written to the RTB App object as section categories.

        public app(string[] pageCat, string[] sectionCat)
        {
            this.pageCat = pageCat;
            this.sectionCat = sectionCat;
        }
    }
}