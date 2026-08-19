using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AdsByNimbus.RTB
{
    /// <summary>
    /// This object should be included if the ad supported content is a non-browser application (typically in mobile) as opposed to a website.
    /// OpenRTB Section 3.2.14
    /// </summary>
    public class App
    {
        [CanBeNull] public string bundle; // A platform-specific application identifier intended to be unique to the app and independent of the exchange. On iOS, it is typically a numeric ID. Default: nil
        public string[] cat; // IAB content categories of the app OpenRTB Section 5.1
        [CanBeNull] public string domain; // Domain of the app (e.g., “adsbynimbus.com”). Default: nil
        [CanBeNull] public string name; // App name (may be aliased at the publisher’s request). Default: nil
        public string[] pagecat; // IAB content categories that describe the current page or view of the app. OpenRTB Section 5.1
        [JsonIgnore]
        public bool? paid; // Whether the app is paid or not
        [JsonProperty("paid")]
        internal byte? paidJson => paid == null ? null : paid.Value ? (byte)1 : (byte)0;
        [JsonIgnore]
        public bool? privacypolicy; // Indicates if the app has a privacy policy
        [JsonProperty("privacypolicy")]
        internal byte? privacyPolicyJson => privacypolicy == null ? null : privacypolicy.Value ? (byte)1 : (byte)0;
        [CanBeNull] public Publisher publisher; // Details about the publisher of the app
        public string[] sectioncat; // IAB content categories that describe the current section of the app. OpenRTB Section 5.1
        [CanBeNull] public string storeurl; // App store URL for an installed app; for IQG 2.1 compliance. Default: nil
        [CanBeNull] public string ver; // Application version

        public App([CanBeNull] string bundle = null, string[] cat = null, [CanBeNull] string domain = null, 
            [CanBeNull] string name = null, string[] pagecat = null, bool? paid = default, bool? privacypolicy = default, 
            [CanBeNull] Publisher publisher = null, string[] sectioncat = null, [CanBeNull] string storeurl = null, [CanBeNull] string ver = null)
        {
            this.bundle = bundle;
            this.cat = cat;
            this.domain = domain;
            this.name = name;
            this.pagecat = pagecat;
            this.paid = paid;
            this.privacypolicy = privacypolicy;
            this.publisher = publisher;
            this.sectioncat = sectioncat;
            this.storeurl = storeurl;
            this.ver = ver;
        }
    }
    
    /// <summary>
    /// This describes the publisher of the media in which the ad will be displayed. The publisher is typically the seller in an OpenRTB transaction.
    /// OpenRTB Section 3.2.15
    /// </summary>
    public class Publisher
    {
        public string[] cat; // IAB content categories that describe the publisher. OpenRTB Section 5.1 Default: nil
        public string domain; // Highest level domain of the publisher (e.g., “adsbynimbus.com”). Default: nil
        public string name; // Publisher name (may be aliased at the publisher’s request). Default: nil

        public Publisher(string[] cat, string domain, string name)
        {
            this.cat = cat;
            this.domain = domain;
            this.name = name;
        }
    }
}