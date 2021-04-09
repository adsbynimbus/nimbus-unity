using System;
using Nimbus.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Nimbus {
	public class GUIButtonTest: MonoBehaviour {
		public TextMeshProUGUI bannerButtonText;

		private string _currentBannerButtonText;
		private bool _shouldDestroyBanner;
		private NimbusManager _manager;
		private NimbusAdUnit _adUnit;
		private void Start() {
			var obj = GameObject.FindWithTag("NimbusAdsManager");
			_manager = obj.GetComponent<NimbusManager>();
			_currentBannerButtonText = bannerButtonText.text;
		}


		private void Update() {
			if (_adUnit == null) return;
			Debug.unityLogger.Log($"AD OF TYPE {_adUnit.AdType}, " +
			                      $"Was Ad rendered {_adUnit.AdWasRendered} " +
			                      $"Current Ad State {_adUnit.CurrentAdState} " +
			                      // $"Was there an Error Message {_adUnit?.AdListenerError.Message} " +
			                      $"Instance ID {_adUnit.InstanceID}");
		}
		

		public void LoadBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				bannerButtonText.text = "Destroy Banner";
				_adUnit = _manager.LoadAndShowBannerAd();
				return;
			}
			_adUnit.Destroy();
			_adUnit = null;
			_shouldDestroyBanner = false;
			bannerButtonText.text = _currentBannerButtonText;
		}
		
		
		
		public void LoadInterstitial() {
			_adUnit = _manager.LoadAndShowInterstitialAd();
		}
		
		public void LoadRewardedVideoAd() {
			_adUnit = _manager.LoadAndShowRewardedVideoAd();
		}
	}
}