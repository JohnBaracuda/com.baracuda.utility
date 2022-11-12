using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Utilities.Singleton
{
    /// <summary>
    /// Base type for developer specific configuration files.
    /// Use the <see cref="DefaultResourcePathAttribute"/> attribute to declare a default resources path for subtypes of
    /// this class in a build.
    /// </summary>
    public abstract class LocalConfiguration<T> : ScriptableObject where T : LocalConfiguration<T>
    {
        private static T local;

        public static T Local
        {
            get
            {
                if (local)
                {
                    return local;
                }

#if UNITY_EDITOR
                var guid = UnityEditor.EditorPrefs.GetString(typeof(T).FullName, null);
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    if (loaded)
                    {
                        Local = loaded;
                        return local;
                    }

                    UnityEngine.Debug.Log($"Unable to load asset {guid} at path {path}");
                }

                // Try loading at path
                var assetPath = $"Assets/Configurations/Developer/{typeof(T).Name}.{System.Environment.UserName.Trim()}.asset";
                var loadedDefault = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (loadedDefault)
                {
                    Local = loadedDefault;
                    return local;
                }

                UnityEngine.Debug.Log($"Creating new local developer config at {assetPath}");
                var asset = CreateInstance<T>();
                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Configurations/Developer"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Configurations");
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Configurations", "Developer");
                }

                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
                Local = asset;
                return local;
#else // !UNITY_EDITOR
                if (System.Attribute.GetCustomAttribute(typeof(T), typeof(DefaultResourcePathAttribute), true) is DefaultResourcePathAttribute attribute)
                {
                    local = Resources.Load<T>(attribute.Path);
                }
                if (local == null)
                {
                    local = CreateInstance<T>();
                }

            return local;
#endif // UNITY_EDITOR
            }
            private set
            {
                local = value;
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(local);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString(typeof(T).FullName, guid);
#endif
            }
        }

#if UNITY_EDITOR

        private bool IsLocalConfig()
        {
            return this == Local;
        }

        [Button]
        [ConditionalShow(nameof(IsLocalConfig), false)]
        public void SetAsLocalConfig()
        {
            Local = (T) this;
        }
#endif // UNITY_EDITOR
    }
}