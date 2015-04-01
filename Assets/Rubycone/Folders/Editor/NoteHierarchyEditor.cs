using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.BoltAction {
    [InitializeOnLoad]
    public static class NoteHierarchyEditor {

        static Texture2D hNoteYellow, hNoteGrey;

        public const string ICON_16 = "Assets/Gizmos/note_16.png";
        public const string GREY_ICON_16 = "Assets/Gizmos/grey_note_16.png";

        static NoteHierarchyEditor() {
            // Init
            hNoteYellow = LoadTex(ICON_16);

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static Texture2D LoadTex(string path) {
            return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
            var obj = (EditorUtility.InstanceIDToObject(instanceID) as GameObject);
            var note = obj.GetAsNote();

            if(note != null) {
                var noteIconRect = new Rect(selectionRect);
                noteIconRect.x = noteIconRect.width;
                noteIconRect.width = 20;

                var maxCharacters = 200;
                var text = note.text.Length > maxCharacters ? note.text.Substring(0, maxCharacters - 3) + "..." : note.text;
                GUI.Label(noteIconRect, new GUIContent(hNoteYellow, text));
            }
        }

    }
}