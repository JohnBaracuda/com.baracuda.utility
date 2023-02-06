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

        public static T Local
        {
            get
            {
#if !UNITY_EDITOR
                return Global;
#else
                if (local == null)
                {
                    //Debug.Log("Singleton", $"Loading local {typeof(T).Name} from AssetDatabase");
                    var guid = UnityEditor.EditorPrefs.GetString(typeof(T).FullName);
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

                    if (asset != null)
                    {
                        Local = asset;
                    }
                }

                if (local == null)
                {
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "Settings");
                    }

                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Settings/Developer"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets/Settings", "Developer");
                    }

                    var asset = CreateInstance<T>();
                    var assetPath = $"Assets/Settings/Developer/{typeof(T).Name}.{System.Environment.UserName.Trim()}.asset";
                    UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    Debug.Log("Singleton", $"Creating new {typeof(T).Name} instance at {assetPath}!");
                    Local = asset;
                }

                if (local == null)
                {
                    Debug.Log("Set Global");
                    Local = Global;
                }

                return local;
#endif
            }
            private set
            {
#if UNITY_EDITOR
                if (value == null)
                {
                    return;
                }
                var path = UnityEditor.AssetDatabase.GetAssetPath(value);
                var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                UnityEditor.EditorPrefs.SetString(typeof(T).FullName, guid);
                if (SingletonRegistry.Resolve<T>() == null)
                {
                    Global = value;
                }
#endif
                local = value;
            }
        }

        public static T Global
        {
            get => SingletonRegistry.Resolve<T>();
            private set => SingletonRegistry.Register(value);
        }
        private void OnEnable()
        {
            if (SingletonRegistry.Exists<T>())
            {
                return;
            }

            Debug.Log("Singleton", $"No instance of type {typeof(T).FullName} found! Settings this as global instance!", this);
            Global = (T) this;
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