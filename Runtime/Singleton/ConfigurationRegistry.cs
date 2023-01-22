using Baracuda.Utilities.Collections;
using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Utilities.Singleton
{
    [DefaultResource("Assets/Settings/Developer/Resources", FileName = "Configuration Registry")]
    internal sealed class ConfigurationRegistry : ScriptableSingleton<ConfigurationRegistry>
    {
        [SerializeField] private Map<string, ScriptableObject> registry;

        internal void SetGlobal<T>(T configuration) where T : LocalConfiguration<T>
        {
            Validate();
            var key = typeof(T).FullName!;
            if (registry.ContainsKey(key))
            {
                registry.Remove(key);
            }
            registry.Add(typeof(T).FullName!, configuration);
        }

        internal T GetGlobal<T>() where T : LocalConfiguration<T>
        {
            Validate();
            return registry.TryGetValue(typeof(T).FullName!, out var value) && value is T global
                ? global
                : null;
        }

        [Button]
        private void Validate()
        {
            for (var index = registry.Count - 1; index >= 0; index--)
            {
                if (registry.TryGetElementAtIndex(index, out var element) && element.Value == null)
                {
                    registry.RemoveElementAtIndex(index);
                }
            }
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Game/Developer-Configuration/Registry")]
        private static void SelectConfigurationRegistry()
        {
            UnityEditor.Selection.activeObject = Singleton;
        }

        [UnityEditor.MenuItem("Game/Developer-Configuration/Global")]
        private static void SelectGlobalConfiguration()
        {
        }

#endif

        /// <summary>
        /// Called on the object when it is initialized as a Singleton
        /// </summary>
        protected sealed override void OnInitialized()
        {
        }
    }
}