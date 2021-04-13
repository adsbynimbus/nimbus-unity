using System;
using Nimbus;
using Nimbus.Internal;
using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Mathematics;

namespace Examples {
	public class BannerExample2: MonoBehaviour {
		public float bannerRefreshInterval = 5f;

		private bool _alreadyTriggered;
		private IEnumerator _bannerRoutine;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_bannerRoutine = NimbusManager.Instance.LoadAndShowBannerAdWithRefresh(bannerRefreshInterval);
			StartCoroutine(_bannerRoutine);
			_alreadyTriggered = true;
		}
		
		public void StopBannerRefresh() {
			StopCoroutine(_bannerRoutine);
		}
	}
}