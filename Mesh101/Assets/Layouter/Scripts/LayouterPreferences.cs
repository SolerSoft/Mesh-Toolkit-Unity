using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F10.Layouter.Editor {
	internal static class LayouterPreferences {
		private const string EditorPrefsKey = "F10.Layouter:data";

		private const string Version = "1.0.0";

#if UNITY_2019_1_OR_NEWER
		private const string PreferenceSettingsWindow = "UnityEditor.PreferenceSettingsWindow";
#else
		private const string PreferenceSettingsWindow = "UnityEditor.SettingsWindow";
#endif

		private static Type[] _allWindowTypes = null;

		private static IEnumerable<Type> AllWindowTypes {
			get {
				if (_allWindowTypes != null) return _allWindowTypes;

				var result = new List<Type>();
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var assembly in assemblies) {
					var types = assembly.GetTypes();
					result.AddRange(
						types.Where(type =>
							type.IsSubclassOf(typeof(EditorWindow)) &&
							!type.IsSubclassOf(typeof(ScriptableWizard)) &&
							!type.IsSubclassOf(typeof(PopupWindow)) &&
							!type.IsGenericType &&
							!type.IsAbstract));
				}

				_allWindowTypes = result.OrderBy(x => x.Name).ToArray();
				return _allWindowTypes;
			}
		}

		private static string[] _layouts = null;

		private static string[] Layouts {
			get {
				if (_layouts != null) return _layouts;

				var layouts = new List<string> { "Don't Change" };
				var paths = LayoutUtility.AllLayouts;
				layouts.AddRange(from path in paths select Path.GetFileNameWithoutExtension(path));

				_layouts = layouts.ToArray();
				return _layouts;
			}
		}

		private static ICollection<string> _awaitFocusList = null;

		private static EditorWindow _lastSelectedWindow = null;

		private static bool _hiddenWindowsFoldout = false;

		private static LayouterPreferencesData _data = null;

		internal static LayouterPreferencesData Data {
			get {
				if (_data == null) {
					var loadedJsonPrefs = EditorPrefs.GetString(EditorPrefsKey, "{}");
					// Make sure we avoid any issues between Unity versions
					if (string.IsNullOrWhiteSpace(loadedJsonPrefs)) {
						loadedJsonPrefs = "{}";
					}

					_data = JsonUtility.FromJson<LayouterPreferencesData>(loadedJsonPrefs);
				}

				return _data;
			}
		}

		private static void SavePreferences() {
			var jsonPrefs = JsonUtility.ToJson(Data);
			EditorPrefs.SetString(EditorPrefsKey, jsonPrefs);
		}

#if UNITY_2018_3_OR_NEWER
		[SettingsProvider]
		private static SettingsProvider CreateSettingsProvider() {
			return new SettingsProvider("Preferences/Layouter", SettingsScope.User) {
				label = "Layouter",
				guiHandler = searchContext => PreferencesGUI(),

				keywords = new HashSet<string>(new[] { "Layout", "Window", "Focus", "Switch", "Auto", "F10" })
			};
		}
#else
		[PreferenceItem("Layouter")]
#endif
		private static void PreferencesGUI() {
			EditorGUIUtility.labelWidth = 250;
			EditorGUILayout.BeginVertical(Styles.MarginGroup);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Asset Store", GUILayout.Width(100))) {
				Application.OpenURL("https://assetstore.unity.com/packages/slug/97581?aid=1100lGhE");
			}

			if (GUILayout.Button("Documentation", GUILayout.Width(130))) {
				Application.OpenURL("https://docs.f10.dev/layouter");
			}

			GUILayout.EndHorizontal();

			if (GUILayout.Button($"F10.DEV - v{Version}", EditorStyles.miniLabel)) {
				Application.OpenURL("https://f10.dev");
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField(new GUIContent("Layout Auto-Switch \u24D8", "Switch between entire editor layouts when a Play, Pause or Stop (Edit mode) event happens"), EditorStyles.boldLabel);
			Data.LayoutSwitch = EditorGUILayout.Toggle("Enable Layout Switch", Data.LayoutSwitch);

			if (Data.LayoutSwitch) {
				int FindLayoutIndex(string layoutName) {
					for (int i = 0; i < Layouts.Length; i++) {
						if (Layouts[i] == layoutName) return i;
					}

					return 0;
				}

				Data.EditLayout = Layouts[EditorGUILayout.Popup("Edit Layout", FindLayoutIndex(Data.EditLayout), Layouts)];
				Data.PlayLayout = Layouts[EditorGUILayout.Popup("Play Layout", FindLayoutIndex(Data.PlayLayout), Layouts)];
				Data.PauseLayout = Layouts[EditorGUILayout.Popup("Pause Layout", FindLayoutIndex(Data.PauseLayout), Layouts)];
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField(new GUIContent("Window Auto-Focus \u24D8", "Focus and/or open any Editor window when a Play, Pause or Stop (Edit mode) event happens"), EditorStyles.boldLabel);
			Data.WindowFocus = EditorGUILayout.Toggle("Enable Window Focus", Data.WindowFocus);

			if (Data.WindowFocus) {
				void FocusSection(string title, List<string> focusList) {
					GUILayout.BeginHorizontal();

					GUILayout.Button(
						new GUIContent($"{title}"),
						EditorStyles.toolbarButton,
						GUILayout.Width(EditorGUIUtility.labelWidth));

					if (GUILayout.Button(
						EditorGUIUtility.IconContent("Toolbar Plus More"),
						EditorStyles.toolbarButton,
						GUILayout.Width(20))) {
						var windowsMenu = new GenericMenu();

						foreach (var windowType in AllWindowTypes) {
							// ReSharper disable once AssignNullToNotNullAttribute
							if (Data.HiddenWindows.Contains(windowType.FullName)) continue;

							var splitFullName = windowType.FullName.Split('.');
							var menuPath = string.Empty;
							for (int i = 0; i < splitFullName.Length; i++) {
								menuPath += splitFullName[i];
								if (i < splitFullName.Length - 1) {
									menuPath += "/";
								}
							}

							windowsMenu.AddItem(
								new GUIContent(menuPath),
								focusList.Contains(windowType.FullName),
								fullName => { OnWindowSelected(focusList, (string)fullName); },
								windowType.FullName);
						}

						windowsMenu.ShowAsContext();
					}

					if (GUILayout.Button(
#if UNITY_2019_1_OR_NEWER
						EditorGUIUtility.IconContent("scenepicking_pickable_hover"),
#else
						EditorGUIUtility.IconContent("eyeDropper.Large"),
#endif
						EditorStyles.toolbarButton,
						GUILayout.Width(20))) {
						EditorApplication.update -= TrackWindowSelection;
						EditorApplication.update += TrackWindowSelection;

						_lastSelectedWindow = null;
						_awaitFocusList = focusList;
					}


					if (GUILayout.Button(
						"Deselect All",
						EditorStyles.toolbarButton,
						GUILayout.Width(80))) {
						focusList.Clear();
						SavePreferences();
					}

					GUILayout.Button(string.Empty, EditorStyles.toolbarButton);

					GUILayout.EndHorizontal();
					GUILayout.BeginVertical(EditorStyles.helpBox);

					foreach (var fullName in focusList) {
						GUILayout.BeginHorizontal();

						if (GUILayout.Button(
							EditorGUIUtility.IconContent("TreeEditor.Trash"),
							EditorStyles.miniButtonLeft,
							GUILayout.Width(20))) {
							focusList.Remove(fullName);
							SavePreferences();
							break;
						}

						if (GUILayout.Button(
#if UNITY_2019_1_OR_NEWER
							EditorGUIUtility.IconContent("winbtn_win_restore"),
#else
							EditorGUIUtility.IconContent("LookDevSingle1"),
#endif
							EditorStyles.miniButtonRight,
							GUILayout.Width(20))) {
							WindowFocus.FindAndOpenWindow(fullName);
						}

						EditorGUILayout.LabelField(fullName);

						GUILayout.EndHorizontal();
					}

					GUILayout.EndVertical();
					GUILayout.Space(4);
				}

				GUILayout.Space(4);

				if (DrawWindowSelection()) {
					GUILayout.EndVertical();
					return;
				}

				FocusSection("Edit Mode", Data.EditWindowsFocus);
				FocusSection("Play Mode", Data.PlayWindowsFocus);
				FocusSection("Pause Mode", Data.PauseWindowsFocus);

				_hiddenWindowsFoldout = EditorGUILayout.Foldout(_hiddenWindowsFoldout, new GUIContent("Hidden Windows \u24D8", "Contains a coma-separated full type name list of editor windows that shouldn't be shown whn selecting windows"));
				if (_hiddenWindowsFoldout) {
					Data.HiddenWindows = EditorGUILayout.TextArea(Data.HiddenWindows, Styles.WrappedArea);
					if (GUILayout.Button("Reset To Defaults", GUILayout.Width(120))) {
						Data.HiddenWindows = string.Join(",", LayouterPreferencesData.DefaultHiddenWindows);
					}
				}
			}

			EditorGUILayout.EndVertical();

			if (GUI.changed) {
				SavePreferences();
			}
		}

		private static bool DrawWindowSelection() {
			if (_awaitFocusList == null) return false;

			EditorGUILayout.LabelField("Click the Editor Window to add to the focus list", Styles.FocusMessage);
			if (_lastSelectedWindow != null) {
				EditorGUILayout.LabelField(
					$"Selected Window: <b>{_lastSelectedWindow.titleContent.text}</b>\nClick back on this window to confirm",
					Styles.FocusMessage);
			} else {
				if (GUILayout.Button("Cancel")) {
					EditorApplication.update -= TrackWindowSelection;
					_awaitFocusList = null;
				}
			}

			return true;
		}

		private static void OnWindowSelected(ICollection<string> focusList, string fullName) {
			if (focusList.Contains(fullName)) {
				focusList.Remove(fullName);
			} else {
				focusList.Add(fullName);
			}

			SavePreferences();
		}

		private static void TrackWindowSelection() {
			var thisWindow = EditorWindow.focusedWindow.GetType().FullName == PreferenceSettingsWindow;

			if (_lastSelectedWindow != null && thisWindow) {
				OnWindowSelected(_awaitFocusList, _lastSelectedWindow.GetType().FullName);
				_awaitFocusList = null;

				EditorApplication.update -= TrackWindowSelection;
			} else if (!thisWindow) {
				_lastSelectedWindow = EditorWindow.focusedWindow;
			}
		}
	}
}