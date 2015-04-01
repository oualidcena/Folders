using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.BoltAction {
    [CustomEditor(typeof(Note))]
    public class NoteInspectorEditor : Editor {
        SerializedProperty text, resolveBeforeBuild;
        Vector2 scroll = Vector2.zero;

        public const string ICON = "Assets/Gizmos/note.png";

        private const string NOTE_HELP = "Notes are simple text-based metadata that you can attach to any GameObject (even Folders!). If a GameObject has a note, an icon will appear in the Hierarchy. They are destroyed at runtime in builds.";

        void OnEnable() {
            text = serializedObject.FindProperty("_text");
            resolveBeforeBuild = serializedObject.FindProperty("_resolveBeforeBuild");
        }

        public override void OnInspectorGUI() {
            EditorStyles.textField.wordWrap = true;
            serializedObject.Update();

            //Draw help
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(NOTE_HELP, EditorStyles.helpBox);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(resolveBeforeBuild);


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Note Text", EditorStyles.boldLabel);
            scroll = EditorGUILayout.BeginScrollView(scroll, false, false, GUILayout.MaxHeight(250f));
            text.stringValue = EditorGUILayout.TextArea(text.stringValue, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}