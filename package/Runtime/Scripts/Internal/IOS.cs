using System;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Nimbus.Runtime.Scripts.Internal {
    public class IOS : NimbusAPI {

#region Declare external C interface    
#if UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void _initializeSDKWithPublisher(string publisher, string apiKey);

#endif
#endregion

#region Wrapped methods and properties

        internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
#if UNITY_IOS && !UNITY_EDITOR
            _initializeSDKWithPublisher("name", "key");
#endif
        }

        internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
            throw new NotImplementedException();
        }

        internal override void SetGDPRConsentString(string consent)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}