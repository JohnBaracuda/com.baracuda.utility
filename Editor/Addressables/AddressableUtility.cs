using System.Linq;
using System.Reflection;
using Baracuda.Utility.Reflection;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Addressables
{
    public static class AddressableUtility
    {
#if UNITY_EDITOR

        public static void UpdateAllAssets()
        {
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(ScriptableObject)}");
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                var attribute = asset.GetType().GetCustomAttributes<AddressablesGroupAttribute>(true).FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                var group = GetGroup(attribute.GroupName, settings);
                if (group == null)
                {
                    UnityEngine.Debug.Log($"No group: {attribute.GroupName} found!");
                    continue;
                }
                var entry = settings.CreateOrMoveEntry(guid, group);
                var move = UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.ModificationEvent.EntryMoved;
                if (attribute.CreateLabel)
                {
                    entry.SetLabel(attribute.GroupName, true, true);
                }
                settings.SetDirty(move, entry, true);
            }

            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static UnityEditor.AddressableAssets.Settings.AddressableAssetGroup GetGroup(string groupName,
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings settings)
        {
            var group = settings.FindGroup(groupName);
            if (group != null)
            {
                return group;
            }
            return null;
        }

#endif
    }
}