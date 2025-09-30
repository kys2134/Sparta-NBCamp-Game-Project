#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using Backend.Util.Debug;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetSettingWindow : EditorWindow
    {
        #region CONSTANT FIELD API

        private const float WindowWidth = 280f;
        private const float WindowHeight = 100f;

        private const string SubjectLabel = "C# Script";
        private const string PathLabel = "Path";
        private const string NamespaceLabel = "Namespace";
        private const string ClassLabel = "Class";

        private const string DefaultDataPath = "Assets/";

        #endregion

        private AddressAssetWindow _parent;

        public void OnGUI()
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;

            var style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = EditorStyles.label.fontSize;
            style.normal.textColor = EditorStyles.label.normal.textColor;

            GUI.Label(new Rect(10f, 10f + (lineHeight * 0f) + (2f * 0f), 80f, lineHeight), SubjectLabel, style);
            GUI.Label(new Rect(10f, 10f + (lineHeight * 1f) + (2f * 1f), 80f, lineHeight), PathLabel);
            GUI.Label(new Rect(10f, 10f + (lineHeight * 2f) + (2f * 2f), 80f, lineHeight), NamespaceLabel);
            GUI.Label(new Rect(10f, 10f + (lineHeight * 3f) + (2f * 3f), 80f, lineHeight), ClassLabel);

            _parent.Loader.Cache.Path = GUI.TextField(new Rect(90f, 10f + (lineHeight * 1f) + (2f * 1f), 180f - lineHeight - 2f, lineHeight), _parent.Loader.Cache.Path);
            _parent.Loader.Cache.Namespace = GUI.TextField(new Rect(90f, 10f + (lineHeight * 2f) + (2f * 2f), 180f, lineHeight), _parent.Loader.Cache.Namespace);
            _parent.Loader.Cache.ClassName = GUI.TextField(new Rect(90f, 10f + (lineHeight * 3f) + (2f * 3f), 180f, lineHeight), _parent.Loader.Cache.ClassName);

            if (GUI.Button(new Rect(270f - lineHeight, 10f + (lineHeight * 1f) + (2f * 1f), lineHeight, lineHeight), "..."))
            {
                var path = EditorUtility.OpenFolderPanel("Select Folder Path", Application.dataPath, string.Empty);
                if (string.IsNullOrEmpty(path) == false && path.StartsWith(Application.dataPath))
                {
                    path = path.Substring(Application.dataPath.Length);
                    path = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    path = path.Replace("\\", "/");

                    _parent.Loader.Cache.Path = path;
                }
            }

            style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleCenter;
            if (GUI.Button(new Rect(230f, 10f + (lineHeight * 4f) + (2f * 4f), 40f, lineHeight), "Save", style))
            {
                _parent.Loader.SaveAddressableAssetCacheData();
            }
        }

        private void OnLostFocus()
        {
            _parent.PopUpWindow = null;

            Close();
        }

        public void SetParentWindow(EditorWindow parent)
        {
            Debugger.LogProgress();

            _parent = parent as AddressAssetWindow;
        }

    }
}

#endif
