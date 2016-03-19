using UnityEngine;
using UnityEditor;

namespace Rubycone.Folders {
	/// <summary>
	/// Sorts transforms via note existence.
	/// </summary>
	public class NoteSort : BaseHierarchySort {
		private static TransformSort defaultSort = new TransformSort();

		/// <summary>
		/// The label for the sort box.
		/// </summary>
		public override GUIContent content {
			get {
				return new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(NoteHierarchyEditor.ICON_16_PATH), "Note First Order");
			}
		}

		/// <summary>
		/// Sorts transforms via note existence.
		/// </summary>
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
				return defaultSort.Compare(lhs, rhs);
			}
		}
	}
}