using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using JetBrains.Annotations;
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
        
        [DllImport("__Internal")]
        private static extern void _setVerificationCallbacks(IntPtr markupCallbackFunctionPtr,  
            IntPtr resourceCallbackFunctionPtr, int numCallbacks);
        
        #endif
        private delegate IntPtr VerificationMarkupMethodDelegate(string response, int index);
        private delegate IntPtr VerificationResourceMethodDelegate(string response, int index);
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
        internal static void SetApp(RTBApp app)
        {
            #if UNITY_IOS
                _setApp(JsonConvert.SerializeObject(app));
            #endif            
        }

        //universal app-wide
        internal static void SetUser(RTBUser user)
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
            #if UNITY_IOS
                _setVerificationCallbacks(markupFunctionPointer, resourceFunctionPointer, _verificationProviders.Length);
            #endif
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
    
    /// <summary>
    ///     A verification provider for ad viewability tracking
    /// </summary>
    public class VerificationProvider
    {
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponse">
        ///     Returns the bid response in a json string format
        /// </param>
        /// <returns>
        ///     A string that provides markup to be injected into a static ad.
        /// </returns>
        public Func<string, string> VerificationMarkupCallback;
        
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponse">
        ///     Returns the bid response in a json string format
        /// </param>
        /// <returns>
        ///     A tuple of 3 strings (url, vendorKey, parameters) which are used to create a VerificationScriptResource
        ///     that is passed to the OM SDK.
        /// </returns>
        public Func<string, Tuple<string, string, string>> VerificationResourceCallback;

        public VerificationProvider(Func<string, string> verificationMarkupCallback, Func<string, Tuple<string, string, string>> verificationResourceCallback)
        {
            VerificationMarkupCallback = verificationMarkupCallback;
            VerificationResourceCallback = verificationResourceCallback;
        }
    }
    
    /// <summary>
    /// This object contains information known or derived about the human user of the device (i.e., the audience for advertising).
    /// The user id is an exchange artifact and may be subject to rotation or other privacy policies.
    /// However, this user ID must be stable long enough to serve reasonably as the basis for frequency capping and retargeting.
    /// OpenRTB Section 3.2.20
    /// </summary>
    public class RTBUser
    {
        // The age of the user
        public int? age; 
        // Buyer-specific ID for the user as mapped by the exchange for the buyer. Set to Facebook bidder token if integrating Facebook demand
        [CanBeNull] public string buyeruid;
        /*
         * Optional feature to pass bidder data that was set in the exchange’s cookie.
         * The string must be in base85 cookie safe characters and be in any format. Proper JSON encoding must be used to include “escaped” quotation marks
         */
        [CanBeNull] public string customData; 
        // The gender of the user
        public Gender? gender;
        // Comma separated list of keywords, interests, or intent
        [CanBeNull] public string keywords;

        public RTBUser(int? age = null, [CanBeNull] string buyeruid = null, [CanBeNull] string customData = null, 
            Gender? gender = null, [CanBeNull] string keywords = null)
        {
            this.age = age;
            this.buyeruid = buyeruid;
            this.customData = customData;
            this.gender = gender;
            this.keywords = keywords;
        }
    }

    public enum Gender: byte
    {
        male = 0,
        female = 1,
        other = 2
    }

    /// <summary>
    /// This object should be included if the ad supported content is a non-browser application (typically in mobile) as opposed to a website.
    /// OpenRTB Section 3.2.14
    /// </summary>
    public class RTBApp
    {
        [CanBeNull] public string bundle; // A platform-specific application identifier intended to be unique to the app and independent of the exchange. On iOS, it is typically a numeric ID. Default: nil
        public string[] cat; // IAB content categories of the app OpenRTB Section 5.1
        [CanBeNull] public string domain; // Domain of the app (e.g., “adsbynimbus.com”). Default: nil
        [CanBeNull] public string name; // App name (may be aliased at the publisher’s request). Default: nil
        public string[] pagecat; // IAB content categories that describe the current page or view of the app. OpenRTB Section 5.1
        public bool? paid; // Whether the app is paid or not
        public bool? privacypolicy; // Indicates if the app has a privacy policy
        [CanBeNull] public RTBPublisher publisher; // Details about the publisher of the app
        public string[] sectioncat; // IAB content categories that describe the current section of the app. OpenRTB Section 5.1
        [CanBeNull] public string storeurl; // App store URL for an installed app; for IQG 2.1 compliance. Default: nil
        [CanBeNull] public string ver; // Application version

        public RTBApp([CanBeNull] string bundle = null, string[] cat = null, [CanBeNull] string domain = null, 
            [CanBeNull] string name = null, string[] pagecat = null, bool? paid = default, bool? privacypolicy = default, 
            [CanBeNull] RTBPublisher publisher = null, string[] sectioncat = null, [CanBeNull] string storeurl = null, [CanBeNull] string ver = null)
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
    public class RTBPublisher
    {
        public string[] cat; // IAB content categories that describe the publisher. OpenRTB Section 5.1 Default: nil
        public string domain; // Highest level domain of the publisher (e.g., “adsbynimbus.com”). Default: nil
        public string name; // Publisher name (may be aliased at the publisher’s request). Default: nil

        public RTBPublisher(string[] cat, string domain, string name)
        {
            this.cat = cat;
            this.domain = domain;
            this.name = name;
        }
    }
}