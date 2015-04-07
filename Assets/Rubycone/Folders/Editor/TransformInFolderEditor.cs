using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.Folders {
    [CustomEditor(typeof(Transform))]
    public class TransformInFolderEditor : Editor {
        Transform targetObj;

        void OnEnable() {
            targetObj = target as Transform;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            try {
                var root = targetObj.root.gameObject.GetAsFolder();
                if(targetObj.parent != null) {
                    var parent = targetObj.parent.gameObject.GetAsFolder();
                    if(parent != null && GUILayout.Button("Select Parent Folder")) {
                        Selection.activeObject = parent;
                        EditorGUIUtility.PingObject(parent);
                    }
                }
                if(root != null) {
                    if(GUILayout.Button("Select Root Folder")) {
                        Selection.activeObject = root;
                        EditorGUIUtility.PingObject(root);
                    }
                }
            }
            catch(System.Exception) { }
        }
    }
}