using UnityEngine;

namespace Baracuda.Utilities
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        public static T Singleton
        {
            get => SingletonRegistry.Resolve<T>();
            private set => SingletonRegistry.Register(value);
        }

        protected virtual void OnEnable()
        {
            Singleton = (T) this;
        }
    }
}
