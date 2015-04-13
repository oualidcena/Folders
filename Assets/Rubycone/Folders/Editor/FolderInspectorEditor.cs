using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Rubycone.Folders {
    [CustomEditor(typeof(Folder))]
    [CanEditMultipleObjects]
    public class FolderInspectorEditor : Editor {
        static SerializedProperty drawMode, folderDrawMode, labelDrawMode, folderColor, allowBrokenPath;
        static Transform transform;
        Vector2 scroll;

        private const string FOLDER_HELP = "Folders are zero centered, zero rotated and non-scaled GameObjects that can only store other objects as children. No other components may be added to this object, and the transform cannot be changed, either in the Editor or at runtime.";

        static Tool lastTool;

        void OnEnable() {
            transform = (target as Folder).transform;
            folderColor = serializedObject.FindProperty("_folderColor");
            drawMode = serializedObject.FindProperty("drawMode");
            labelDrawMode = serializedObject.FindProperty("labelDrawMode");
            allowBrokenPath = serializedObject.FindProperty("allowBrokenPath");


            lastTool = Tools.current;
            Tools.current = Tool.None;
        }

        void OnDisable() {
            Tools.current = lastTool;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            Tools.current = Tool.None;

            //Draw help
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(FOLDER_HELP, EditorStyles.helpBox);

            EditorGUILayout.LabelField((target as Folder).path);

            if(transform.parent == null || transform.parent.gameObject.GetAsFolder() == null) {
                //Root folder settings
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Root Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(folderColor);
                EditorGUILayout.PropertyField(allowBrokenPath);
            }

            //Draw settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(drawMode);
            EditorGUILayout.PropertyField(labelDrawMode);

            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Global Actions", EditorStyles.boldLabel);
            //if(GUILayout.Button("Alphabetize Folders")) {

            //}

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI() {
            var value = (Folder.TwoWayDrawMode)Enum.GetValues(typeof(Folder.TwoWayDrawMode)).GetValue(labelDrawMode.enumValueIndex);

            if(value == Folder.TwoWayDrawMode.OnSelected) {
                Handles.Label(Vector3.zero, (target as Folder).path, EditorStyles.helpBox);
            }
        }
    }
}