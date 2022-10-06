using System;
using UnityEngine;

namespace Baracuda.Utilities.Types
{
    [Serializable]
    public class Prefab
    {
        public bool enabled = true;
        public GameObject gameObject;

        public static implicit operator GameObject(Prefab prefab)
        {
            return prefab.gameObject;
        }
    }
}
