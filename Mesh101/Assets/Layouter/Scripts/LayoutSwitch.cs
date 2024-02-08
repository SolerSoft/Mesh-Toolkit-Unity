using System.Threading.Tasks;
using UnityEditor;

namespace F10.Layouter.Editor {
	[InitializeOnLoad]
	internal static class LayoutSwitch {
		static LayoutSwitch() {
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
			EditorApplication.playModeStateChanged += PlayModeStateChanged;

			EditorApplication.pauseStateChanged -= PauseModeStateChanged;
			EditorApplication.pauseStateChanged += PauseModeStateChanged;
		}

		private static async void PlayModeStateChanged(PlayModeStateChange playModeStateChange) {
			if (!LayouterPreferences.Data.LayoutSwitch) return;

			switch (playModeStateChange) {
				case PlayModeStateChange.EnteredEditMode:
					await Task.Delay(1); // Prevents issues where a window is waiting for this exact call to do something
					LayoutUtility.LoadLayout(LayouterPreferences.Data.EditLayout);
					break;
				case PlayModeStateChange.EnteredPlayMode:
					await Task.Delay(1); // Prevents issues where a window is waiting for this exact call to do something
					LayoutUtility.LoadLayout(LayouterPreferences.Data.PlayLayout);
					break;
			}
		}

		private static void PauseModeStateChanged(PauseState pauseState) {
			if (!LayouterPreferences.Data.LayoutSwitch) return;

			switch (pauseState) {
				case PauseState.Paused:
					LayoutUtility.LoadLayout(LayouterPreferences.Data.PauseLayout);
					break;
				case PauseState.Unpaused:
					LayoutUtility.LoadLayout(EditorApplication.isPlaying ? LayouterPreferences.Data.PlayLayout : LayouterPreferences.Data.EditLayout);
					break;
			}
		}
	}
}