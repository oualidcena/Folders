using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Rubycone.Folders {
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Folder : MonoBehaviour {
        public enum ThreeWayDrawMode { DontShow, OnSelected, AlwaysShow }
        public enum TwoWayDrawMode { DontShow, OnSelected }

        [SerializeField]
        ThreeWayDrawMode _drawMode       = ThreeWayDrawMode.OnSelected,
                         _folderDrawMode = ThreeWayDrawMode.OnSelected;
        [SerializeField]
        TwoWayDrawMode _labelDrawMode= TwoWayDrawMode.OnSelected;
        [SerializeField]
        Color _folderColor = new Color(1f, 1f, 0f, 1f);

        public string path { get; private set; }

        Transform oldParent = null;


        public Color folderColor {
            get {
                if(transform.root == transform) {
                    return _folderColor;
                }
                else {
                    return transform.root.gameObject.GetAsFolder().folderColor;
                }
            }
        }



        void Awake() {
            RecalculatePath();
        }

        void Update() {
            transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            transform.position = Vector3.zero;

            var allComponents = GetComponents<Component>()
                                .Where(c => c != this && c != transform && c.GetType() != typeof(Note))
                                .ToArray();

            if(allComponents.Length != 0) {
                foreach(var c in allComponents) {
                    DestroyImmediate(c);
                }
            }

        }

        void OnDisable() {
            enabled = true;
        }

        public void OnDrawGizmos() {
            if(_folderDrawMode == ThreeWayDrawMode.AlwaysShow) {
                DrawFolderIcon();
            }
            if(_drawMode == ThreeWayDrawMode.AlwaysShow) {
                DrawConnections();
            }
        }

        public void OnDrawGizmosSelected() {
            if(_folderDrawMode == ThreeWayDrawMode.OnSelected) {
                DrawFolderIcon();
            }
            if(_drawMode == ThreeWayDrawMode.OnSelected) {
                DrawConnections();
            }
        }

        void DrawFolderIcon() {
            Gizmos.DrawIcon(transform.position, "folder_icon_full", true);
        }

        void DrawConnections() {
            var transforms = GetComponentsInChildren<Transform>();
            foreach(var t in transforms) {
                var oldColor = Gizmos.color;
                Gizmos.color = folderColor;
                Gizmos.DrawLine(transform.position, t.position);
                Gizmos.color = oldColor;
            }
        }

        void OnTransformParentChanged() {
            if(transform.parent == oldParent) {
                return;
            }
            RecalculatePath();
            if(transform.parent != null) {
                if(transform.parent.gameObject.GetAsFolder() == null) {
                    transform.parent = null;
                }
                else {
                    transform.SetAsFirstSibling();
                }
            }

            oldParent = transform.parent;
        }

        private void RecalculatePath() {
            path = string.Empty;
            var sb = new StringBuilder("/");
            var parents = transform.GetParents();
            var names = parents.Reverse().Select(s => s.name).ToArray();
            foreach(var p in names) {
                sb.Append(p + '/');
            }
            sb.Append(name);
            path = sb.ToString();
        }
    }

    public static class FolderUtils {
        public static Transform[] GetParents(this Transform transform) {
            var list = new List<Transform>();
            var pointer = transform;
            while(pointer.parent != null) {
                list.Add(pointer.parent);
                pointer = pointer.parent;
            }
            return list.ToArray();
        }

        public static Folder GetAsFolder(this GameObject g) {
            if(g == null) {
                return null;
            }
            var folder = g.GetComponent<Folder>();
            return folder;
        }

        public static bool IsFolder(this GameObject g) {
            if(g == null) {
                return false;
            }
            else {
                return g.GetComponent<Folder>();
            }
        }

        public static Transform NextSibling(this Transform t) {
            var roots = t.parent == null ? GameObject.FindObjectsOfType<Transform>().Where(tr => tr.parent == null).ToArray() : null;
            var next = t.GetSiblingIndex() + 1;

            if(roots != null && next < roots.Length) {
                return roots[next];
            }
            else if(next < t.parent.childCount) {
                return t.parent.GetChild(next);
            }
            else {
                return null;
            }
        }

        public static Transform PreviousSibling(this Transform t) {
            var roots = t.parent == null ? GameObject.FindObjectsOfType<Transform>().Where(tr => tr.parent == null).ToArray() : null;
            var previous = t.GetSiblingIndex() - 1;

            if(previous < 0) {
                return null;
            }
            else if(roots != null) {
                return roots[previous];
            }
            else {
                return t.parent.GetChild(previous);
            }
        }

    }
}