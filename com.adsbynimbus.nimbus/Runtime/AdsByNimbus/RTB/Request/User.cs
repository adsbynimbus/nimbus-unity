using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    public class user: RequestComponent
    {
        // Comma separated list of keywords, interests, or intent
        public string keywords;

        public user(string keywords)
        {
            this.keywords = keywords;
        }
    }
}