using System;
using System.Collections.Generic;
using UnityEngine;

namespace F10.Layouter.Editor {
	[Serializable]
	internal class LayouterPreferencesData {
		internal static readonly string[] DefaultHiddenWindows = {
			// Generic
			"UnityEditor.AddComponent.AddComponentWindow",
			"UnityEditor.AddShaderVariantWindow",
			"UnityEditor.IMGUI.Controls.AdvancedDropdownWindow",
			"UnityEditor.ColorPicker",
			"UnityEditor.SketchUpImportDlg",
			"UnityEditor.BumpMapSettingsFixingWindow",
			"UnityEditor.AnnotationWindow",
			"UnityEditor.AssetSaveDialog",
			"UnityEditor.PragmaFixingWindow",
			"UnityEditor.PropertyEditor",
			"UnityEditor.SaveWindowLayout",
			"UnityEditor.SafeModeToolbarWindow",
			"UnityEditor.PopupWindow",
			"UnityEditor.PreviewWindow",
			"UnityEditor.SaveWindowLayout",
			"UnityEditor.FallbackEditorWindow",
			"UnityEditor.UIAutomation.TestEditorWindow",
			"UnityEditor.ScriptableWizard",
			"UnityEditor.UISystemPreviewWindow",
			// Graphs
			"UnityEditor.Graphs.AnimationStateMachine.AddStateMachineBehaviourComponentWindow",
			"UnityEditor.Graphs.LayerSettingsWindow",
			// Presets
			"UnityEditor.Presets.AddPresetTypeWindow",
			"UnityEditor.Presets.PresetSelector",
			// ShortcutManagement
			"UnityEditor.ShortcutManagement.ConflictResolverWindow",
			"UnityEditor.ShortcutManagement.DeleteShortcutProfileWindow",
			"UnityEditor.ShortcutManagement.PromptWindow",
			// VersionControl
			"UnityEditor.VersionControl.WindowCheckoutFailure",
			"UnityEditor.VersionControl.WindowResolve",
			// VisualScripting
			"Unity.VisualScripting.FuzzyWindow",
			// UnityEditorInternal
			"UnityEditorInternal.AddCurvesPopup",
			"UnityEditorInternal.Profiling.ProfilerFrameDataViewBase+SelectedSampleStackWindow",
			// JetBrains
			"JetBrains.Rider.Unity.Editor.Navigation.Window.FindUsagesWindow"
		};

		[SerializeField]
		private bool _layoutSwitch = false;

		[SerializeField]
		private string _editLayout;

		[SerializeField]
		private string _playLayout;

		[SerializeField]
		private string _pauseLayout;

		[SerializeField]
		private bool _windowFocus = false;

		[SerializeField]
		private List<string> _editWindowsFocus = new List<string>();

		[SerializeField]
		private List<string> _playWindowsFocus = new List<string>();

		[SerializeField]
		private List<string> _pauseWindowsFocus = new List<string>();

		[SerializeField]
		private string _hiddenWindows = string.Join(",", DefaultHiddenWindows);

		internal bool LayoutSwitch {
			get => _layoutSwitch;
			set => _layoutSwitch = value;
		}

		internal string EditLayout {
			get => _editLayout;
			set => _editLayout = value;
		}

		internal string PlayLayout {
			get => _playLayout;
			set => _playLayout = value;
		}

		internal string PauseLayout {
			get => _pauseLayout;
			set => _pauseLayout = value;
		}

		internal bool WindowFocus {
			get => _windowFocus;
			set => _windowFocus = value;
		}

		internal List<string> EditWindowsFocus {
			get => _editWindowsFocus;
			set => _editWindowsFocus = value;
		}

		internal List<string> PlayWindowsFocus {
			get => _playWindowsFocus;
			set => _playWindowsFocus = value;
		}

		internal List<string> PauseWindowsFocus {
			get => _pauseWindowsFocus;
			set => _pauseWindowsFocus = value;
		}

		internal string HiddenWindows {
			get => _hiddenWindows;
			set => _hiddenWindows = value;
		}
	}
}