using TMPro;
using UnityEngine;

namespace Example.Scripts {
	public class ScoreUI : MonoBehaviour {
		public static ScoreUI Instance;
		private float _currentScore;
		private TextMeshProUGUI _text;

		private void Start() {
			_text = GetComponent<TextMeshProUGUI>();
			Instance = this;
		}

		public void UpdateScore(float score) {
			_currentScore += score;
			_text.text = $"Score: {_currentScore}";
		}
	}
}