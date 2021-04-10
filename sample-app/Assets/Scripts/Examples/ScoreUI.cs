using TMPro;
using UnityEngine;

namespace Examples {
    public class ScoreUI : MonoBehaviour {
        private TextMeshProUGUI _text;
        private float _currentScore = 0f;
        public static ScoreUI Instance;

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
