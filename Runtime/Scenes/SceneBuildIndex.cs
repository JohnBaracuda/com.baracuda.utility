using System;
using UnityEngine.SceneManagement;

namespace Baracuda.Bedrock.Scenes
{
    public readonly struct SceneBuildIndex : IEquatable<SceneBuildIndex>
    {
        public readonly int Value;
        public readonly bool IsConstructed;

        public SceneBuildIndex(int value)
        {
            if (!IsValidBuildIndex(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"The index {value} is not valid in the current build settings.");
            }

            Value = value;
            IsConstructed = true;
        }

        public bool Equals(SceneBuildIndex other)
        {
            return Value == other.Value && IsConstructed == other.IsConstructed;
        }

        public override bool Equals(object obj)
        {
            return obj is SceneBuildIndex other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, IsConstructed);
        }

        public static bool operator ==(SceneBuildIndex left, SceneBuildIndex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SceneBuildIndex left, SceneBuildIndex right)
        {
            return !(left == right);
        }

        public static implicit operator int(SceneBuildIndex index)
        {
            return index.Value;
        }

        public static implicit operator SceneBuildIndex(int value)
        {
            return new SceneBuildIndex(value);
        }

        private static bool IsValidBuildIndex(int index)
        {
            return index >= 0 && index < SceneManager.sceneCountInBuildSettings;
        }
    }
}