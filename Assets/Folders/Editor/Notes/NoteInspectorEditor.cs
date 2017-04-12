using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BeardPhantom.Folders
{
    [CustomEditor(typeof(Note))]
    [CanEditMultipleObjects]
    public class NoteInspectorEditor : Editor
    {
        private SerializedProperty text;
        private SerializedProperty resolveBeforeBuild;
        private Vector2 scroll = Vector2.zero;

        private const string NOTE_HELP = "Notes are simple text-based metadata that you can attach to any GameObject (even Folders!). If a GameObject has a note, an icon will appear in the Hierarchy. They are destroyed at runtime in builds.";

        private void OnEnable()
        {
            text = serializedObject.FindProperty("_text");
            resolveBeforeBuild = serializedObject.FindProperty("_resolveBeforeBuild");
        }

        public override void OnInspectorGUI()
        {
            // Enable word wrapping
            EditorStyles.textField.wordWrap = true;
            serializedObject.Update();

            //Draw help
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(NOTE_HELP, EditorStyles.helpBox);

            // Draw resolve before build toggle
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(resolveBeforeBuild);
            GUI.enabled = true;

            // Draw textbox
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