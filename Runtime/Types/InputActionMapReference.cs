using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baracuda.Utilities.Types
{
    [Serializable]
    public struct InputActionMapReference : IEquatable<InputActionMapReference>
    {
        [SerializeField]
        private string actionMapName;

        public static implicit operator string(InputActionMapReference inputActionMap)
        {
            return inputActionMap.actionMapName;
        }

        public static implicit operator InputActionMapReference(string name)
        {
            return new InputActionMapReference(name);
        }

        public InputActionMapReference(string name)
        {
            actionMapName = name;
        }

        public InputActionMapReference(InputActionMap inputActionMap)
        {
            actionMapName = inputActionMap.name;
        }

        public override string ToString()
        {
            return actionMapName;
        }

        public bool Equals(InputActionMapReference other)
        {
            return actionMapName == other.actionMapName;
        }

        public override bool Equals(object obj)
        {
            return obj is InputActionMapReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return actionMapName != null ? actionMapName.GetHashCode() : 0;
        }
    }
}