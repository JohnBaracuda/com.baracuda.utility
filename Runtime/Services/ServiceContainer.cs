﻿using System;
using System.Collections.Generic;
using Baracuda.Utility.Collections;
using Baracuda.Utility.Utilities;
using JetBrains.Annotations;

namespace Baracuda.Utility.Services
{
    public class ServiceContainer : IServiceContainer
    {
        #region Fields

        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Delegate> _transientServices = new();
        private readonly Dictionary<Type, Delegate> _lazyServices = new();
        private readonly HashSet<Type> _registeredServiceTypes = new();
        private readonly LogCategory _category = nameof(ServiceContainer);
        private Func<IServiceContainer> _fallbackContainer;

        #endregion


        #region Public API: Get

        [PublicAPI]
        public IEnumerable<object> GetAllServices(bool includeLazy = false, bool includeTransient = false)
        {
            foreach (var services in _services.Values)
            {
                yield return services;
            }
            if (includeLazy)
            {
                throw new NotImplementedException();
            }
            if (includeTransient)
            {
                throw new NotImplementedException();
            }
        }

        [PublicAPI]
        public int ServiceCount => _services.Count;

        [PublicAPI]
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var value))
            {
                return value as T;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var service = lazyFunc.CastExplicit<Func<T>>()();
                _services.Add(type, service);
                _lazyServices.Remove(type);
                return service;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                return transientFunc.CastExplicit<Func<T>>()();
            }

            if (_fallbackContainer is not null)
            {
                return _fallbackContainer().Get<T>();
            }

            Debug.LogWarning(_category, $"Service of type {type.FullName} not registered");
            return null;
        }

        [PublicAPI]
        public object Get(Type type)
        {
            if (_services.TryGetValue(type, out var value))
            {
                return value;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var service = lazyFunc.DynamicInvoke();
                _services.Add(type, service);
                _lazyServices.Remove(type);
                return service;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                return transientFunc.DynamicInvoke();
            }

            if (_fallbackContainer is not null)
            {
                return _fallbackContainer().Get(type);
            }

            Debug.LogWarning(_category, $"Service of type {type.FullName} not registered");
            return null;
        }

        #endregion


        #region Public API: Try Get

        [PublicAPI]
        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var value))
            {
                service = value as T;
                return true;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var result = lazyFunc.CastExplicit<Func<T>>()();
                _services.Add(type, result);
                _lazyServices.Remove(type);
                service = result;
                return true;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                service = transientFunc.CastExplicit<Func<T>>()();
                return true;
            }

            if (_fallbackContainer is not null)
            {
                return _fallbackContainer().TryGet(out service);
            }

            service = default;
            return false;
        }

        [PublicAPI]
        public bool TryGet(Type type, out object service)
        {
            if (_services.TryGetValue(type, out var value))
            {
                service = value;
                return true;
            }

            if (_lazyServices.TryGetValue(type, out var lazyFunc))
            {
                var result = lazyFunc.DynamicInvoke();
                _services.Add(type, result);
                _lazyServices.Remove(type);
                service = result;
                return true;
            }

            if (_transientServices.TryGetValue(type, out var transientFunc))
            {
                service = transientFunc.DynamicInvoke();
                return true;
            }

            if (_fallbackContainer is not null)
            {
                return _fallbackContainer().TryGet(type, out service);
            }

            service = default;
            return false;
        }

        #endregion


        #region Public API: Add Singleton

        [PublicAPI]
        public IServiceContainer Add<T>(T service)
        {
            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _services.Add(typeof(T), service);

            return this;
        }

        [PublicAPI]
        public IServiceContainer Add(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                Debug.LogWarning(_category, "Type of service does not match type of service interface!");
                return this;
            }

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _services.Add(type, service);

            return this;
        }

        [PublicAPI]
        public IServiceContainer AddTransient<T>(Func<T> func)
        {
            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _transientServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public IServiceContainer AddTransient(Type type, Delegate func)
        {
            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            if (func.Method.ReturnType != type)
            {
                Debug.LogWarning(_category, "Delegate of transient service is invalid!");
                return this;
            }

            _transientServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public IServiceContainer AddLazy<T>(Func<T> func)
        {
            var type = typeof(T);

            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            _lazyServices.Add(type, func);

            return this;
        }

        [PublicAPI]
        public IServiceContainer AddLazy(Type type, Delegate func)
        {
            if (!_registeredServiceTypes.Add(type))
            {
                Debug.LogWarning(_category, $"Service of type {type.FullName} is already registered!");
                return this;
            }

            if (func.Method.ReturnType != type)
            {
                Debug.LogWarning(_category, "Delegate of transient service is invalid!");
                return this;
            }

            _lazyServices.Add(type, func);

            return this;
        }

        #endregion


        #region Public API: Contains

        public bool Contains<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        #endregion


        #region Public API: Remove

        [PublicAPI]
        public IServiceContainer Remove<T>(T service)
        {
            var type = typeof(T);

            if (!_registeredServiceTypes.Remove(type))
            {
                return this;
            }

            _services.TryRemove(type);
            _transientServices.TryRemove(type);
            _lazyServices.TryRemove(type);
            return this;
        }

        [PublicAPI]
        public IServiceContainer Remove<T>()
        {
            var type = typeof(T);

            if (!_registeredServiceTypes.Remove(type))
            {
                return this;
            }

            _services.TryRemove(type);
            _transientServices.TryRemove(type);
            _lazyServices.TryRemove(type);
            return this;
        }

        #endregion


        #region Internal API:

        public IServiceContainer SetFallbackContainer(IServiceContainer fallback)
        {
            _fallbackContainer = () => fallback;
            return this;
        }

        public IServiceContainer SetFallbackContainer(Func<IServiceContainer> fallback)
        {
            _fallbackContainer = fallback;
            return this;
        }

        public void Clear()
        {
            _services.Clear();
            _transientServices.Clear();
            _lazyServices.Clear();
            _registeredServiceTypes.Clear();
        }

        #endregion
    }
}