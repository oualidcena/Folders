using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Folders
{
    [Serializable]
    public class CommentRegistry : ScriptableObject
    {
        [HideInInspector]
        public List<CommentThread> threads = new List<CommentThread>();
    }
}