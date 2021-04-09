using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Nimbus.Internal {
	internal class AdManagerListener : AndroidJavaProxy {
		private readonly ILogger _logger;
		private readonly AndroidJavaClass _helper;
		private  NimbusAdUnit _adUnit;

		public AdManagerListener(ILogger logger, ref AndroidJavaClass helper, ref NimbusAdUnit adUnit) : base("com.adsbynimbus.NimbusAdManager$Listener") {
			_logger = logger;
			_helper = helper;
			_adUnit = adUnit;
		}

		private void onAdResponse(AndroidJavaObject response) {
			_logger.Log("responded with ad type " + response.Call<string>("type"));
			//TODO add event and maybe proxy response data to the AdUnit Object
		}

		private void onAdRendered(AndroidJavaObject controller) {
			_helper.CallStatic("addListener", controller, new AdControllerListener(_logger, ref _adUnit));
			_logger.Log("Ad was rendered");
			_adUnit.SetAndroidController(ref controller);
			_adUnit.EmitOnAdRendered(_adUnit);
		}

		private void onError(AndroidJavaObject adError) {
			var errMessage = adError.Call<string>("getMessage");
			_logger.Log("Ad error " + errMessage);
			_adUnit.AdListenerError = new AdError(errMessage);
			_adUnit.EmitOnAdError(_adUnit);
		}
	}

	internal class AdControllerListener : AndroidJavaProxy {
		private readonly ILogger _logger;
		private readonly NimbusAdUnit _adUnit;

		public AdControllerListener(ILogger logger, ref NimbusAdUnit adUnit) : base("com.adsbynimbus.render.AdController$Listener") {
			_logger = logger;
			_adUnit = adUnit;
		}

		private void onAdEvent(AndroidJavaObject adEvent) {
			_logger.Log("Ad event " + adEvent.Call<string>("name"));
			var eventState = adEvent.Call<string>("name");
			Enum.TryParse(eventState, out AdEventTypes state);
			_adUnit.CurrentAdState = state;
			_adUnit.EmitOnAdEvent(_adUnit);
		}
			
		private void onError(AndroidJavaObject adError) { 
			var errMessage = adError.Call<string>("getMessage");
			_logger.Log("Ad error " + errMessage);
			_adUnit.AdControllerError = new AdError(errMessage);
			_adUnit.EmitOnAdError(_adUnit);
		}
	}
}