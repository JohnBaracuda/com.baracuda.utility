using UnityEngine;

namespace Baracuda.Utilities.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Singleton { get; private set; }
        public static bool IsSingletonInitialized { get; private set; } = false;

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                var typename = typeof(T).Name;
                Debug.LogWarning("Singleton", $"More that one instance of {typename} found!");
                return;
            }
            Singleton = (T) this;
            IsSingletonInitialized = true;
        }

        private void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
                IsSingletonInitialized = false;
            }
        }
    }
}
