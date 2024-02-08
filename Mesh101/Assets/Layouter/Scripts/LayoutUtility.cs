using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Type = System.Type;

namespace F10.Layouter.Editor {
	/// <summary>
	/// Wraps reflection calls to UnityEditor private classes to manage editor layouts.
	/// </summary>
	public static class LayoutUtility {
		private static MethodInfo _loadLayoutMethod;

		private static PropertyInfo _layoutsModePreferencesPathProperty;

		internal static IEnumerable<string> AllLayouts => Directory.GetFiles(_layoutsModePreferencesPathProperty.GetValue(null) as string ?? string.Empty)
			.Where(path => path.EndsWith(".wlt"))
			.ToArray();

		static LayoutUtility() {
			var layout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");

			if (layout != null) {
				_loadLayoutMethod = layout.GetMethod(
					"LoadWindowLayout",
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static,
					null,
#if UNITY_2019_1_OR_NEWER
					new[] { typeof(string), typeof(bool), typeof(bool), typeof(bool) },
#else
					new[] { typeof(string), typeof(bool) },
#endif
					null);

				_layoutsModePreferencesPathProperty = layout.GetProperty(
#if UNITY_2019_1_OR_NEWER
					"layoutsModePreferencesPath",
#else
					"layoutsPreferencesPath",
#endif
					BindingFlags.NonPublic | BindingFlags.Static);
			}
		}

		/// <summary>
		/// Loads and replaces the current editor layout with the given replacement.
		/// </summary>
		/// <param name="layoutName">Name of the layout to load.</param>
		[PublicAPI]
		public static void LoadLayout(string layoutName) {
			var path = GetLayoutPath(layoutName);
			if (string.IsNullOrEmpty(path)) return;

#if UNITY_2019_1_OR_NEWER
			_loadLayoutMethod.Invoke(null, new object[] { path, false, true, true });
#else
			_loadLayoutMethod.Invoke(null, new object[] { path, false });
#endif
		}

		/// <summary>
		/// Returns the absolute path of a given editor layout.
		/// </summary>
		/// <param name="layoutName">Name of the layout to get the path from.</param>
		/// <returns>Absolute path or null if not found.</returns>
		[PublicAPI]
		[CanBeNull]
		public static string GetLayoutPath(string layoutName) {
			if (layoutName == "Don't Change") return null;

			foreach (var layout in AllLayouts) {
				if (layout.EndsWith($"{layoutName}.wlt")) return layout;
			}

			Debug.LogError($"No layout '{layoutName}' found! It may have been renamed or deleted");
			return null;
		}
	}
}