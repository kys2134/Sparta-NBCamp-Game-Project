#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Backend.Util.Debug;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetWindow : EditorWindow
    {
        private Vector2 _scroll;

        private AddressAssetDataTable _table;

        public AddressAssetSettingWindow PopUpWindow;

        private void OnEnable()
        {
            Debugger.LogProgress();

            _table = new AddressAssetDataTable();

            Loader = new AddressAssetDataLoader();
            Loader.LoadAddressableAssetCacheData(_table.TreeView);
        }

        [MenuItem("Window/Addressable/Address Viewer")]
        public static void ShowWindow()
        {
            Debugger.LogProgress();

            var window = GetWindow<AddressAssetWindow>("Address Viewer");
            window.minSize = new Vector2(400, 300);
        }

        public void OnGUI()
        {
            DrawColumnHeader();
        }

        private void DrawColumnHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Update", EditorStyles.toolbarButton, GUILayout.Width(54f)))
            {
                Loader.UpdateAddressableAssetData(_table.TreeView);
            }

            if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                var rect = GUILayoutUtility.GetLastRect();
                var screenPosition = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y + rect.height));

                var height = 20f + (EditorGUIUtility.singleLineHeight * 5f) + (2f * 4f);

                if (PopUpWindow is null)
                {
                    PopUpWindow = CreateInstance<AddressAssetSettingWindow>();
                    PopUpWindow.position = new Rect(screenPosition.x + 54f + 0.6f, screenPosition.y + EditorGUIUtility.singleLineHeight + 3f, 280f, height);
                    PopUpWindow.SetParentWindow(this);
                    PopUpWindow.ShowPopup();
                }
                else
                {
                    PopUpWindow.Close();
                    PopUpWindow = null;
                }
            }

            GUILayout.Label(" ");

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            // Get automatically aligned rect for our multi-column header component.
            var windowRect = GUILayoutUtility.GetLastRect();
            windowRect.width = position.width;
            windowRect.height = position.height;

            _table.TreeView.OnGUI(new Rect(0, EditorGUIUtility.singleLineHeight, position.width, 10000));
        }

        public AddressAssetDataLoader Loader { get; private set; }
    }
}

#endif
