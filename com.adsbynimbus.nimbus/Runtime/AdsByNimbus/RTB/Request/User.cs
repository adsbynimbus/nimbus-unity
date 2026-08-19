using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    public class user: RequestComponent
    {
        // Comma separated list of keywords, interests, or intent
        [CanBeNull] public string keywords;

        public user([CanBeNull] string keywords)
        {
            this.keywords = keywords;
        }
    }
}