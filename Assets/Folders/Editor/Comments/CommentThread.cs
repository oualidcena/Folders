using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Folders
{
    [Serializable]
    public class CommentThread
    {
        public string threadTitle = "New Comment Thread";
        public string scene;
        public Vector3 position;
        public Vector3 normal;
        public Color color = Color.red;

        public List<Comment> thread = new List<Comment>();

        public void AddComment(string text)
        {
            thread.Add(new Comment()
            {
                user = Environment.UserName,
                commentText = text,
                date = DateTime.UtcNow.ToBinary()
            });
            thread.Sort();
        }
        internal void SetPositionViaRaycast()
        {
            var sceneCamera = SceneView.lastActiveSceneView.camera;
            var ray = sceneCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                position = hit.point;
                normal = hit.normal;
            }
            else
            {
                position = ray.GetPoint(1f);
                normal = Vector3.up;
            }
            SceneView.RepaintAll();
        }
        internal void FocusView()
        {
            if (SceneView.lastActiveSceneView)
            {
                SceneView.lastActiveSceneView.LookAt(position);
            }
        }
    }
}