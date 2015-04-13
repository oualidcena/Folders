using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System;

namespace Rubycone.Folders {
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Folder : MonoBehaviour {
        public enum ThreeWayDrawMode { DontShow, OnSelected, AlwaysShow }
        public enum TwoWayDrawMode { DontShow, OnSelected }

        [SerializeField]
        ThreeWayDrawMode drawMode = ThreeWayDrawMode.OnSelected;
        [SerializeField]
        TwoWayDrawMode labelDrawMode = TwoWayDrawMode.OnSelected;
        [SerializeField]
        Color _folderColor = new Color(1f, 1f, 0f, 1f);
        [SerializeField]
        bool allowBrokenPath = false;

        static Type[] invalidTypes;

        public string path { get; private set; }

        public Color folderColor {
            get {
                if(transform.parent == null || transform.parent.gameObject.GetAsFolder() == null) {
                    return _folderColor;
                }
                else {
                    return transform.root.gameObject.GetAsFolder()._folderColor;
                }
            }
        }

        void Awake() {
            RecalculatePath();
        }

        void Update() {
            EnforceFolderBehaviours();
        }

        [Conditional("UNITY_EDITOR")]
        void EnforceFolderBehaviours() {
            if(!allowBrokenPath) {
                OnTransformParentChanged();
            }
            if(invalidTypes == null) {
                invalidTypes = new Type[]{
                    typeof(Rigidbody),
                    typeof(Rigidbody2D),
                    typeof(Collider),
                    typeof(ConstantForce),
                    typeof(ConstantForce2D)
                };
            }

            RecalculatePath();
            transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            transform.position = Vector3.zero;

            var allComponents = GetComponents<Component>()
                                .Where(c => invalidTypes.Contains(c.GetType()))
                                .ToArray();

            foreach(var c in allComponents) {
                DestroyImmediate(c);
            }
        }

        void OnDisable() {
            enabled = true;
        }

        public void OnDrawGizmos() {
            if(drawMode == ThreeWayDrawMode.AlwaysShow) {
                DrawConnections();
            }
        }

        public void OnDrawGizmosSelected() {
            if(drawMode == ThreeWayDrawMode.OnSelected) {
                DrawConnections();
            }
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
            if(!allowBrokenPath && transform.parent != null) {
                if(transform.parent.gameObject.GetAsFolder() == null) {
                    transform.parent = null;
                }
            }
        }

        private void RecalculatePath() {
            path = string.Empty;
            var sb = new StringBuilder("/");
            var parents = transform.GetParents().Where(t => t.gameObject.GetAsFolder() != null);
            var names = parents.Reverse().Select(s => s.name);
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