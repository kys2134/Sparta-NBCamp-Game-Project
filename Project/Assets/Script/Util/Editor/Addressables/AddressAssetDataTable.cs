#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetDataTable
    {
        private readonly MultiColumnHeader _header;

        public AddressAssetDataTable()
        {
            var columns = new MultiColumnHeaderState.Column[3];
            columns[0] = new MultiColumnHeaderState.Column
                         {
                             allowToggleVisibility = true,
                             autoResize = false,
                             canSort = false,
                             sortingArrowAlignment = TextAlignment.Right,
                             headerContent =
                                 new GUIContent(EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow")),
                             headerTextAlignment = TextAlignment.Center,
                             width = 24f
                         };
            columns[1] = new MultiColumnHeaderState.Column
                         {
                             allowToggleVisibility = true,
                             autoResize = false,
                             canSort = true,
                             sortingArrowAlignment = TextAlignment.Right,
                             headerContent = new GUIContent("Address"),
                             headerTextAlignment = TextAlignment.Left,
                             width = 128f
                         };
            columns[2] = new MultiColumnHeaderState.Column
                         {
                             allowToggleVisibility = true,
                             autoResize = false,
                             canSort = false,
                             sortingArrowAlignment = TextAlignment.Center,
                             headerContent = new GUIContent("Name"),
                             headerTextAlignment = TextAlignment.Left,
                             width = 128f
                         };

            _header = new MultiColumnHeader(new MultiColumnHeaderState(columns));
            _header.visibleColumnsChanged += header => header.ResizeToFit();

            // Initial resizing of the content.
            _header.ResizeToFit();

            TreeView = new AddressAssetTreeView(new TreeViewState(), _header);
        }

        public AddressAssetTreeView TreeView { get; }
    }
}

#endif
