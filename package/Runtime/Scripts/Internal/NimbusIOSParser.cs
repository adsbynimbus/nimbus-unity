using UnityEngine;

// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
    internal static class NimbusIOSParser
    {
        internal static T ParseMessage<T>(string jsonParams)
        {
            return JsonUtility.FromJson<T>(jsonParams);
        }
    }
    
    [System.Serializable]
    internal class NimbusIOSParams
    {
        public int adUnitInstanceID;
    }

    [System.Serializable]
    internal class NimbusIOSAdResponse : NimbusIOSParams
    {
        public string auctionId;
        public int bidRaw;
        public int bidInCents;
        public string network;
        public string placementId;
    }

    [System.Serializable]
    internal class NimbusIOSErrorData : NimbusIOSParams
    {
        public string errorMessage;
    }
    
    [System.Serializable]
    internal class NimbusIOSAdEventData : NimbusIOSParams
    {
        public string eventName;
    }
}