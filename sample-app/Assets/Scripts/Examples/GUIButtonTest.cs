using Nimbus;
using Nimbus.Internal;
using TMPro;
using UnityEngine;

namespace Examples {
	public class GUIButtonTest: MonoBehaviour {
		public TextMeshProUGUI bannerButtonText;

		private string _currentBannerButtonText;
		private bool _shouldDestroyBanner;
		private bool _isUnique;
		private NimbusManager _manager;
		private NimbusAdUnit _adUnit;
		private void Start() {
			var obj = GameObject.FindWithTag("NimbusAdsManager");
			_manager = obj.GetComponent<NimbusManager>();
			_currentBannerButtonText = bannerButtonText.text;
		}
		
		private void Update() {
			if (_adUnit == null||!_isUnique ) return;
			_isUnique = false;
			Debug.unityLogger.Log($"AD OF TYPE {_adUnit.AdType}, " +
			                      $"Was Ad rendered {_adUnit.WasAdRendered()} " +
			                      $"Current Ad State {_adUnit.GetCurrentAdState()} " +
			                      $"Instance ID {_adUnit.InstanceID}");
		}
		

		public void LoadBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				bannerButtonText.text = "Destroy Banner";
				_adUnit = _manager.LoadAndShowBannerAd();
				_isUnique = true;
				return;
			}
			_adUnit.Destroy();
			_adUnit = null;
			_shouldDestroyBanner = false;
			bannerButtonText.text = _currentBannerButtonText;
		}
		
		
		
		public void LoadInterstitial() {
			_adUnit = _manager.LoadAndShowInterstitialAd();
			_isUnique = true;
		}
		
		public void LoadRewardedVideoAd() {
			_adUnit = _manager.LoadAndShowRewardedVideoAd();
			_isUnique = true;
		}
	}
}