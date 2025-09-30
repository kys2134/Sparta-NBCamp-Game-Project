#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetTreeView : TreeView
    {
        private readonly List<TreeViewItem> _items = new();

        public AddressAssetTreeView(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, children = new List<TreeViewItem>() };

            var count = _items.Count;
            for (var i = 0; i < count; i++)
            {
                root.AddChild(_items[i]);
            }

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as AddressAssetTreeViewItem;

            var count = args.GetNumVisibleColumns();
            for (var i = 0; i < count; i++)
            {
                var rect = args.GetCellRect(i);

                switch (args.GetColumn(i))
                {
                    case 0:
                        EditorGUI.LabelField(rect, string.Empty);
                        break;
                    case 1:
                        EditorGUI.LabelField(rect, item?.Address);
                        break;
                    case 2:
                        EditorGUI.LabelField(rect, item?.Name);
                        break;
                }
            }
        }

        public void AddItem(TreeViewItem item)
        {
            _items.Add(item);

            Reload();
        }

        public void Clear()
        {
            _items.Clear();

            Reload();
        }
    }
}

#endif
