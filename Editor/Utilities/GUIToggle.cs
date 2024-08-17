using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Utilities.Editor.Utilities
{
    public sealed class GUIToggle : IDisposable
    {
        public event Action<bool> Changed;
        public bool Value { get; private set; }

        private readonly Action _draw;

        public GUIToggle(string label, bool startValue, bool toggleLeft = false)
        {
            Value = startValue;
            _draw = toggleLeft
                ? () =>
                {
                    var value = UnityEditor.EditorGUILayout.ToggleLeft(label, Value);
                    UpdateValue(value);
                }
                : () =>
                {
                    var value = UnityEditor.EditorGUILayout.Toggle(label, Value);
                    UpdateValue(value);
                };
        }

        public void Draw()
        {
            _draw();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateValue(bool value)
        {
            if (Value == value)
            {
                return;
            }

            Value = value;
            Changed?.Invoke(value);
        }

        public static implicit operator bool(GUIToggle toggle)
        {
            return toggle.Value;
        }

        public void Dispose()
        {
            Changed = null;
        }

        public void SaveValue(string key)
        {
            UnityEditor.EditorPrefs.SetBool(key, Value);
        }

        public void LoadValue(string key)
        {
            UpdateValue(UnityEditor.EditorPrefs.GetBool(key, Value));
        }
    }
}