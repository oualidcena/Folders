using UnityEngine;
using System.Collections;

namespace BeardPhantom.Folders
{
    /// <summary>
    /// A simple note that shows up visually in the Hierarchy.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class Note : MonoBehaviour
    {
        /// <summary>
        /// Unused: forces Unity to not build unless note is "resolved"
        /// </summary>
        [SerializeField]
        private bool _resolveBeforeBuild;
        /// <summary>
        /// Text of the note.
        /// </summary>
        [SerializeField]
        private string _text = string.Empty;

        public bool resolveBeforeBuild { get { return _resolveBeforeBuild; } }
        public string text { get { return _text; } }

        private void Update()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }

        private void OnDisable()
        {
            // Disallow note disabling
            enabled = true;
        }
    }

    public static class NoteUtils
    {
        public static Note GetAsNote(this Component c)
        {
            if (c == null)
            {
                return null;
            }
            else
            {
                return c.GetComponent<Note>();
            }
        }

        public static Note GetAsNote(this GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.GetComponent<Note>();
            }
        }
    }
}