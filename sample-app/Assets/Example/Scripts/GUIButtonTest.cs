using Nimbus.Scripts;
using Nimbus.Scripts.Internal;
using TMPro;
using UnityEngine;

namespace Example.Scripts {
	public class GUIButtonTest : MonoBehaviour {
		public TextMeshProUGUI bannerButtonText;
		private NimbusAdUnit _adUnit;

		private string _currentBannerButtonText;
		private bool _isUnique;
		private bool _shouldDestroyBanner;

		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
			;
		}

		private void Start() {
			_currentBannerButtonText = bannerButtonText.text;
		}

		private void Update() {
			if (_adUnit == null || !_isUnique) return;
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
				_adUnit = NimbusManager.Instance.LoadAndShowBannerAd("unity_demo_banner_position");
				_isUnique = true;
				return;
			}

			_adUnit.Destroy();
			_adUnit = null;
			_shouldDestroyBanner = false;
			bannerButtonText.text = _currentBannerButtonText;
		}


		public void LoadInterstitial() {
			_adUnit = NimbusManager.Instance.LoadAndShowFullScreenAd("unity_demo_interstitial_position");
			_isUnique = true;
		}

		public void LoadRewardedVideoAd() {
			_adUnit = NimbusManager.Instance.LoadAndShowRewardedVideoAd("unity_demo_video_position");
			_isUnique = true;
		}
	}
}