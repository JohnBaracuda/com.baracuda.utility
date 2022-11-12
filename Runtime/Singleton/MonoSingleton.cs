using UnityEngine;

namespace Baracuda.Utilities.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Singleton { get; private set; }

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                var typename = typeof(T).Name;
                Debug.LogWarning("Singleton", $"More that one instance of {typename} found!");
                return;
            }
            Singleton = this as T;
        }

        private void OnDestroy()
        {
            if (Singleton == this)
            {
                Singleton = null;
            }
        }
    }
}
