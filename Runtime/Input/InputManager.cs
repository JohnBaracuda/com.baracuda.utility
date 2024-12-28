using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baracuda.Utility.Input
{
    /// <summary>
    ///     Input Manager handles input state (controller or desktop) and input map states.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public partial class InputManager : MonoBehaviour
    {
        #region Properties

        public bool IsGamepadScheme { get; private set; }
        public bool IsDesktopScheme => !IsGamepadScheme;
        public InteractionMode InteractionMode { get; private set; }
        public bool EnableNavigationEvents { get; set; } = true;
        public PlayerInput PlayerInput => playerInput;

        #endregion


        #region Events

        public event Action BecameControllerScheme
        {
            add => _onBecameControllerScheme.AddListener(value);
            remove => _onBecameControllerScheme.RemoveListener(value);
        }

        public event Action BecameDesktopScheme
        {
            add => _onBecameDesktopScheme.AddListener(value);
            remove => _onBecameDesktopScheme.RemoveListener(value);
        }

        public event Action NavigationInputReceived
        {
            add => _onNavigationInputReceived.AddUniqueListener(value);
            remove => _onNavigationInputReceived.RemoveListener(value);
        }

        public event Action MouseInputReceived
        {
            add => _onMouseInputReceived.AddListener(value);
            remove => _onMouseInputReceived.RemoveListener(value);
        }

        #endregion


        #region Methods: Action Maps

        public void BlockActionMap(InputActionMapReference actionMap, object source)
        {
            BlockActionMapInternal(actionMap, source);
        }

        public void UnblockActionMap(InputActionMapReference actionMap, object source)
        {
            UnblockActionMapInternal(actionMap, source);
        }

        public void AddActionMapSource(InputActionMapReference actionMapReference, object source)
        {
            ProvideActionMapInternal(actionMapReference, source);
        }

        public void RemoveActionMapSource(InputActionMapReference actionMapReference, object source)
        {
            WithdrawActionMapInternal(actionMapReference, source);
        }

        #endregion
    }
}