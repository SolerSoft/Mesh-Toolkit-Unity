using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace F10.Layouter.Editor {
	/// <summary>
	/// Handles support for finding, opening and/or focusing Editor windows, including internal and private classes not normally accessible with EditorWindow.GetWindow().
	/// </summary>
	[InitializeOnLoad]
	public static class WindowFocus {
		private static readonly Assembly[] _allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

		static WindowFocus() {
			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
			EditorApplication.playModeStateChanged += PlayModeStateChanged;

			EditorApplication.pauseStateChanged -= PauseModeStateChanged;
			EditorApplication.pauseStateChanged += PauseModeStateChanged;
		}

		private static void PlayModeStateChanged(PlayModeStateChange playModeStateChange) {
			if (!LayouterPreferences.Data.WindowFocus) return;

			switch (playModeStateChange) {
				case PlayModeStateChange.EnteredEditMode:
					FindAndOpenWindows(LayouterPreferences.Data.EditWindowsFocus);
					break;
				case PlayModeStateChange.EnteredPlayMode:
					FindAndOpenWindows(LayouterPreferences.Data.PlayWindowsFocus);
					break;
			}
		}

		private static void PauseModeStateChanged(PauseState pauseState) {
			if (!LayouterPreferences.Data.WindowFocus) return;

			switch (pauseState) {
				case PauseState.Paused:
					FindAndOpenWindows(LayouterPreferences.Data.PauseWindowsFocus);
					break;
				case PauseState.Unpaused:
					FindAndOpenWindows(EditorApplication.isPlaying ? LayouterPreferences.Data.PlayWindowsFocus : LayouterPreferences.Data.EditWindowsFocus);
					break;
			}
		}

		/// <summary>
		/// Finds, opens and/or focuses all the given Editor windows by type.
		/// </summary>
		/// <param name="listOfTypes">List of full name EditorWindow types.</param>
		[PublicAPI]
		public static void FindAndOpenWindows(List<string> listOfTypes) {
			foreach (var window in listOfTypes) {
				FindAndOpenWindow(window);
			}
		}

		/// <summary>
		/// Finds, opens and/or focuses the given EditorWindow type.
		/// </summary>
		/// <param name="windowType">Full type name of an EditorWindow.</param>
		[PublicAPI]
		public static void FindAndOpenWindow(string windowType) {
			foreach (var assembly in _allAssemblies) {
				var allTypes = assembly.GetTypes();
				foreach (var type in allTypes) {
					if (type.FullName != windowType) continue;

					EditorWindow.GetWindow(type).Show();
					return;
				}
			}
		}
	}
}