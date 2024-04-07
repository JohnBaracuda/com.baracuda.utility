using System;
using UnityEngine;

namespace Baracuda.Utilities.Types
{
    [Serializable]
    public struct InputActionMapName
    {
        [SerializeField]
        private string actionMapName;

        public static implicit operator string(InputActionMapName inputActionMap)
        {
            return inputActionMap.actionMapName;
        }

        public static implicit operator InputActionMapName(string name)
        {
            return new InputActionMapName(name);
        }

        public InputActionMapName(string name)
        {
            actionMapName = name;
        }

        public override string ToString()
        {
            return actionMapName;
        }
    }
}