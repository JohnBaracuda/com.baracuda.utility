using System;
using System.Linq;
using Baracuda.Bedrock.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.Services
{
    public static class ServiceContainerExtensions
    {
        [PublicAPI]
        public static T AddSingleton<TInterface, T>(this ServiceContainer container) where T : TInterface, new()
        {
            var instance = new T();
            container.Add<TInterface>(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingleton<TInterface, T>(this ServiceContainer container, T instance) where T : TInterface
        {
            container.Add<TInterface>(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingleton<T>(this ServiceContainer container) where T : class, new()
        {
            var instance = new T();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingleton<T>(this ServiceContainer container, T instance) where T : class
        {
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingletonBehaviour<T>(this ServiceContainer container) where T : MonoBehaviour
        {
            var instance = new GameObject($"[{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add(service);
            return service;
        }

        [PublicAPI]
        public static T AddSingletonBehaviour<TInterface, T>(this ServiceContainer container) where T : MonoBehaviour, TInterface
        {
            var instance = new GameObject($"[{typeof(TInterface).Name}] [{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add<TInterface>(service);
            return service;
        }

        [PublicAPI]
        public static T AddSingletonPrefab<T>(this ServiceContainer container, T prefab) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingletonPrefab<T>(this ServiceContainer container, T prefab, Action<T> callback) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.Add(instance);
            callback(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddSingletonPrefab<T>(this ServiceContainer container, GameObject prefab) where T : MonoBehaviour
        {
            var gameObject = Object.Instantiate(prefab);
            var instance = gameObject.GetComponent<T>();
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static void AddSingletonPrefabLazy<T>(this ServiceContainer container, T prefab) where T : MonoBehaviour
        {
            container.AddLazy(() =>
            {
                var instance = Object.Instantiate(prefab);
                instance.name = $"[{typeof(T).Name}]";
                instance.DontDestroyOnLoad();
                return instance;
            });
        }

        [PublicAPI]
        public static void SortMonoBehaviourHierarchy(this ServiceContainer container)
        {
            foreach (var globalService in container.GetAllServices().Reverse())
            {
                if (globalService is MonoBehaviour behaviour)
                {
                    behaviour.transform.SetSiblingIndex(0);
                }
            }
        }
    }
}