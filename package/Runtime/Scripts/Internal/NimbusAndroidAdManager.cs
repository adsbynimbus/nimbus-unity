using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Nimbus.Runtime.Scripts.Internal {
	internal class AdManagerListener : AndroidJavaProxy {
		private readonly AndroidJavaClass _helper;
		private readonly ILogger _logger;
		private NimbusAdUnit _adUnit;

		internal AdManagerListener(ILogger logger, in AndroidJavaClass helper, ref NimbusAdUnit adUnit) : base(
			"com.adsbynimbus.NimbusAdManager$Listener") {
			_adUnit = adUnit;
			_helper = helper;
			_logger = logger;
		}

		private void onAdResponse(AndroidJavaObject response) {
			_adUnit.ResponseMetaData = new MetaData(response);
		}

		private void onAdRendered(AndroidJavaObject controller) {
			_helper.CallStatic("addListener", controller, new AdControllerListener(_logger, ref _adUnit));
			_logger.Log("Ad was rendered");
			_adUnit.AdWasRendered = true;
			_adUnit.SetAndroidController(controller);
			_adUnit.SetAndroidHelper(_helper);
			_adUnit.EmitOnAdRendered(_adUnit);
		}

		private void onError(AndroidJavaObject adError) {
			var errMessage = adError.Call<string>("getMessage");
			_logger.Log($"Listener Ad error: {errMessage}");
			_adUnit.AdListenerError = new AdError(errMessage);
			_adUnit.EmitOnAdError(_adUnit);
		}
	}

	internal class AdControllerListener : AndroidJavaProxy {
		private readonly NimbusAdUnit _adUnit;
		private readonly ILogger _logger;

		public AdControllerListener(ILogger logger, ref NimbusAdUnit adUnit) : base(
			"com.adsbynimbus.render.AdController$Listener") {
			_logger = logger;
			_adUnit = adUnit;
		}

		private void onAdEvent(AndroidJavaObject adEvent) {
			_logger.Log("Ad event " + adEvent.Call<string>("name"));
			var eventState = adEvent.Call<string>("name");

			if (!Enum.TryParse(eventState, out AdEventTypes state)) return;
			_adUnit.CurrentAdState = state;
			_adUnit.EmitOnAdEvent(state);
		}

		private void onError(AndroidJavaObject adError) {
			var errMessage = adError.Call<string>("getMessage");
			_logger.Log($"Controller ad error: {errMessage}");
			_adUnit.AdControllerError = new AdError(errMessage);
			_adUnit.EmitOnAdError(_adUnit);
		}
	}
}