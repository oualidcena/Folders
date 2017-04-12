using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Folders
{
    public partial class CommentWindow : EditorWindow
    {
        private Scene currentScene;
        private CommentThread selectedThread;
        private bool isShowingSideBar = true;
        private Bounds frustumTestBounds = new Bounds(Vector3.zero, Vector3.one * 3f);
        private bool isShowing = true;
        private bool isShowingPreview = true;
        private Vector2 threadScrollPosition;
        private Vector2 newCommentScrollPosition;
        private string text = string.Empty;

        [MenuItem("Window/Comments")]
        private static void ShowCommentsWindow()
        {
            var instance = GetWindow<CommentWindow>();
            instance.titleContent = new GUIContent("Comments");
            instance.RegisterWithScene();
        }

        private void RegisterWithScene()
        {
            RemoveCallbacks();
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            SceneView.RepaintAll();
            Repaint();
        }
        private void RemoveCallbacks()
        {
            if (SceneView.onSceneGUIDelegate == null)
            {
                return;
            }
            var callbacks = SceneView.onSceneGUIDelegate.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                var callback = callbacks[i] as SceneView.OnSceneFunc;
                if (callback == null || callback.Target.GetType() == GetType())
                {
                    SceneView.onSceneGUIDelegate -= callback;
                }
            }
        }
        private void DeregisterFromScene()
        {
            RemoveCallbacks();
            SceneView.RepaintAll();
        }
        private void OnProjectChange()
        {
            RegisterWithScene();
        }
        private void OnDestroy()
        {
            DeregisterFromScene();
        }
        private void OnSceneGUI(SceneView view)
        {
            if (!isShowing)
            {
                return;
            }
            foreach (var t in CommentStatics.registry.threads)
            {
                frustumTestBounds.center = t.position;
                if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(SceneView.lastActiveSceneView.camera), frustumTestBounds))
                {
                    Handles.color = t.color;
                    Handles.ArrowHandleCap(0, t.position, Quaternion.LookRotation(t.normal), 1f, EventType.Repaint);
                    Handles.Label(t.position + t.normal, t.threadTitle, EditorStyles.helpBox);
                }
            }

            if (isShowingPreview)
            {
                var ray = view.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                var point = ray.GetPoint(1f);
                var rotation = Quaternion.LookRotation(Vector3.up);
                var size = 0.25f;
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10f))
                {
                    rotation = Quaternion.LookRotation(hit.normal);
                    point = hit.point;
                    size = 1f;
                }
                Handles.color = new Color(1f, 1f, 1f, 0.5f);
                Handles.ArrowHandleCap(0, point, rotation, size, EventType.Repaint);
            }
        }
        private void OnGUI()
        {
            var color = GUI.color;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.Separator();
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(isShowingSideBar ? 100f : 12f)))
                {
                    isShowingSideBar = EditorGUILayout.ToggleLeft("SHOW SIDEBAR", isShowingSideBar);
                    if (isShowingSideBar)
                    {
                        DrawSideBar();
                    }
                }
                if (isShowingSideBar)
                {
                    GUILayout.Box(EditorGUIUtility.whiteTexture, GUILayout.ExpandHeight(true), GUILayout.Width(1f));
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    if (DrawSelectedThread())
                    {
                        CommentStatics.registry.threads.Remove(selectedThread);
                        selectedThread = null;
                        SceneView.RepaintAll();
                    }
                }
            }
            GUI.color = color;
            EditorGUIUtility.labelWidth = labelWidth;
        }
        private void DrawSideBar()
        {
            EditorGUI.BeginChangeCheck();
            var color = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("Create New Thread"))
            {
                var newThread = CommentStatics.NewThread();
                selectedThread = newThread == null ? selectedThread : newThread;
                selectedThread.FocusView();
            }
            GUI.color = color;
            isShowing = EditorGUILayout.Toggle("Show Comments", isShowing);
            isShowingPreview = EditorGUILayout.Toggle("Show Preview", isShowingPreview);
            foreach (var t in CommentStatics.registry.threads)
            {
                if (GUILayout.Button(t.threadTitle))
                {
                    selectedThread = t;
                    selectedThread.FocusView();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }
        private bool DrawSelectedThread()
        {
            if (selectedThread == null)
            {
                return false;
            }
            EditorGUIUtility.labelWidth = 60f;
            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            selectedThread.threadTitle = EditorGUILayout.TextField("Thread: ", selectedThread.threadTitle);
            selectedThread.color = EditorGUILayout.ColorField("Color: ", selectedThread.color);
            var color = GUI.color;
            GUI.color = Color.red;
            if (GUILayout.Button("Delete"))
            {
                return true;
            }
            GUI.color = color;
            if (GUILayout.Button("Raycast New Position"))
            {
                selectedThread.SetPositionViaRaycast();
                selectedThread.FocusView();
            }
            var width = EditorGUIUtility.currentViewWidth - 40f;
            using (var scroll = new EditorGUILayout.ScrollViewScope(threadScrollPosition))
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(width)))
                {
                    foreach (var c in selectedThread.thread)
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.LabelField("User: ", c.user, EditorStyles.boldLabel);
                            EditorGUILayout.LabelField("Date: ", DateTime.FromBinary(c.date).ToLocalTime().ToString(), EditorStyles.boldLabel);
                            EditorGUILayout.SelectableLabel(c.commentText, GUILayout.Height(EditorStyles.label.CalcHeight(new GUIContent(c.commentText), width)));
                        }
                        EditorGUILayout.Separator();
                    }
                }
                threadScrollPosition = scroll.scrollPosition;
            }
            var textAreaWW = new GUIStyle(EditorStyles.textArea);
            textAreaWW.wordWrap = true;
            using (new EditorGUILayout.VerticalScope())
            {
                {
                    const int LIMIT = 1000;
                    if (text.Length > LIMIT)
                    {
                        text = text.Substring(0, LIMIT);
                        EditorGUIUtility.editingTextField = false;
                    }
                    EditorGUILayout.LabelField(string.Format("{0}/{1}", text.Length, LIMIT));
                    text = EditorGUILayout.TextArea(text, textAreaWW, GUILayout.Height(50f));

                    if (GUILayout.Button("Add Comment"))
                    {
                        selectedThread.AddComment(text);
                        text = string.Empty;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
            return false;
        }
    }
}