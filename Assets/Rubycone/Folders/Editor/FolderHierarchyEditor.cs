using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Rubycone.BoltAction {
    [InitializeOnLoad]
    public static class FolderHierarchyEditor {

        static Texture2D hFolder;

        public const string ICON_16 = "Assets/Gizmos/folder_icon_16.png";
        public const string ICON_32 = "Assets/Gizmos/folder_icon_32.png";

        static Color oldEditorColor;
        static Color highlightColor = new Color(1f, 1f, 0f, 0.2f);


        static FolderHierarchyEditor() {
            // Init
            hFolder = LoadTex(ICON_16);
            oldEditorColor = GUI.color;

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static Texture2D LoadTex(string path) {
            return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
            var folder = (EditorUtility.InstanceIDToObject(instanceID) as GameObject).GetAsFolder();

            if(folder != null) {
                var parentCount = GetParentCount(folder.transform);
                if(!Selection.Contains(folder.gameObject)) {
                    DrawColoredBackground(selectionRect, folder);
                }
                DrawFolderIcon(selectionRect, folder, parentCount);
            }
        }

        private static void DrawColoredBackground(Rect selectionRect, Folder folder) {
            var color = folder.folderColor;
            color.a = 0.3f;
            SaveGUIColor(color);
            GUI.Box(selectionRect, string.Empty);
            RestoreGUIColor();
        }

        private static void DrawFolderIcon(Rect selectionRect, Folder folder, int parentCount) {
            var color = folder.folderColor;
            color.a = 1f;
            SaveGUIColor(color);

            var folderIconRect = new Rect(selectionRect);

            /*
             * tab size == HIERARCHY_LEVEL * 7
             * Root level object would have tab of 0 * 7 == 0
             * First level object would have tab of 1 * 7 == 7
             * etc...
             */
            folderIconRect.x = (selectionRect.width - 2) + (parentCount * 14);

            folderIconRect.width = 20;

            GUI.Label(folderIconRect, hFolder);
            RestoreGUIColor();
        }

        private static void SaveGUIColor(Color newColor) {
            oldEditorColor = GUI.color;
            GUI.color = newColor;
        }

        private static void RestoreGUIColor() {
            GUI.color = oldEditorColor;
        }

        private static int GetParentCount(Transform transform) {
            var count = 0;
            var pointer = transform;
            while(pointer.parent != null) {
                count++;
                pointer = pointer.parent;
            }
            return count;
        }

        [MenuItem("GameObject/Folder", false, int.MinValue)]
        public static void AddFolder() {
            var selection = Selection.activeTransform;
            var obj = new GameObject("New Folder");
            obj.AddComponent<Folder>();
            obj.transform.parent = selection;
        }
    }
}