using System.Collections.Generic;

namespace Nimbus.Internal.Extensions
{
    public class ConfigHelpers
    {
        public static void setSessionId(string sessionId)
        {
            
        }
        
        public static void setCoppa(bool coppa)
        {
            
        }

        //universal app-wide
        public static void setApp(App app)
        {
            
        }

        //universal app-wide
        public static void setUser(User user)
        {
            
        }

        public static void setBlockedAdvertisingDomains(string[] blockedAdvertisingDomains)
        {
            
        }

        public static void setRequestUrl(string requestUrl)
        {
            
        }

        public static void setAdditionalRequestHeaders(Dictionary<string, string> additionalRequestHeaders)
        {
            
        }

        public static void setInterceptorTimeout(int interceptorTimeout)
        {
            
        }
        
        public static void showMuteButton(bool showMuteButton)
        {
        }
        
        //very difficult, maybe say the can add it in swift/kotlin if needed?
        public static void setVerificationProviders()
        {
            
        }

        public static void enableSwipeProtection(bool enableSwipeProtection)
        {
            
        }
        
        //very difficult, maybe say the can add it in swift/kotlin if needed?
        public static void setIdentityProvider()
        {
            
        }

        public static void setIsSKOverlayEnabledForAllUnits(bool isSKOverlayEnabled)
        {
            
        }

        public static void setIABProperties(IABProperties properties)
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