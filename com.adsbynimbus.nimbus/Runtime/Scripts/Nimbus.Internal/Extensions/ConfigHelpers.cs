using System.Collections.Generic;

namespace Nimbus.Internal.Extensions
{
    internal class ConfigHelpers
    {
        internal static void SetSessionId(string sessionId)
        {
            
        }
        
        internal static void SetCoppa(bool coppa)
        {
            
        }

        //universal app-wide
        internal static void SetApp(App app)
        {
            
        }

        //universal app-wide
        internal static void SetUser(User user)
        {
            
        }

        internal static void SetBlockedAdvertisingDomains(string[] blockedAdvertisingDomains)
        {
            
        }

        internal static void SetRequestUrl(string requestUrl)
        {
            
        }

        internal static void SetAdditionalRequestHeaders(Dictionary<string, string> additionalRequestHeaders)
        {
            
        }

        internal static void SetInterceptorTimeout(int interceptorTimeout)
        {
            
        }
        
        internal static void ShowMuteButton(bool showMuteButton)
        {
        }
        
        //very difficult, maybe say the can add it in swift/kotlin if needed?
        internal static void SetVerificationProviders()
        {
            
        }

        internal static void EnableSwipeProtection(bool enableSwipeProtection)
        {
            
        }
        
        //very difficult, maybe say the can add it in swift/kotlin if needed?
        internal static void SetIdentityProvider()
        {
            
        }

        internal static void SetIsSKOverlayEnabledForAllUnits(bool isSKOverlayEnabled)
        {
            
        }

        internal static void SetIABProperties(IABProperties properties)
        {
            
        }
    }
    //TODO: User, app, iab objects
    public class User
    {
        public int age;
        public string buyerUid;
        public string customData;
        public Gender gender;
        public string keywords;
    }

    public enum Gender
    {
        female = 0,
        male = 1,
        other = 2
    }

    public class App
    {
        public string bundle;
        public string[] cat;
        public string domain;
        public string name;
        public string[] pageCat;
        public bool paid;
        public bool privacyPolicy;
        public Publisher publisher;
        public string[] sectionCat;
        public string storeUrl;
        public string ver;
    }

    public class Publisher
    {
        public string[] cat;
        public string domain;
        public string name;
    }

    public class IABProperties
    {
        public bool gdprApplies;
        public string gppConsentString;
        public string gppSectionId;
        public string tcfString;
        public string usPrivacyString;
    }
}