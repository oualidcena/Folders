using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Rubycone.Shared;

namespace Rubycone.Folders {
    [InitializeOnLoad]
    public static class NoteHierarchyEditor {

        static Texture2D hNote16;

        public const string ICON_16 = "Assets/Rubycone/Folders/Resources/note_16.png";

        static NoteHierarchyEditor() {
            // Init
            hNote16 = Resources.LoadAssetAtPath<Texture2D>(ICON_16);

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static Transform[] GetParents(Transform transform) {
            var list = new List<Transform>();
            var pointer = transform;
            while(pointer.parent != null) {
                list.Add(pointer.parent);
                pointer = pointer.parent;
            }
            return list.ToArray();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            var note = obj.GetAsNote();

            if(obj == null) {
                return;
            }

            if(note != null || obj.GetComponentInChildren<Note>() != null) {
                var parents = GetParents(obj.transform);
                var noteIconRect = new Rect(selectionRect);
                noteIconRect.x = (selectionRect.width - 18) + (parents.Length * 14);
                noteIconRect.width = 20;

                var maxCharacters = 200;
                var text = string.Empty;

                if(note != null) {
                    text = note.text.Length > maxCharacters ? note.text.Substring(0, maxCharacters - 3) + "..." : note.text;
                }
                else {
                    text = string.Format("Child '{0}' has note", obj.GetComponentInChildren<Note>().name);
                    EditorGUIHelper.SaveGUIColor(new Color(1f, 1f, 1f, 0.4f));
                }

                if(GUI.Button(noteIconRect, new GUIContent(hNote16, text), GUIStyle.none) && note == null) {
                    Selection.activeObject = obj.GetComponentInChildren<Note>();
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }

                if(note == null) {
                    EditorGUIHelper.RestoreGUIColor();
                }
            }
        }

    }
}