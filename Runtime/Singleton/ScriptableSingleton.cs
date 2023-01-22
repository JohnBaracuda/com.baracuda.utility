using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Utilities
{
    public abstract class ScriptableSingleton<T> : ScriptableObject, ISerializationCallbackReceiver where T : ScriptableSingleton<T>
    {
        public static T Singleton
        {
            get
            {
                if (initialized)
                {
                    return singleton;
                }
                //singleton = Resources.Load<T>(string.Empty);
#if UNITY_EDITOR
                // Find existing instance
                if (singleton == null)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
                    for (var i = 0; i < guids.Length; i++)
                    {
                        var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                        if (asset == null)
                        {
                            continue;
                        }

                        singleton = asset;
                        Debug.Log($"Found {singleton.name} at {assetPath}");
                        break;
                    }
                }

                // Create new instance
                if (singleton == null)
                {
                    var resourceAttribute = typeof(T).GetCustomAttribute<DefaultResourceAttribute>();

                    var path = resourceAttribute is not null
                        ? resourceAttribute.Path
                        : "Assets/Resources";

                    var fileName = resourceAttribute?.FileName ?? "Assets/Resources";

                    EnsureFolderPathExists(path);

                    var instance = CreateInstance<T>();
                    UnityEditor.AssetDatabase.CreateAsset(instance, $"{path}/{fileName}.asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    Singleton = instance;
                }
#endif
                Assert.IsNotNull(singleton);
                return singleton;
            }
            private set
            {
                Assert.IsNotNull(value);
                singleton = value;
                initialized = true;
                singleton.OnInitialized();
            }
        }

        private static T singleton;
        private static bool initialized;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Assert.IsTrue(singleton == this || singleton == null);
            if (singleton == null && singleton != this)
            {
                Singleton = (T) this;
            }
        }

        /// <summary>
        /// Called on the object when it is initialized as a Singleton
        /// </summary>
        protected abstract void OnInitialized();


        #region Editor Utility

#if UNITY_EDITOR

        private static void EnsureFolderPathExists(string path)
        {
            Debug.Log(LogCategory.Asset, $"Ensure Folder Path Exists: {path}");
            var folders = path.Split('/').ToList();

            if (folders.FirstOrDefault()?.Equals("Assets", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                folders.RemoveAt(0);
            }

            if (!folders.Contains("Resources", StringComparer.OrdinalIgnoreCase))
            {
                folders.Add("Resources");
            }

            var currentFolderPath = "Assets";
            foreach (var folder in folders)
            {
                var nextFolderPath = Path.Combine(currentFolderPath, folder);
                if (!UnityEditor.AssetDatabase.IsValidFolder(nextFolderPath))
                {
                    Debug.Log(LogCategory.Asset, $"Creating folder: {folder} in {currentFolderPath}");
                    UnityEditor.AssetDatabase.CreateFolder(currentFolderPath, folder);
                }
                currentFolderPath = nextFolderPath;
            }
        }

#endif

        #endregion
    }
}
