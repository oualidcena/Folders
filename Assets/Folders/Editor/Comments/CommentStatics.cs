using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Folders
{
    public static class CommentStatics
    {
        private const string COMMENT_REGISTRY_ASSET = "CommentRegistry.asset";
        private static CommentRegistry _registry;

        public static CommentRegistry registry
        {
            get
            {
                if (!_registry)
                {
                    _registry = EditorGUIUtility.Load(COMMENT_REGISTRY_ASSET) as CommentRegistry;
                    if (!_registry)
                    {
                        _registry = ScriptableObject.CreateInstance<CommentRegistry>();
                        var path = Path.Combine("Assets", "Editor Default Resources");
                        Directory.CreateDirectory(path);
                        path = Path.Combine(path, COMMENT_REGISTRY_ASSET);
                        AssetDatabase.CreateAsset(_registry, path);
                        AssetDatabase.SaveAssets();
                    }
                }
                return _registry;
            }
        }

        public static CommentThread NewThread()
        {
            var scene = SceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(scene))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                scene = SceneManager.GetActiveScene().path;
                if (string.IsNullOrEmpty(scene))
                {
                    return null;
                }
            }
            var thread = new CommentThread();
            thread.scene = scene;
            thread.SetPositionViaRaycast();
            registry.threads.Add(thread);
            AssetDatabase.SaveAssets();
            return thread;
        }
    }
}