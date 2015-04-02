using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.Folders {
    public class FolderSort : BaseHierarchySort {

        TransformSort defSort;

        public override GUIContent content {
            get {
                return new GUIContent(AssetDatabase.LoadAssetAtPath(FolderHierarchyEditor.ICON_16, typeof(Texture2D)) as Texture2D, "Folder First Order");
            }
        }

        public override int Compare(GameObject lhs, GameObject rhs) {
            if(lhs == rhs) {
                return 0;
            }
            else if(lhs.IsFolder() && !rhs.IsFolder()) {
                return -1;
            }
            else if(!lhs.IsFolder() && rhs.IsFolder()) {
                return 1;
            }
            else {
                if(defSort == null) {
                    defSort = new TransformSort();
                }
                return defSort.Compare(lhs, rhs);
            }
        }
    }
}