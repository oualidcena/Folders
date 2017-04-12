using UnityEngine;
using UnityEditor;
using System;

namespace BeardPhantom.Folders
{
    [CustomEditor(typeof(Folder))]
    [CanEditMultipleObjects]
    public class FolderInspectorEditor : Editor
    {
        private const string FOLDER_HELP = "Folders are zero centered, zero rotated and non-scaled GameObjects that can only store other objects as children. No other components may be added to this object, and the transform cannot be changed, either in the Editor or at runtime.";

        private SerializedProperty drawMode;
        private SerializedProperty folderDrawMode;
        private SerializedProperty labelDrawMode;
        private SerializedProperty folderColor;
        private SerializedProperty allowBrokenPath;
        /// <summary>
        /// The inspected transform
        /// </summary>
        private Transform transform;
        private Vector2 scroll;
        /// <summary>
        /// The cached tool for restoring on editor disable
        /// </summary>
        private Tool lastTool;

        private void OnEnable()
        {
            // Get properties
            transform = (target as Folder).transform;
            folderColor = serializedObject.FindProperty("_folderColor");
            drawMode = serializedObject.FindProperty("drawMode");
            labelDrawMode = serializedObject.FindProperty("labelDrawMode");
            allowBrokenPath = serializedObject.FindProperty("allowBrokenPath");

            // Disable tools
            lastTool = Tools.current;
            Tools.current = Tool.None;
        }

        private void OnDisable()
        {
            // Restores previous tool
            Tools.current = lastTool;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Disables tools
            Tools.current = Tool.None;

            //Draw help
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(FOLDER_HELP, EditorStyles.helpBox);

            EditorGUILayout.LabelField((target as Folder).path);

            if (transform.parent == null || transform.parent.gameObject.GetFolder() == null)
            {
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

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the folder path in the Scene view
        /// </summary>
        private void OnSceneGUI()
        {
            var value = (Folder.TwoWayDrawMode)Enum.GetValues(typeof(Folder.TwoWayDrawMode)).GetValue(labelDrawMode.enumValueIndex);

            if (value == Folder.TwoWayDrawMode.OnSelected)
            {
                Handles.Label(Vector3.zero, (target as Folder).path, EditorStyles.helpBox);
            }
        }
    }
}