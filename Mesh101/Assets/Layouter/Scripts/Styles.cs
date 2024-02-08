using UnityEditor;
using UnityEngine;

namespace F10.Layouter.Editor {
    internal static class Styles {
        
        internal static GUIStyle MarginGroup { get; }

        internal static GUIStyle WrappedArea { get; }

        internal static GUIStyle FocusMessage { get; }
        
        static Styles() {
            MarginGroup = new GUIStyle {
                margin = new RectOffset(6, 6, 10, 10)
            };
            
            WrappedArea = new GUIStyle(EditorStyles.textArea) {
                wordWrap = true
            };
            
            FocusMessage = new GUIStyle(EditorStyles.helpBox) {
                fontSize = 14,
                padding = new RectOffset(10,10,10,10),
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }
    }
}
