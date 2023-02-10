using Baracuda.Utilities.Collections;
using UnityEngine;

namespace Baracuda.Utilities
{
    public class SingletonRegistry : ScriptableObject
    {
        #region Data

        [SerializeField] private Map<string, Object> registry;

        #endregion


        #region Public

        public static void Register<T>(T instance) where T : Object
        {
            Singleton.registry.AddOrUpdate(Key<T>(), instance);
        }

        public static T Resolve<T>() where T : Object
        {
            if (Singleton.registry.TryGetValue(Key<T>(), out var value))
            {
                return value as T;
            }
            Debug.LogError("Singleton", $"No singleton instance of type {typeof(T)} registered!");
            return null;
        }

        public static bool Exists<T>()
        {
            if (!Singleton.registry.TryGetValue(Key<T>(), out var value))
            {
                return false;
            }

            if (value != null)
            {
                return true;
            }

            Singleton.registry.Remove(Key<T>());
            return false;
        }

        #endregion


        #region Singleton

        private static SingletonRegistry singleton;

        private static SingletonRegistry Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = Resources.Load<SingletonRegistry>("Singletons");
                }

                if (singleton == null)
                {
                    Create();
                }

                return singleton;
            }
        }

        #endregion


        #region Helper

        private static string Key<T>() => typeof(T).FullName!;

        private static void Create()
        {
            Debug.Log("Singleton", "Creating new singleton registry!");
            singleton = CreateInstance<SingletonRegistry>();

#if UNITY_EDITOR
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
            }

            UnityEditor.AssetDatabase.CreateAsset(singleton, "Assets/Resources/Singletons.asset");
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        #endregion
    }
}
