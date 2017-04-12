using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace BeardPhantom.Folders
{
    public class MoveToFolderWindow : EditorWindow
    {
        Vector2 scroll = Vector2.zero;
        GameObject[] selected;
        Folder[] availableFolders;
        string[] availableFolderNames;
        int index = 0;

        [MenuItem("GameObject/Move To Folder", false, int.MinValue + 1)]
        public static void MoveTo()
        {
            var window = EditorWindow.GetWindow<MoveToFolderWindow>();
            window.title = "Move To Folder...";
            window.ShowUtility();
        }

        [MenuItem("GameObject/Move To Folder", true)]
        public static bool ValidateMoveTo()
        {
            return Selection.gameObjects.Length > 0;
        }

        void OnEnable()
        {
            selected = Selection.gameObjects;
            availableFolders = FindObjectsOfType<Folder>().Where(f => !ArrayUtility.Contains(selected, f.gameObject)).ToArray();
            availableFolderNames = availableFolders.Select(f => f.name).ToArray();
        }

        void OnGUI()
        {
            if (availableFolders.Length == 0)
            {
                EditorGUILayout.LabelField("NO FOLDERS IN SCENE");
            }
            else
            {
                index = EditorGUILayout.Popup(index, availableFolderNames);
                if (GUILayout.Button("Move"))
                {
                    MoveItems();
                }
                EditorGUILayout.LabelField("Selected:", EditorStyles.boldLabel);
                scroll = EditorGUILayout.BeginScrollView(scroll);
                foreach (var s in selected)
                {
                    EditorGUILayout.LabelField(s.name);
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void MoveItems()
        {
            foreach (var s in selected)
            {
                s.transform.parent = availableFolders[index].transform;
            }
            Close();
        }
    }
}