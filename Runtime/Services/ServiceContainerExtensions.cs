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
        public static T Add<TInterface, T>(this IServiceContainer container) where T : TInterface, new()
        {
            var instance = new T();
            container.Add<TInterface>(instance);
            return instance;
        }

        [PublicAPI]
        public static T Add<TInterface, T>(this IServiceContainer container, T instance) where T : TInterface
        {
            container.Add<TInterface>(instance);
            return instance;
        }

        [PublicAPI]
        public static T Add<T>(this IServiceContainer container) where T : class, new()
        {
            var instance = new T();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T Add<T>(this IServiceContainer container, T instance) where T : class
        {
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddMonoBehaviour<T>(this IServiceContainer container) where T : MonoBehaviour
        {
            var instance = new GameObject($"[{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add(service);
            return service;
        }

        [PublicAPI]
        public static T AddMonoBehaviour<TInterface, T>(this IServiceContainer container) where T : MonoBehaviour, TInterface
        {
            var instance = new GameObject($"[{typeof(TInterface).Name}] [{typeof(T).Name}]");
            instance.DontDestroyOnLoad();
            var service = instance.AddComponent<T>();
            container.Add<TInterface>(service);
            return service;
        }

        [PublicAPI]
        public static T AddPrefab<T>(this IServiceContainer container, T prefab) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddPrefab<T>(this IServiceContainer container, T prefab, Action<T> callback) where T : MonoBehaviour
        {
            var instance = Object.Instantiate(prefab);
            instance.name = $"[{typeof(T).Name}]";
            instance.DontDestroyOnLoad();
            container.Add(instance);
            callback(instance);
            return instance;
        }

        [PublicAPI]
        public static T AddPrefab<T>(this IServiceContainer container, GameObject prefab) where T : MonoBehaviour
        {
            var gameObject = Object.Instantiate(prefab);
            var instance = gameObject.GetComponent<T>();
            instance.DontDestroyOnLoad();
            container.Add(instance);
            return instance;
        }

        [PublicAPI]
        public static void AddPrefabLazy<T>(this IServiceContainer container, T prefab) where T : MonoBehaviour
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
        public static void SortMonoBehaviourHierarchy(this IServiceContainer container)
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