using UnityEngine;
using UnityEditor;

namespace Rubycone.Folders {
	/// <summary>
	/// Sorts transforms via Folder existence.
	/// </summary>
	public class FolderSort : BaseHierarchySort {
		private static TransformSort defaultSort = new TransformSort();

		/// <summary>
		/// The label for the sort box.
		/// </summary>
		public override GUIContent content {
			get {
				return new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(FolderHierarchyEditor.ICON_16), "Folder First Order");
			}
		}

		/// <summary>
		/// Sorts objects based on Folder existence.
		/// </summary>
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
				return defaultSort.Compare(lhs, rhs);
			}
		}
	}
}