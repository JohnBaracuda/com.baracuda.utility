using System;
using UnityEngine;

namespace Baracuda.Bedrock.Input
{
    [Serializable]
    public struct ControlSchemeReference : IEquatable<ControlSchemeReference>
    {
        [SerializeField]
        private string value;

        public string Value => value;

        public static implicit operator string(ControlSchemeReference controlActionMap)
        {
            return controlActionMap.value;
        }

        public static implicit operator ControlSchemeReference(string name)
        {
            return new ControlSchemeReference(name);
        }

        public ControlSchemeReference(string name)
        {
            value = name;
        }

        public override string ToString()
        {
            return value;
        }

        public bool Equals(ControlSchemeReference other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            return obj is ControlSchemeReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }
    }
}