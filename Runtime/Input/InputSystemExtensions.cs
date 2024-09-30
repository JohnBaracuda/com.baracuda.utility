using System;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Baracuda.Utility.Input
{
    public enum InputActionEventType
    {
        Performed = 0,
        Started = 1,
        Cancelled = 2
    }

    public static class InputSystemExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddListener(this InputActionReference inputActionReference, Action<CallbackContext> listener)
        {
            inputActionReference.action.performed += listener;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveListener(this InputActionReference inputActionReference, Action<CallbackContext> listener)
        {
            inputActionReference.action.performed -= listener;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAddListener(this InputActionReference inputActionReference,
            Action<CallbackContext> listener, InputActionEventType eventType = InputActionEventType.Performed)
        {
            if (inputActionReference == null)
            {
                return false;
            }

            switch (eventType)
            {
                case InputActionEventType.Performed:
                    inputActionReference.action.performed += listener;
                    return true;

                case InputActionEventType.Started:
                    inputActionReference.action.started += listener;
                    return true;

                case InputActionEventType.Cancelled:
                    inputActionReference.action.canceled += listener;
                    return true;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemoveListener(this InputActionReference inputActionReference,
            Action<CallbackContext> listener, InputActionEventType eventType = InputActionEventType.Performed)
        {
            if (inputActionReference == null)
            {
                return false;
            }

            switch (eventType)
            {
                case InputActionEventType.Performed:
                    inputActionReference.action.performed -= listener;
                    return true;

                case InputActionEventType.Started:
                    inputActionReference.action.started -= listener;
                    return true;

                case InputActionEventType.Cancelled:
                    inputActionReference.action.canceled -= listener;
                    return true;

                default:
                    return false;
            }
        }
    }
}