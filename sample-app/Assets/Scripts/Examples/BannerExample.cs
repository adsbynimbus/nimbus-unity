using Nimbus;
using Nimbus.Internal;
using UnityEngine;
using Unity.Mathematics;

namespace Examples {
	public class BannerExample: MonoBehaviour {
		public GameObject fire;
		public float fadeSpeed = .01f;
		private NimbusAdUnit _ad;
		private SpriteRenderer _renderer;
		private bool _shouldDestroyAd;
		private bool _alreadyTriggered;

		private void Awake() {
			_renderer = GetComponent<SpriteRenderer>();
		}

		private void Update() {
			if (!_shouldDestroyAd) return;
			
			fire.SetActive(true);
			var alpha = _renderer.color.a;
			if (alpha > .1f) {
				alpha = math.lerp(alpha, 0, fadeSpeed * Time.deltaTime);
				var color = _renderer.color;
				color = new Color(color.r, color.g, color.b, alpha);
				_renderer.color = color;
			}
			else {
				gameObject.SetActive(false);
				fire.SetActive(false);
				_ad?.Destroy();
			}
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.LoadAndShowBannerAd("unity_demo_banner_position");
			_alreadyTriggered = true;
		}
		

		private void OnDestroy() {
			_ad?.Destroy();
		}
		

		public void DestroyBannerAd() {
			_shouldDestroyAd = true;
		}

	}
}