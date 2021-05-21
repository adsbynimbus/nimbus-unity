using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal
{
    internal class IOSAdManager
    {
        private NimbusAdUnit adUnit;

        public IOSAdManager()
        {
            new GameObject("IOSAdManager", typeof(IOSAdManager));
        }

        public void SetAdUnit(NimbusAdUnit adUnit)
        {
            this.adUnit = adUnit;
        }

        #region iOS Event Callbacks

        public void OnIOSEventReceived(string argsJson)
        {
            Debug.unityLogger.Log("OnIOSEventReceived params: " + argsJson);
        }

        public void OnAdRendered(string param)
        {
            Debug.unityLogger.Log("OnAdRendered");
            adUnit.EmitOnAdRendered(adUnit);
        }

        public void OnError(string param)
        {
            Debug.unityLogger.Log("OnError: " + param);
            adUnit.EmitOnAdError(adUnit);
        }

        public void OnAdEvent(string param)
        {
            Debug.unityLogger.Log("OnAdRendered");
            AdEventTypes eventType = (AdEventTypes)System.Enum.Parse(typeof(AdEventTypes), param, true);
            adUnit.EmitOnAdEvent(eventType);

            //var args = JsonUtility.FromJson<Dictionary<string, dynamic>>(argsJson);
            
            //switch (eventType)
            //{
            //    case AdEventTypes.NOT_LOADED:
            //        break;
            //    case AdEventTypes.LOADED:
                    
            //        break;
            //    case AdEventTypes.IMPRESSION:
            //        break;
            //    case AdEventTypes.CLICKED:
            //        break;
            //    case AdEventTypes.PAUSED:
            //        break;
            //    case AdEventTypes.RESUME:
            //        break;
            //    case AdEventTypes.FIRST_QUARTILE:
            //        break;
            //    case AdEventTypes.MIDPOINT:
            //        break;
            //    case AdEventTypes.THIRD_QUARTILE:
            //        break;
            //    case AdEventTypes.COMPLETED:
            //        break;
            //    case AdEventTypes.DESTROYED:
            //        break;
            //}

            
            //if (activeAdUnit != null)
            //{
            //    NimbusEvents.EmitOnAdRendered(activeAdUnit);
            //}
        }
        #endregion
    }
}
