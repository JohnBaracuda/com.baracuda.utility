using Baracuda.Utilities.Inspector;
using UnityEngine;

namespace Baracuda.Utilities.Singleton
{
    /// <summary>
    /// Base type for developer specific configuration files.
    /// </summary>
    public abstract class LocalConfiguration<T> : ScriptableObject where T : LocalConfiguration<T>
    {
        private static T local;
        private static T global;

        /// <summary>
        /// The local configuration for the individual developer.
        /// </summary>
        public static T Local
        {
            get
            {
                if (local)
                {
                    return local;
                }

#if !UNITY_EDITOR
                if (local == null)
                {
                    Local = Global;
                }
                if (local == null)
                {
                    Local = CreateInstance<T>();
                }

                return local;

#else // UNITY_EDITOR

                var guid = UnityEditor.EditorPrefs.GetString($"{typeof(T).FullName}.{nameof(Local)}", null);
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
                var assetPath =
                    $"Assets/Settings/Developer/{typeof(T).Name}.{System.Environment.UserName.Trim()}.asset";
                var loadedDefault = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (loadedDefault)
                {
                    Local = loadedDefault;
                    return local;
                }

                // Create new asset
                UnityEngine.Debug.Log($"Creating new local {typeof(T).Name} at {assetPath}");
                var asset = CreateInstance<T>();

                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Settings");
                }

                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings/Developer"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Settings", "Developer");
                }

                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();

                Local = asset;
                return local;

#endif // UNITY_EDITOR
            }
            private set
            {
                local = value;
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(local);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString($"{typeof(T).FullName}.{nameof(Local)}", guid);
#endif
            }
        }

        /// <summary>
        /// The global configuration used in a build.
        /// </summary>
        public static T Global
        {
            get
            {
                if (global)
                {
                    return global;
                }

#if !UNITY_EDITOR
                if (global == null)
                {
                    Global = Registry.GetGlobal<T>();
                }
                if (global == null)
                {
                    Global = CreateInstance<T>();
                }

                return global;
#else // UNITY_EDITOR

                var guid = UnityEditor.EditorPrefs.GetString($"{typeof(T).FullName}.{nameof(Global)}", null);
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var loaded = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    if (loaded)
                    {
                        Global = loaded;
                        return global;
                    }

                    UnityEngine.Debug.Log($"Unable to load asset {guid} at path {path}");
                }

                // Try loading at path
                var assetPath = $"Assets/Settings/Developer/{typeof(T).Name}.{nameof(Global)}.asset";
                var loadedDefault = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (loadedDefault)
                {
                    Global = loadedDefault;
                    return global;
                }

                UnityEngine.Debug.Log($"Creating new global {typeof(T).Name} at {assetPath}");
                var asset = CreateInstance<T>();

                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Settings");
                }

                if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings/Developer"))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets/Settings", "Developer");
                }

                UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();

                Global = asset;
                return global;

#endif // UNITY_EDITOR
            }
            private set
            {
                global = value;
#if UNITY_EDITOR
                var path = UnityEditor.AssetDatabase.GetAssetPath(global);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString($"{typeof(T).FullName}.{nameof(Global)}", guid);
#endif
            }
        }

#if UNITY_EDITOR

        private bool IsLocal()
        {
            return this == Local;
        }

        private bool IsGlobal()
        {
            return this == Global;
        }

        [Button]
        [ConditionalShow(nameof(IsLocal), false)]
        public void DeclareAsLocal()
        {
            Local = (T) this;
        }

        [Button]
        [ConditionalShow(nameof(IsGlobal), false)]
        public void DeclareAsGlobal()
        {
            Global = (T) this;
        }
#endif // UNITY_EDITOR
    }
}