using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Newtonsoft.Json;

namespace Nimbus.Internal.Extensions
{
    public class ConfigHelpers
    {
        #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void _setSessionId(string sessionId);
        
        [DllImport("__Internal")]
        private static extern void _setCoppa(bool coppa);
        
        [DllImport("__Internal")]
        private static extern void _setApp(string appJsonStr);
        
        [DllImport("__Internal")]
        private static extern void _setUser(string userJsonStr);
        
        [DllImport("__Internal")]
        private static extern void _setBlockedAdvertisingDomains(string domains);
        
        [DllImport("__Internal")]
        private static extern void _setRequestUrl(string url);
        
        [DllImport("__Internal")]
        private static extern void _setAdditionalRequestHeaders(string headers);
        
        [DllImport("__Internal")]
        private static extern void _setInterceptorTimeout(int timeout);
        
        [DllImport("__Internal")]
        private static extern void _showMuteButton(bool show);
        
        [DllImport("__Internal")]
        private static extern void _enableSwipeProtection(bool enableSwipeProtection);
        
        [DllImport("__Internal")]
        private static extern void _setIsSkOverlayEnabledForAllUnits(bool isEnabled);
        
        [DllImport("__Internal")]
        private static extern void _setGdprProperties(bool gdprApplies, string gdprConsentString);
        
        [DllImport("__Internal")]
        private static extern void _setGppProperties(string gppSectionId, string gppConsentString);
        
        [DllImport("__Internal")]
        private static extern void _setUsPrivacyString(string usPrivacyString);
        
        private delegate IntPtr VerificationMarkupMethodDelegate(string response, int index);
        private delegate IntPtr VerificationResourceMethodDelegate(string response, int index);
        [DllImport("__Internal")]
        private static extern void _setVerificationCallbacks(IntPtr markupCallbackFunctionPtr,  
            IntPtr resourceCallbackFunctionPtr, int numCallbacks);
        
        #endif
        
        private static VerificationMarkupMethodDelegate _verificationMarkupDelegate;
        private static VerificationResourceMethodDelegate _verificationResourceDelegate;
        private static VerificationProvider[] _verificationProviders;
        
        internal static void SetSessionId(string sessionId)
        {
            #if UNITY_IOS
                _setSessionId(sessionId);
            #endif
        }
        
        internal static void SetCoppa(bool coppa)
        {
            #if UNITY_IOS
                _setCoppa(coppa);
            #endif
        }

        //universal app-wide
        internal static void SetApp(App app)
        {
            #if UNITY_IOS
                _setApp(JsonConvert.SerializeObject(app));
            #endif            
        }

        //universal app-wide
        internal static void SetUser(User user)
        {
            #if UNITY_IOS
                _setUser(JsonConvert.SerializeObject(user));
            #endif                
        }

        internal static void SetBlockedAdvertisingDomains(string[] blockedAdvertisingDomains)
        {
            #if UNITY_IOS
                _setBlockedAdvertisingDomains(String.Join(",", blockedAdvertisingDomains));
            #endif  
        }

        internal static void SetRequestUrl(string requestUrl)
        {
            #if UNITY_IOS
                _setRequestUrl(requestUrl);
            #endif  
        }

        internal static void SetAdditionalRequestHeaders(Dictionary<string, string> additionalRequestHeaders)
        {
            #if UNITY_IOS
                _setAdditionalRequestHeaders(JsonConvert.SerializeObject(additionalRequestHeaders));
            #endif
        }

        internal static void SetInterceptorTimeout(int interceptorTimeout)
        {
            #if UNITY_IOS
                _setInterceptorTimeout(interceptorTimeout);
            #endif
        }
        
        internal static void ShowMuteButton(bool showMuteButton)
        {
            #if UNITY_IOS
                _showMuteButton(showMuteButton);
            #endif
        }

        internal static void EnableSwipeProtection(bool enableSwipeProtection)
        {
            #if UNITY_IOS
                _enableSwipeProtection(enableSwipeProtection);
            #endif
        }
        
        internal static void SetIsSkOverlayEnabledForAllUnits(bool isSkOverlayEnabledForAllUnits)
        {
            #if UNITY_IOS
                _setIsSkOverlayEnabledForAllUnits(isSkOverlayEnabledForAllUnits);
            #endif
        }

        internal static void SetGdprProperties(bool gdprApplies, string consentString)
        {
            #if UNITY_IOS
                _setGdprProperties(gdprApplies, consentString);
            #endif
        }

        internal static void SetGppProperties(string gppSectionId, string gppConsentString)
        {
            #if UNITY_IOS
                _setGppProperties(gppSectionId, gppConsentString);
            #endif
        }

        internal static void SetUsPrivacyString(string usPrivacyString)
        {
            #if UNITY_IOS
                _setUsPrivacyString(usPrivacyString);
            #endif
        }

        internal static void SetVerificationProviders(VerificationProvider[] providers)
        {	
            _verificationProviders = providers;
            _verificationMarkupDelegate = VerificationMarkupMethod;
            _verificationResourceDelegate = VerificationResourceMethod;
            var markupFunctionPointer = Marshal.GetFunctionPointerForDelegate(_verificationMarkupDelegate);
            var resourceFunctionPointer = Marshal.GetFunctionPointerForDelegate(_verificationResourceDelegate);
            _setVerificationCallbacks(markupFunctionPointer, resourceFunctionPointer, _verificationProviders.Length);
        }
        
        		
        [MonoPInvokeCallback(typeof(VerificationMarkupMethodDelegate))]
        private static IntPtr VerificationMarkupMethod(string response, int index)
        {
            var pointer = Marshal.StringToHGlobalAnsi(_verificationProviders[index].VerificationMarkupCallback(response));
            return pointer;
        }
		
        [MonoPInvokeCallback(typeof(VerificationResourceMethodDelegate))]
        private static IntPtr VerificationResourceMethod(string response,int index)
        {
            var resourceValues = _verificationProviders[index].VerificationResourceCallback(response);
            var pointer =
                Marshal.StringToHGlobalAnsi($"{resourceValues.Item1},{resourceValues.Item2},{resourceValues.Item3}");
            return pointer;
        }
    }
    public class VerificationProvider
    {
        public Func<string, string> VerificationMarkupCallback;
        public Func<string, Tuple<string, string, string>> VerificationResourceCallback;

        public VerificationProvider(Func<string, string> verificationMarkupCallback, Func<string, Tuple<string, string, string>> verificationResourceCallback)
        {
            VerificationMarkupCallback = verificationMarkupCallback;
            VerificationResourceCallback = verificationResourceCallback;
        }
    }
    public class User
    {
        public int age;
        public string buyeruid;
        public string customData;
        public Gender gender;
        public string keywords;

        public User(int age, string buyeruid, string customData, Gender gender, string keywords)
        {
            this.age = age;
            this.buyeruid = buyeruid;
            this.customData = customData;
            this.gender = gender;
            this.keywords = keywords;
        }
    }

    public enum Gender
    {
        male = 0,
        female = 1,
        other = 2
    }

    public class App
    {
        public string bundle;
        public string[] cat;
        public string domain;
        public string name;
        public string[] pagecat;
        public bool paid;
        public bool privacypolicy;
        public Publisher publisher;
        public string[] sectioncat;
        public string storeurl;
        public string ver;

        public App(string bundle, string[] cat, string domain, string name, string[] pagecat, bool paid, bool privacypolicy, Publisher publisher, string[] sectioncat, string storeurl, string ver)
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

    public class Publisher
    {
        public string[] cat;
        public string domain;
        public string name;

        public Publisher(string[] cat, string domain, string name)
        {
            this.cat = cat;
            this.domain = domain;
            this.name = name;
        }
    }
}