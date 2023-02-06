using Baracuda.Utilities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
    public enum FoldoutStyle
    {
        Default = 0,
        Line = 1,
        Dark = 2,
        Title = 3,
        Simple = 4,
        DarkGradient = 5,
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

        private Dictionary<string, bool> Data => _data;
        public Dictionary<string, bool> DefaultFoldoutStates { get; } = new Dictionary<string, bool>();

        /*
         * Fields
         */

        private readonly Dictionary<string, bool> _data;
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

            var data = EditorPrefs.GetString(_dataKey);

            if (!data.IsNotNullOrWhitespace())
            {
                _data = new Dictionary<string, bool>();
                return;
            }

            var dictionary = new Dictionary<string, bool>();
            var lines = data.Split('$');
            foreach (var line in lines)
            {
                if (line.IsNullOrWhiteSpace())
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

            _data = dictionary;
        }

        public void SaveState()
        {
            var data = string.Empty;
            foreach (var (entry, value) in Data)
            {
                data += $"${entry}§{value}";
            }
            EditorPrefs.SetString(_dataKey, data);
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

                if (!_data.TryGetValue(data.Title, out var currentValue))
                {
                    _data.Add(data.Title, currentValue = !DefaultFoldoutStates.TryGetValue(data.Title, out var state) || state);
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
                if (!_data.TryAdd(data.Title, value))
                {
                    _data[data.Title] = value;
                }
            }
        }

        public bool this[string title]
        {
            get
            {
                Active = this;
                ActiveTitle = title;

                if (!_data.TryGetValue(title, out var currentValue))
                {
                    _data.Add(title, currentValue = !DefaultFoldoutStates.TryGetValue(title, out var state) || state);
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
                if (!_data.TryAdd(title, value))
                {
                    _data[title] = value;
                }
            }
        }

        public bool this[string title, string tooltip]
        {
            get
            {
                Active = this;
                ActiveTitle = title;

                if (!_data.TryGetValue(title, out var currentValue))
                {
                    _data.Add(title, currentValue = !DefaultFoldoutStates.TryGetValue(title, out var state) || state);
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
                if (!_data.TryAdd(title, value))
                {
                    _data[title] = value;
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
                case FoldoutStyle.Title:
                    return TitleFoldout(value, title, tooltip);
                case FoldoutStyle.Simple:
                    return FoldoutSimple(value, title, tooltip);
                case FoldoutStyle.DarkGradient:
                    return DarkGradientFoldout(value, title, tooltip);
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
        public static bool Foldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EditorGUI.DrawRect(widthRect, color.GetValueOrDefault(new Color(0f, 0f, 0f, 0.15f)));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FoldoutSimple(bool value, string label, string tooltip = "")
        {
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUILayout.Foldout(value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LineFoldout(bool value, string label, string tooltip = "", Color? color = null)
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.3f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DarkFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(20, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.3f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DarkGradientFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 2);
            var foldoutRect = new Rect(lastRect.x, lastRect.y + 1, GetViewWidth() - 10, lastRect.height);
            var lineRect = new Rect(0, lastRect.y, GetViewWidth(), 1);
            GUIHelper.DrawGradient(lineRect, new Color(0f, 0f, 0f, 0.05f), new Color(0f, 0f, 0f, 0.4f), 20, 3);
            GUIHelper.DrawGradient(widthRect, new Color(0f, 0f, 0f, 0.1f), new Color(0f, 0f, 0f, 0.23f), 20, 1);

            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TitleFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, GetViewWidth(), lastRect.height + 6);
            var foldoutRect = new Rect(20, lastRect.y - 2, GetViewWidth() - 10, lastRect.height + 8);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, GetViewWidth(), 1), new Color(0f, 0f, 0f, 0.4f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.25f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            var result = EditorGUI.Foldout(foldoutRect, value, EmptyContent, true, BoldFoldoutStyle);
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DynamicFoldout(bool value, string label, string tooltip = "")
        {
            EditorGUILayout.LabelField("");
            var lastRect = GetLastRect();
            var widthRect = new Rect(0, lastRect.y, lastRect.width + 5, lastRect.height + 2);
            var foldoutRect = new Rect(10, lastRect.y + 1, lastRect.width, lastRect.height);
            EditorGUI.DrawRect(new Rect(0, lastRect.y, lastRect.width + 5, 1), new Color(0f, 0f, 0f, 0.3f));
            EditorGUI.DrawRect(widthRect, new Color(0f, 0f, 0f, 0.15f));
            EmptyContent.text = label;
            EmptyContent.tooltip = tooltip;
            return EditorGUI.Foldout(foldoutRect, value, EmptyContent, true);
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
            return EditorGUIUtility.currentViewWidth;
        }

        private static GUIStyle BoldFoldoutStyle => boldFoldoutStyle ??= Create("Foldout", fontStyle: FontStyle.Normal, fontSize: 14);


        private static GUIStyle Create(GUIStyle other, int? fontSize = null, FontStyle? fontStyle = null, bool? richText = null)
        {
            return new GUIStyle(other)
            {
                fontSize = fontSize ?? other.fontSize,
                fontStyle = fontStyle ?? other.fontStyle,
                richText = richText ?? other.richText,
            };
        }

        private static GUIStyle boldFoldoutStyle;

        #endregion
    }
}