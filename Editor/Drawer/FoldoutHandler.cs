using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Utilities;
using UnityEngine;

namespace Baracuda.Utility.Editor.Drawer
{
    public enum FoldoutStyle
    {
        Default = 0,
        Line = 1,
        Dark = 2,
        Simple = 4
    }

    public class FoldoutHandler
    {
        /*
         * Static access
         */

        public static void SetDirty()
        {
            Active?.SaveState();
        }

        /*
         * Properties
         */

        public static string ActiveTitle { get; private set; }
        public static FoldoutHandler Active { get; private set; }
        public static bool ForceFoldout { get; set; } = false;

        public static FoldoutStyle Style { get; set; } = FoldoutStyle.Default;

        public bool DefaultState { get; set; } = true;
        private Dictionary<string, bool> Data { get; }

        public Dictionary<string, bool> DefaultFoldoutStates { get; } = new();

        /*
         * Fields
         */

        private readonly string _dataKey;
        private readonly Color? _color;
        private static readonly HashSet<FoldoutHandler> activeHandlers = new(4);

        /*
         * Ctor
         */

        public FoldoutHandler(string dataKey = null, Color? color = null)
        {
            activeHandlers.Add(this);

            _dataKey = dataKey;
            _color = color;

            var data = UnityEditor.EditorPrefs.GetString(_dataKey);

            if (!data.IsNotNullOrWhitespace())
            {
                Data = new Dictionary<string, bool>();
                return;
            }

            var dictionary = new Dictionary<string, bool>();
            var lines = data.Split('$');
            foreach (var line in lines)
            {
                if (line.IsNullOrWhitespace())
                {
                    continue;
                }

                var entries = line.Split('§');
                if (entries.Length == 2)
                {
                    var title = entries[0];
                    var value = bool.Parse(entries[1]);
                    dictionary.Add(title, value);
                }
            }

            Data = dictionary;
        }

        public void SaveState()
        {
            var data = string.Empty;
            foreach (var (entry, value) in Data)
            {
                data += $"${entry}§{value}";
            }
            UnityEditor.EditorPrefs.SetString(_dataKey, data);
        }

        public void ForceHeader(FoldoutData data)
        {
            DrawFoldoutForStyle(true, data.Title, data.Tooltip, _color);
        }

        public bool this[in FoldoutData data]
        {
            get
            {
                Active = this;
                ActiveTitle = data.Title;

                if (!Data.TryGetValue(data.Title, out var currentValue))
                {
                    var defaultState = DefaultFoldoutStates.TryGetValue(data.Title, out var state)
                        ? state
                        : DefaultState;
                    Data.Add(data.Title, currentValue = defaultState);
                }

                var (title, tooltip) = data;

                if (ForceFoldout)
                {
                    DrawFoldoutForStyle(true, title, tooltip, _color);
                    return true;
                }

                var newValue = DrawFoldoutForStyle(currentValue, title, tooltip, _color);
                this[data] = newValue;

                if (newValue != currentValue && Event.current.alt)
                {
                    SetAll(newValue, this);
                }

                return currentValue;
            }
            private set
            {
                if (!Data.TryAdd(data.Title, value))
                {
                    Data[data.Title] = value;
                }
            }
        }

        public bool this[string title]
        {
            get
            {
                Active = this;
                ActiveTitle = title;

                if (!Data.TryGetValue(title, out var currentValue))
                {
                    var defaultState = DefaultFoldoutStates.TryGetValue(title, out var state)
                        ? state
                        : DefaultState;
                    Data.Add(title, currentValue = defaultState);
                }

                if (ForceFoldout)
                {
                    DrawFoldoutForStyle(true, title, string.Empty, _color);
                    return true;
                }

                var newValue = DrawFoldoutForStyle(currentValue, title, string.Empty, _color);
                this[title] = newValue;

                if (newValue != currentValue && Event.current.alt)
                {
                    SetAll(newValue, this);
                }

                return currentValue;
            }
            private set
            {
                if (!Data.TryAdd(title, value))
                {
                    Data[title] = value;
                }
            }
        }

        public bool this[string title, string tooltip]
        {
            get
            {
                Active = this;
                ActiveTitle = title;

                if (!Data.TryGetValue(title, out var currentValue))
                {
                    var defaultState = DefaultFoldoutStates.TryGetValue(title, out var state)
                        ? state
                        : DefaultState;
                    Data.Add(title, currentValue = defaultState);
                }

                if (ForceFoldout)
                {
                    DrawFoldoutForStyle(true, title, tooltip, _color);
                    return true;
                }

                var newValue = DrawFoldoutForStyle(currentValue, title, tooltip, _color);
                this[title] = newValue;

                if (newValue != currentValue && Event.current.alt)
                {
                    SetAll(newValue, this);
                }

                return currentValue;
            }
            private set
            {
                if (!Data.TryAdd(title, value))
                {
                    Data[title] = value;
                }
            }
        }

        private bool DrawFoldoutForStyle(bool value, string title, string tooltip, Color? color)
        {
            switch (Style)
            {
                case FoldoutStyle.Default:
                    return Foldout(value, title, tooltip, color);

                case FoldoutStyle.Line:
                    return LineFoldout(value, title, tooltip, color);

                case FoldoutStyle.Dark:
                    return DarkFoldout(value, title, tooltip);

                case FoldoutStyle.Simple:
                    return FoldoutSimple(value, title, tooltip);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void SetAll(bool open, FoldoutHandler handler)
        {
            foreach (var foldoutHandler in activeHandlers)
            {
                var keys = foldoutHandler.Data.Keys.Select(key => key).ToArray();

                if (handler != foldoutHandler)
                {
                    continue;
                }

                foreach (var key in keys)
                {
                    foldoutHandler.Data[key] = open;
                }
            }
        }


        #region Foldout

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Foldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            UnityEditor.EditorGUI.DrawRect(widthRect, color.GetValueOrDefault(new Color(0f, 0f, 0f, 0.15f)));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            if (result != value)
            {
                SaveState();
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FoldoutSimple(bool value, string label, string tooltip = "")
        {
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUILayout.Foldout(value, EmptyContent, true);
            if (result != value)
            {
                SaveState();
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool LineFoldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            if (result != value)
            {
                SaveState();
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DarkFoldout(bool value, string label, string tooltip = "")
        {
            UnityEditor.EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            UnityEditor.EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            UnityEditor.EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.24f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = UnityEditor.EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            if (result != value)
            {
                SaveState();
            }
            return result;
        }

        #endregion


        #region Misc

        private static GUIContent EmptyContent { get; } = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetViewWidth()
        {
            return UnityEditor.EditorGUIUtility.currentViewWidth;
        }

        private static GUIStyle BoldFoldoutStyle =>
            boldFoldoutStyle ??= Create("Foldout", fontStyle: FontStyle.Normal, fontSize: 14);

        private static GUIStyle Create(GUIStyle other, int? fontSize = null, FontStyle? fontStyle = null,
            bool? richText = null)
        {
            return new GUIStyle(other)
            {
                fontSize = fontSize ?? other.fontSize,
                fontStyle = fontStyle ?? other.fontStyle,
                richText = richText ?? other.richText
            };
        }

        private static GUIStyle boldFoldoutStyle;

        #endregion


        #region Override Style

        private static readonly Stack<FoldoutStyle> styleOverrides = new(4);

        public static void BeginStyleOverride(FoldoutStyle style)
        {
            var current = Style;
            styleOverrides.Push(current);
            Style = style;
        }

        public static void EndStyleOverride()
        {
            if (styleOverrides.TryPop(out var cached))
            {
                Style = cached;
            }
            else
            {
                UnityEngine.Debug.LogError(
                    $"Mismatched calls of {nameof(BeginStyleOverride)} & {nameof(EndStyleOverride)}!");
            }
        }

        #endregion
    }
}