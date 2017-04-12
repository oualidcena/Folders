using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BeardPhantom.Folders
{
    [InitializeOnLoad]
    public static class NoteHierarchyEditor
    {
        private static Texture2D hNote16;
        public const string ICON_16_PATH = "note_icon_16.png";

        static NoteHierarchyEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static Transform[] GetParents(Transform transform)
        {
            var list = new List<Transform>();
            var pointer = transform;
            while (pointer.parent != null)
            {
                list.Add(pointer.parent);
                pointer = pointer.parent;
            }
            return list.ToArray();
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            // Get icon if it unassigned
            if (hNote16 == null)
            {
                hNote16 = EditorGUIUtility.Load(ICON_16_PATH) as Texture2D;
            }

            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null)
            {
                return;
            }
            var note = obj.GetComponent<Note>();
            if (note != null || obj.GetComponentInChildren<Note>() != null)
            {
                var parents = GetParents(obj.transform);
                var noteIconRect = new Rect(selectionRect);
                noteIconRect.x = (selectionRect.width - 18) + (parents.Length * 14);
                noteIconRect.width = 20;

                var maxCharacters = 200;
                var text = string.Empty;

                if (note != null)
                {
                    text = note.text.Length > maxCharacters ? note.text.Substring(0, maxCharacters - 3) + "..." : note.text;
                }
                else
                {
                    text = string.Format("Child '{0}' has note", obj.GetComponentInChildren<Note>().name);
                    EditorGUIHelper.SaveGUIColor(new Color(1f, 1f, 1f, 0.4f));
                }

                if (GUI.Button(noteIconRect, new GUIContent(hNote16, text), GUIStyle.none) && note == null)
                {
                    Selection.activeObject = obj.GetComponentInChildren<Note>();
                    EditorGUIUtility.PingObject(Selection.activeObject);
                }

                if (note == null)
                {
                    EditorGUIHelper.RestoreGUIColor();
                }
            }
        }

    }
}