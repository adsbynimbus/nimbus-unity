using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Nimbus.RTB;
using UnityEngine;

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
        
        #elif UNITY_ANDROID
        
        private const string HelperClass = "com.adsbynimbus.unity.NimbusHelper";
        private const string NimbusPackage = "com.adsbynimbus.Nimbus";
        private static AndroidJavaObject _helper;
        private static AndroidJavaObject Helper => _helper ??= new AndroidJavaObject(HelperClass).GetStatic<AndroidJavaObject>("INSTANCE");
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
            #elif UNITY_ANDROID
            Helper.CallStatic("setSessionId", sessionId);
            #endif
        }
        
        internal static void SetCoppa(bool coppa)
        {
            #if UNITY_IOS
            _setCoppa(coppa);
            #elif UNITY_ANDROID
            Helper.CallStatic("setCoppa", coppa);
            #endif
        }

        //universal app-wide
        internal static void SetApp(App app)
        {
            #if UNITY_IOS
            _setApp(JsonConvert.SerializeObject(app));
            #elif UNITY_ANDROID
            Helper.CallStatic("setApp", JsonConvert.SerializeObject(app));
            #endif
        }

        //universal app-wide
        internal static void SetUser(User user)
        {
            #if UNITY_IOS
            _setUser(JsonConvert.SerializeObject(user));
            #elif UNITY_ANDROID
            Helper.CallStatic("setUser", JsonConvert.SerializeObject(user));
            #endif
        }

        internal static void SetBlockedAdvertisingDomains(string[] blockedAdvertisingDomains)
        {
            #if UNITY_IOS
            _setBlockedAdvertisingDomains(String.Join(",", blockedAdvertisingDomains));
            #elif UNITY_ANDROID
            Helper.CallStatic("setBlockedAdvertisingDomains", String.Join(",", blockedAdvertisingDomains));
            #endif
        }

        internal static void SetRequestUrl(string requestUrl)
        {
            #if UNITY_IOS
            _setRequestUrl(requestUrl);
            #elif UNITY_ANDROID
            Helper.CallStatic("setRequestUrl", requestUrl);
            #endif
        }

        internal static void SetAdditionalRequestHeaders(Dictionary<string, string> additionalRequestHeaders)
        {
            #if UNITY_IOS
            _setAdditionalRequestHeaders(JsonConvert.SerializeObject(additionalRequestHeaders));
            #elif UNITY_ANDROID
            Helper.CallStatic("setAdditionalRequestHeaders", JsonConvert.SerializeObject(additionalRequestHeaders));
            #endif
        }

        internal static void SetInterceptorTimeout(int interceptorTimeout)
        {
            #if UNITY_IOS
            _setInterceptorTimeout(interceptorTimeout);
            #elif UNITY_ANDROID
            Helper.CallStatic("setInterceptorTimeout", interceptorTimeout);
            #endif
        }
        
        internal static void ShowMuteButton(bool showMuteButton)
        {
            #if UNITY_IOS
            _showMuteButton(showMuteButton);
            #elif UNITY_ANDROID
            Helper.CallStatic("showMuteButton", showMuteButton);
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
            #elif UNITY_ANDROID
            Helper.CallStatic("setGdprProperties", gdprApplies, consentString);
            #endif
        }

        internal static void SetGppProperties(string gppSectionId, string gppConsentString)
        {
            #if UNITY_IOS
            _setGppProperties(gppSectionId, gppConsentString);
            #elif UNITY_ANDROID
            Helper.CallStatic("setGppProperties", gppSectionId, gppConsentString);
            #endif
        }

        internal static void SetUsPrivacyString(string usPrivacyString)
        {
            #if UNITY_IOS
            _setUsPrivacyString(usPrivacyString);
            #elif UNITY_ANDROID
            Helper.CallStatic("setUsPrivacyString", usPrivacyString);
            #endif
        }

        internal static void SetVerificationProviders(VerificationProvider[] providers)
        {
            _verificationProviders = providers;
            _verificationMarkupDelegate = VerificationMarkupMethod;
            _verificationResourceDelegate = VerificationResourceMethod;

            #if UNITY_IOS
            var markupFunctionPointer = Marshal.GetFunctionPointerForDelegate(_verificationMarkupDelegate);
            var resourceFunctionPointer = Marshal.GetFunctionPointerForDelegate(_verificationResourceDelegate);
            _setVerificationCallbacks(markupFunctionPointer, resourceFunctionPointer, _verificationProviders.Length);
            #elif UNITY_ANDROID
            if (_helper == null)
            {
                var helperClass = new AndroidJavaObject(HelperClass);
                _helper = helperClass.GetStatic<AndroidJavaObject>("INSTANCE");
            }

            var javaArrayPtr = AndroidJNIHelper.ConvertToJNIArray(providers);
            try
            {
                using (AndroidJavaObject javaArray = new AndroidJavaObject(javaArrayPtr))
                {
                    Helper.CallStatic("setVerificationProviders", javaArray);
                }
            }
            finally
            {
                AndroidJNI.DeleteLocalRef(javaArrayPtr);
            }
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
                Marshal.StringToHGlobalAnsi(JsonConvert.SerializeObject(resourceValues));
            return pointer;
        }
    }
    
    /// <summary>
    ///     A verification provider for ad viewability tracking
    /// </summary>
    public class VerificationProvider: AndroidJavaProxy
    {
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponseMarkup">
        ///     Returns the bid response markup
        /// </param>
        /// <returns>
        ///     A string that provides markup to be injected into a static ad.
        /// </returns>
        public Func<string, string> VerificationMarkupCallback;
        
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponseMarkup">
        ///     Returns the bid response markup
        /// </param>
        /// <returns>
        ///     VerificationScriptResource that is passed to the OM SDK.
        /// </returns>
        public Func<string, VerificationScriptResource> VerificationResourceCallback;

        public VerificationProvider(Func<string, string> verificationMarkupCallback, Func<string, VerificationScriptResource> verificationResourceCallback):
            base("com.adsbynimbus.unity.VerificationProviderCallbackInterface")
        {
            VerificationMarkupCallback = verificationMarkupCallback;
            VerificationResourceCallback = verificationResourceCallback;
        }
        

        // Internal Nimbus method, do not use. Must be public for implementation.
        public string _verificationMarkupCallback(string response) 
        {
            return JsonConvert.SerializeObject(VerificationResourceCallback(response));
        }
        
        // Internal Nimbus method, do not use.  Must be public for implementation.
        public string _verificationResourceCallback(string response) 
        {
            return VerificationMarkupCallback(response);
        }

        public class VerificationScriptResource
        {
            public string url;
            public string vendorKey;
            public string parameters;

            public VerificationScriptResource(string url, string vendorKey, string parameters)
            {
                this.url = url;
                this.vendorKey = vendorKey;
                this.parameters = parameters;
            }
        }
    }
}