#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;
using Backend.Util.Debug;

namespace Backend.Util.Editor.Addressables
{
    public class AddressLoader
    {
        public void Update()
        {
            Addresses.Clear();
            Groups.Clear();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings is null)
            {
                Debugger.LogError("Addressable settings is not existed.");

                return;
            }

            var entries = settings.groups.Where(group => IsDefaultAsset(group.Name)).SelectMany(group => group.entries);
            foreach (var entry in entries)
            {
                Addresses.Add(entry.address);

                foreach (var label in entry.labels)
                {
                    if (Groups.TryGetValue(label, out var list) == false)
                    {
                        list = new List<string>();

                        Groups[label] = list;
                    }
                    list.Add(entry.address);
                }
            }
        }

        private bool IsDefaultAsset(string name)
        {
            return name is "Default Local Group" or "EditorSceneList" or "Resources";
        }

        public List<string> Addresses { get; } = new ();

        public Dictionary<string, List<string>> Groups { get; } = new ();
    }
}

#endif
