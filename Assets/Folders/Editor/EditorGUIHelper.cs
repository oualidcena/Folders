using UnityEngine;
using UnityEditor;
using System.Collections;

namespace BeardPhantom.Folders
{
    public static class EditorGUIHelper
    {
        private static Color oldEditorColor;

        public static void SaveGUIColor(Color newColor)
        {
            oldEditorColor = GUI.color;
            GUI.color = newColor;
        }

        public static void RestoreGUIColor()
        {
            GUI.color = oldEditorColor;
        }
    }
}