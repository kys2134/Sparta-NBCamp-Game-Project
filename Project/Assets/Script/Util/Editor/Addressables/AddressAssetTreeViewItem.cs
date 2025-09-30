#if UNITY_EDITOR

using UnityEditor.IMGUI.Controls;

namespace Backend.Util.Editor.Addressables
{
    public class AddressAssetTreeViewItem : TreeViewItem
    {
        public string Address { get; set; }

        public string Name { get; set; }
    }
}

#endif
