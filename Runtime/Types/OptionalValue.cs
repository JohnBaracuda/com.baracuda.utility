using System;
using UnityEngine;

namespace Baracuda.Utilities
{
    /// <summary>
    /// Wrap a value <see cref="T"/> in this struct to make accessing it conditional.
    /// </summary>
    /// <typeparam name="T">Inner type wrapped by the optional value</typeparam>
    [Serializable]
    public struct OptionalValue<T>
    {
        // Configuration

        [SerializeField] private bool enabled;
        [SerializeField] private T value;

        /// <summary>
        /// Determines if the value is enabled or not. Accessing a disabled value is not allowed and will result in an exception.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        /// Get the inner value. Accessing this property asserts that you are allowed to access the value.
        /// Use <see cref="ValueOrDefault()"/> or <see cref="TryGetValue"/> if you don't know if accessing the value is allowed.
        /// </summary>
        public T Value
        {
            get
            {
                UnityEngine.Assertions.Assert.IsTrue(enabled);
                return value;
            }
        }

        /// <summary>
        /// Get either the inner value or a default value, depending on whether or not <see cref="Enabled"/> is true.
        /// </summary>
        /// <returns>The inner value</returns>
        public T ValueOrDefault() => TryGetValue(out var optionalValue) ? optionalValue : default;

        /// <summary>
        /// Get either the inner value or a default value, depending on whether or not <see cref="Enabled"/> is true.
        /// </summary>
        /// <param name="defaultValue">Override the default value that is returned if <see cref="Enabled"/> is false.</param>
        /// <returns>The inner value</returns>
        public T ValueOrDefault(T defaultValue) => TryGetValue(out var optionalValue) ? optionalValue : defaultValue;

        /// <summary>
        /// Get <see cref="Value"/> if <see cref="Enabled"/> is true.
        /// </summary>
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

        public static implicit operator T(OptionalValue<T> optionalValue)
        {
            return optionalValue.Value;
        }

        public static implicit operator OptionalValue<T>(T value)
        {
            return new OptionalValue<T>(value);
        }

        public static implicit operator OptionalValue<T>((T value, bool enabled) tuple)
        {
            return new OptionalValue<T>(tuple.value, tuple.enabled);
        }

        public OptionalValue(T value)
        {
            this.value = value;
            enabled = true;
        }

        public OptionalValue(T value, bool enabled)
        {
            this.value = value;
            this.enabled = enabled;
        }
    }
}
