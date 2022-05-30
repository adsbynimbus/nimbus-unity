using Example.Scripts.NotAdRelated;
using Nimbus.Internal;
using Nimbus.Runtime.Scripts;
using Unity.Mathematics;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for banner ad within a game context
	/// </summary>
	public class BannerExample : MonoBehaviour {
		public GameObject fire;
		public float fadeSpeed = .01f;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;
		private SpriteRenderer _renderer;
		private bool _shouldDestroyAd;

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


		private void OnDestroy() {
			_ad?.Destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.RequestBannerAdAndLoad("unity_demo_banner_position");
			_alreadyTriggered = true;
		}


		public void DestroyBannerAd() {
			_shouldDestroyAd = true;
		}
	}
}