using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using Rubycone.Shared;

namespace Rubycone.Folders {
    [InitializeOnLoad]
    public static class FolderHierarchyEditor {

        static Texture2D hFolder16;
        static Color highlightColor = new Color(1f, 1f, 0f, 0.2f);

        const string ICON_16 = "Assets/Rubycone/Folders/Resources/folder_icon_16.png";


        static FolderHierarchyEditor() {
            // Init
            hFolder16 = Resources.LoadAssetAtPath<Texture2D>(ICON_16);

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
            var folder = (EditorUtility.InstanceIDToObject(instanceID) as GameObject).GetAsFolder();

            if(folder != null) {
                var parentCount = GetParentCount(folder.transform);
                if(!Selection.Contains(instanceID)) {
                    DrawColoredBackground(selectionRect, folder);
                }
                DrawFolderIcon(selectionRect, folder, parentCount);
            }
        }

        private static void DrawColoredBackground(Rect selectionRect, Folder folder) {
            var color = folder.folderColor;
            color.a = 0.3f;
            EditorGUIHelper.SaveGUIColor(color);
            GUI.Box(selectionRect, string.Empty);
            EditorGUIHelper.RestoreGUIColor();
        }

        private static void DrawFolderIcon(Rect selectionRect, Folder folder, int parentCount) {
            var color = folder.folderColor;
            color.a = 1f;
            EditorGUIHelper.SaveGUIColor(color);

            var folderIconRect = new Rect(selectionRect);

            /*
             * tab size == HIERARCHY_LEVEL * 7
             * Root level object would have tab of 0 * 7 == 0
             * First level object would have tab of 1 * 7 == 7
             * etc...
             */
            folderIconRect.x = (selectionRect.width - 2) + (parentCount * 14);

            folderIconRect.width = 20;

            GUI.Label(folderIconRect, hFolder16);
            EditorGUIHelper.RestoreGUIColor();
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

        [MenuItem("GameObject/Create Folder", false, int.MinValue)]
        public static void AddFolder(MenuCommand cmd) {

            var selection = Selection.activeTransform;
            var obj = new GameObject("New Folder");
            obj.AddComponent<Folder>();

            GameObjectUtility.SetParentAndAlign(obj, cmd.context as GameObject);
            Undo.RegisterCreatedObjectUndo(obj, "Created " + obj.name);
            Selection.activeGameObject = obj;
        }
    }
}