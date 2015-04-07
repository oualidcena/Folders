using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Rubycone.Folders {
    public class FolderSort : BaseHierarchySort {

        TransformSort defSort;
        const string ICON_32 = "Assets/Rubycone/Folders/Resources/folder_icon_32.png";

        public override GUIContent content {
            get {
                return new GUIContent(Resources.LoadAssetAtPath<Texture2D>(ICON_32), "Folder First Order");
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