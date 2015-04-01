using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.BoltAction {
    public class NoteSort : BaseHierarchySort {

        public override GUIContent content {
            get {
                return new GUIContent(AssetDatabase.LoadAssetAtPath(NoteHierarchyEditor.GREY_ICON_16, typeof(Texture2D)) as Texture2D, "Note First Order");
            }
        }

        public override int Compare(GameObject lhs, GameObject rhs) {
            if(lhs == rhs) {
                return 0;
            }
            else if(lhs.GetAsNote() != null && rhs.GetAsNote() == null) {
                return -1;
            }
            else if(lhs.GetAsNote() == null && rhs.GetAsNote() != null) {
                return 1;
            }
            else {
                return lhs.transform.GetSiblingIndex() < rhs.transform.GetSiblingIndex() ? 1 : -1;
            }
        }
    }
}