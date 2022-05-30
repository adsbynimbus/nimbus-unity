using System;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Nimbus.Internal {
	internal class AdManagerListener : AndroidJavaProxy {
		private readonly AndroidJavaClass _helper;
		private NimbusAdUnit _adUnit;

		internal AdManagerListener(in AndroidJavaClass helper, ref NimbusAdUnit adUnit) : base(
			"com.adsbynimbus.NimbusAdManager$Listener") {
			_adUnit = adUnit;
			_helper = helper;
		}

		private void onAdResponse(AndroidJavaObject response) {
			_adUnit.FireMobileAdEvents(AdEventTypes.LOADED);
		}

		private void onAdRendered(AndroidJavaObject controller) {
			_helper.CallStatic("addListener", controller, new AdControllerListener(ref _adUnit));
			Debug.unityLogger.Log("Ad was rendered");
			_adUnit.AdWasRendered = true;
			_adUnit.SetAndroidController(controller);
			_adUnit.SetAndroidHelper(_helper);
			_adUnit.FireMobileAdRenderedEvent();
		}

		private void onError(AndroidJavaObject adError) {
			var errMessage = adError.Call<string>("getMessage");
			Debug.unityLogger.LogError("Android", $"Listener Ad error: {errMessage}");
			_adUnit.FireMobileOnAdErrorEvent();
		}
	}

	internal class AdControllerListener : AndroidJavaProxy {
		private readonly NimbusAdUnit _adUnit;

		public AdControllerListener(ref NimbusAdUnit adUnit) : base(
			"com.adsbynimbus.render.AdController$Listener") {
			_adUnit = adUnit;
		}

		private void onAdEvent(AndroidJavaObject adEvent) {
			Debug.unityLogger.Log("Ad event " + adEvent.Call<string>("name"));
			var eventState = adEvent.Call<string>("name");
			if (!Enum.TryParse(eventState, out AdEventTypes state)) return;
			_adUnit.FireMobileAdEvents(state);
		}

		private void onError(AndroidJavaObject adError) {
			var errMessage = adError.Call<string>("getMessage");
			Debug.unityLogger.LogError("Android", $"Listener Ad error: {errMessage}");
			_adUnit.FireMobileOnAdErrorEvent();
		}
	}
}