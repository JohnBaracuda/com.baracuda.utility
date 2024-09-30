using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Baracuda.Bedrock.Services
{
    [PublicAPI]
    public interface IServiceContainer
    {
        /// <summary>
        ///     Gets all services registered in the container, excluding transient and non initialized lazy services if not
        ///     explicitly requested.
        /// </summary>
        IEnumerable<object> GetAllServices(bool includeLazy = false, bool includeTransient = false);

        /// <summary>
        ///     Gets the number of services registered in the container.
        /// </summary>
        int ServiceCount { get; }

        /// <summary>
        ///     Gets a service of the specified type.
        /// </summary>
        T Get<T>() where T : class;

        /// <summary>
        ///     Gets a service of the specified type.
        /// </summary>
        object Get(Type type);

        /// <summary>
        ///     Tries to get a service of the specified type.
        /// </summary>
        bool TryGet<T>(out T service) where T : class;

        /// <summary>
        ///     Tries to get a service of the specified type.
        /// </summary>
        bool TryGet(Type type, out object service);

        /// <summary>
        ///     Adds a service to the container.
        /// </summary>
        IServiceContainer Add<T>(T service);

        /// <summary>
        ///     Adds a service to the container.
        /// </summary>
        IServiceContainer Add(Type type, object service);

        /// <summary>
        ///     Adds a transient service to the container.
        /// </summary>
        IServiceContainer AddTransient<T>(Func<T> func);

        /// <summary>
        ///     Adds a transient service to the container.
        /// </summary>
        IServiceContainer AddTransient(Type type, Delegate func);

        /// <summary>
        ///     Adds a lazy-loaded service to the container.
        /// </summary>
        IServiceContainer AddLazy<T>(Func<T> func);

        /// <summary>
        ///     Adds a lazy-loaded service to the container.
        /// </summary>
        IServiceContainer AddLazy(Type type, Delegate func);

        /// <summary>
        ///     Checks if a service of the specified type exists in the container.
        /// </summary>
        bool Contains<T>();

        /// <summary>
        ///     Removes a specific service from the container.
        /// </summary>
        IServiceContainer Remove<T>(T service);

        /// <summary>
        ///     Removes a service of the specified type from the container.
        /// </summary>
        IServiceContainer Remove<T>();

        /// <summary>
        ///     Sets a fallback container for resolving services.
        /// </summary>
        IServiceContainer SetFallbackContainer(IServiceContainer fallback);

        /// <summary>
        ///     Resets the container by removing all services.
        /// </summary>
        void Clear();
    }
}