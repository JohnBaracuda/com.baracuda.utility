using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Utilities
{
    public abstract class AssetSingleton<T> : ScriptableObject, ISerializationCallbackReceiver where T : AssetSingleton<T>
    {
        public static T Singleton
        {
            get
            {
                if (initialized)
                {
                    return singleton;
                }
                singleton = Resources.Load<T>(string.Empty);
#if UNITY_EDITOR
                if (singleton == null)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
                    for (var i = 0; i < guids.Length; i++)
                    {
                        var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                        if (asset != null)
                        {
                            singleton = asset;
                            break;
                        }
                    }
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
    }
}
