using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System;

namespace BeardPhantom.Folders
{
    /// <summary>
    /// A logical structure that holds objects but has no Transform.
    /// Is color coded and makes the hierarchy easier to digest.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Folder : MonoBehaviour
    {
        private static Type[] invalidTypes;

        #region Inspector Exposed Fields
        [SerializeField]
        private ThreeWayDrawMode drawMode = ThreeWayDrawMode.OnSelected;
        [SerializeField]
        private TwoWayDrawMode labelDrawMode = TwoWayDrawMode.OnSelected;
        [SerializeField]
        private Color _folderColor = new Color(1f, 1f, 0f, 1f);
        [SerializeField]
        private bool allowBrokenPath = false;
        #endregion

        #region Properties
        public string path { get; private set; }
        public Color folderColor
        {
            get
            {
                // If there is no root folder, use this folder color
                if (transform.parent == null || transform.parent.GetComponent<Folder>() == null)
                {
                    return _folderColor;
                }
                // otherwise inherit the color from the root
                else
                {
                    return transform.root.GetComponent<Folder>()._folderColor;
                }
            }
        }
        #endregion

        #region Unity Messages
        private void Awake()
        {
            RecalculatePath();
        }

        private void Update()
        {
            EnforceFolderBehaviours();
        }

        // Dont allow disabling folders
        private void OnDisable()
        {
            enabled = true;
        }

        public void OnDrawGizmos()
        {
            if (drawMode == ThreeWayDrawMode.AlwaysShow)
            {
                DrawConnections();
            }
        }

        public void OnDrawGizmosSelected()
        {
            if (drawMode == ThreeWayDrawMode.OnSelected)
            {
                DrawConnections();
            }
        }

        private void OnTransformParentChanged()
        {
            if (!allowBrokenPath && transform.parent != null)
            {
                if (transform.parent.gameObject.GetComponent<Folder>() == null)
                {
                    transform.parent = null;
                }
            }
        }
        #endregion

        [Conditional("UNITY_EDITOR")]
        private void EnforceFolderBehaviours()
        {
            // Will fix broken path if detected
            if (!allowBrokenPath)
            {
                OnTransformParentChanged();
            }
            if (invalidTypes == null)
            {
                invalidTypes = new Type[]{
                    typeof(Rigidbody),
                    typeof(Rigidbody2D),
                    typeof(Collider),
                    typeof(ConstantForce),
                    typeof(ConstantForce2D)
                };
            }

            RecalculatePath();

            // Hide Transform
            transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            transform.position = Vector3.zero;

            var invalidComponents = GetComponents<Component>()
                                    .Where(c => invalidTypes.Contains(c.GetType()))
                                    .ToArray();

            foreach (var c in invalidComponents)
            {
                DestroyImmediate(c);
            }
        }

        /// <summary>
        /// Draws lines from folder to all children
        /// </summary>
        private void DrawConnections()
        {
            var transforms = GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                var oldColor = Gizmos.color;
                Gizmos.color = folderColor;
                Gizmos.DrawLine(transform.position, t.position);
                Gizmos.color = oldColor;
            }
        }

        /// <summary>
        /// Recalculates string based path of all folders
        /// </summary>
        private void RecalculatePath()
        {
            path = string.Empty;
            var sb = new StringBuilder("/");
            var parents = transform.GetParents().Where(t => t.gameObject.GetComponent<Folder>() != null);
            var names = parents.Reverse().Select(s => s.name);
            foreach (var p in names)
            {
                sb.Append(p);
                sb.Append('/');
            }
            sb.Append(name);
            path = sb.ToString();
        }

        public enum ThreeWayDrawMode
        {
            DontShow,
            OnSelected,
            AlwaysShow
        }

        public enum TwoWayDrawMode
        {
            DontShow,
            OnSelected
        }
    }

    public static class FolderUtils
    {
        public static bool IsFolder(this Component c)
        {
            if (c == null)
            {
                return false;
            }
            else
            {
                return c.GetComponent<Folder>() != null;
            }
        }

        public static Folder GetFolder(this Component c)
        {
            if (c == null)
            {
                return null;
            }
            else
            {
                return c.GetComponent<Folder>();
            }
        }

        public static bool IsFolder(this GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                return obj.GetComponent<Folder>() != null;
            }
        }

        public static Folder GetFolder(this GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.GetComponent<Folder>();
            }
        }

        /// <summary>
        /// Gets an array of all parents in ascending order.
        /// </summary>
        /// <param name="transform">The transform to start at.</param>
        /// <returns>An array of parents</returns>
        public static Transform[] GetParents(this Transform transform)
        {
            var list = new List<Transform>();
            var pointer = transform;
            while (pointer.parent != null)
            {
                list.Add(pointer.parent);
                pointer = pointer.parent;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the next transform sibling.
        /// </summary>
        /// <param name="t">The transform to start at.</param>
        /// <returns>The next child or null.</returns>
        public static Transform NextSibling(this Transform t)
        {
            var sceneRoots = t.parent == null ? UnityEngine.Object.FindObjectsOfType<Transform>().Where(tr => tr.parent == null).ToArray() : null;
            var nextIndex = t.GetSiblingIndex() + 1;

            // If there are any objects in the scene and t is not the last transform
            if (sceneRoots != null && nextIndex < sceneRoots.Length)
            {
                return sceneRoots[nextIndex];
            }
            else if (nextIndex < t.parent.childCount)
            {
                return t.parent.GetChild(nextIndex);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the previous transform sibling.
        /// </summary>
        /// <param name="t">The transform to start at.</param>
        /// <returns>The next child or null.</returns>
        public static Transform PreviousSibling(this Transform t)
        {
            var roots = t.parent == null ? UnityEngine.Object.FindObjectsOfType<Transform>().Where(tr => tr.parent == null).ToArray() : null;
            var previous = t.GetSiblingIndex() - 1;

            if (previous < 0)
            {
                return null;
            }
            else if (roots != null)
            {
                return roots[previous];
            }
            else
            {
                return t.parent.GetChild(previous);
            }
        }

    }
}