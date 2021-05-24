using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal
{
    public class IOSAdManager : MonoBehaviour
    {
        private NimbusAdUnit adUnit;

        private static IOSAdManager instance;

        public static IOSAdManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject("IOSAdManager");
                    instance = (IOSAdManager)obj.AddComponent(typeof(IOSAdManager));
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void SetAdUnit(NimbusAdUnit adUnit)
        {
            this.adUnit = adUnit;
        }

        #region iOS Event Callbacks

        public void OnAdRendered(string param)
        {
            Debug.unityLogger.Log("OnAdRendered");
            adUnit.AdWasRendered = true;
            adUnit.EmitOnAdRendered(adUnit);
        }

        public void OnError(string param)
        {
            Debug.unityLogger.Log("OnError: " + param);
            adUnit.EmitOnAdError(adUnit);
        }

        public void OnAdEvent(string param)
        {
            Debug.unityLogger.Log("OnAdEvent");
            AdEventTypes eventType = (AdEventTypes)System.Enum.Parse(typeof(AdEventTypes), param, true);
            adUnit.EmitOnAdEvent(eventType);
        }
        #endregion
    }
}
