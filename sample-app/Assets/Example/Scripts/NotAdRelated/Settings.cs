using UnityEngine;

namespace Example.Scripts.NotAdRelated {
	public class Settings : MonoBehaviour {
		[SerializeField] private GameObject _settingsCanvas;
		private MenuState _menuState = MenuState.Closed;
			
		public void OnSettingsPressed() {
			if (_menuState == MenuState.Closed) {
				_menuState = MenuState.Opened;
				_settingsCanvas.SetActive(true);
				return;
			}
			_menuState = MenuState.Closed;
			_settingsCanvas.SetActive(false);
		}

		public void ToggleCoppa(bool value) {
			Nimbus.configuration.coppa = value;
		}
	}

	internal enum MenuState {
		Closed = 0,
		Opened = 1,
	}
}