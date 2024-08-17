using System;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Bedrock.Types
{
    /// <summary>
    ///     Wrap a value <see cref="T" /> in this struct to make accessing it conditional.
    /// </summary>
    /// <typeparam name="T">Inner type wrapped by the optional value</typeparam>
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        /// <summary>
        ///     Determines if the value is enabled or not. Accessing a disabled value is not allowed and will result in an
        ///     exception.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        ///     Get the inner value. Accessing this property asserts that you are allowed to access the value.
        ///     Use <see cref="ValueOrDefault()" /> or <see cref="TryGetValue" /> if you don't know if accessing the value is
        ///     allowed.
        /// </summary>
        public T Value
        {
            get
            {
                Assert.IsTrue(enabled);
                return value;
            }
        }

        /// <summary>
        ///     Get the inner value. Accessing this property does not assert that you are allowed to access the value.
        ///     Use <see cref="ValueOrDefault()" /> or <see cref="TryGetValue" /> if you don't know if accessing the value is
        ///     allowed.
        /// </summary>
        public T GetValueWithoutCheck()
        {
            return value;
        }

        /// <summary>
        ///     Get either the inner value or a default value, depending on whether or not <see cref="Enabled" /> is true.
        /// </summary>
        /// <returns>The inner value</returns>
        [Pure]
        public T ValueOrDefault()
        {
            return TryGetValue(out var optionalValue) ? optionalValue : default;
        }

        /// <summary>
        ///     Get either the inner value or a default value, depending on whether or not <see cref="Enabled" /> is true.
        /// </summary>
        /// <param name="defaultValue">Override the default value that is returned if <see cref="Enabled" /> is false.</param>
        /// <returns>The inner value</returns>
        [Pure]
        public T ValueOrDefault(T defaultValue)
        {
            return TryGetValue(out var optionalValue) ? optionalValue : defaultValue;
        }

        /// <summary>
        ///     Get <see cref="Value" /> if <see cref="Enabled" /> is true.
        /// </summary>
        [Pure]
        public bool TryGetValue(out T optionalValue)
        {
            if (Enabled)
            {
                optionalValue = Value;
                return true;
            }

            optionalValue = default;
            return false;
        }

        public static implicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static implicit operator Optional<T>((T value, bool enabled) tuple)
        {
            return new Optional<T>(tuple.value, tuple.enabled);
        }

        public Optional(T value)
        {
            this.value = value;
            enabled = true;
        }

        public Optional(T value, bool enabled)
        {
            this.value = value;
            this.enabled = enabled;
        }
    }
}