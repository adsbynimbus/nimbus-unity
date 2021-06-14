using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal
{
    internal class NimbusIOSAdManager : MonoBehaviour
    {
        private NimbusAdUnit _adUnit;

        private static NimbusIOSAdManager _instance;

        internal static NimbusIOSAdManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var obj = new GameObject("NimbusIOSAdManager");
                _instance = (NimbusIOSAdManager)obj.AddComponent(typeof(NimbusIOSAdManager));
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        internal void SetAdUnit(NimbusAdUnit adUnit)
        {
            _adUnit = adUnit;
        }

        #region iOS Event Callbacks

        // TODO missing response data, i need to the Nimbus response object back
        // private void onAdResponse(....) {
        //     
        // }
        
        internal void OnAdRendered(string param)
        {
            Debug.unityLogger.Log("OnAdRendered");
            _adUnit.AdWasRendered = true;
            _adUnit.EmitOnAdRendered(_adUnit);
        }

        internal void OnError(string param)
        {
            // TODO pass through error message like Android
            // e.g 	var errMessage = adError.Call<string>("getMessage");
            Debug.unityLogger.Log("OnError: " + param);
            _adUnit.EmitOnAdError(_adUnit);
        }

        internal void OnAdEvent(string param)
        {
            Debug.unityLogger.Log("OnAdEvent: " + param);
            var eventType = (AdEventTypes)System.Enum.Parse(typeof(AdEventTypes), param, true);
            _adUnit.EmitOnAdEvent(eventType);
        }
        #endregion
    }
}
