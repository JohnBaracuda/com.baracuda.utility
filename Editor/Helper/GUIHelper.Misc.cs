using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        /*
         * Size
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSizeOfLabel(GUIContent label)
        {
            return EditorStyles.label.CalcSize(label);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIndent()
        {
            return EditorGUI.indentLevel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetControlRect()
        {
            return EditorGUILayout.GetControlRect();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLastRectIndented()
        {
            return EditorGUI.IndentedRect(GUILayoutUtility.GetLastRect());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetViewWidth()
        {
            return EditorGUIUtility.currentViewWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLabelWidth()
        {
            return EditorGUIUtility.labelWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLabelWidthIndented()
        {
            return EditorGUIUtility.labelWidth - (EditorGUI.indentLevel * 15f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFieldWidth()
        {
            return EditorGUIUtility.fieldWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLineHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        /*
         * Object Specific
         */

        public static GUIContent GetContentForObject(Object target, Type type = null)
        {
            type ??= target.GetType();
            var name = type.GetCustomAttribute<AddComponentMenu>()?.componentMenu?.Split('/').Last() ??
                       type.Name.Humanize();
            return new GUIContent(" " + name, AssetPreview.GetMiniThumbnail(target));
        }

        /*
         * Box
         */

        public static void BeginBox()
        {
            EditorGUILayout.BeginVertical(HelpBoxStyle);
        }

        public static void EndBox()
        {
            EditorGUILayout.EndVertical();
        }

        /*
         * Drag & Drop
         */

        public static Object[] DropZone(string title, params GUILayoutOption[] options)
        {
            BeginBox();

            GUILayout.Box(new GUIContent(title), options);

            var eventType = Event.current.type;
            var isAccepted = false;

            if (eventType is EventType.DragUpdated or EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }

                Event.current.Use();
            }

            EndBox();
            return isAccepted ? DragAndDrop.objectReferences : null;
        }

        /*
         * Async
         */

        public static async Task CompleteGUI()
        {
            await Task.Delay(25);
        }

        /*
         * Indent
         */

        private static readonly Stack<int> indentOverrides = new(4);

        public static void BeginIndentOverride(int indent)
        {
            var current = EditorGUI.indentLevel;
            indentOverrides.Push(current);
            EditorGUI.indentLevel = indent;
        }

        public static void EndIndentOverride()
        {
            if (indentOverrides.TryPop(out var cached))
            {
                EditorGUI.indentLevel = cached;
            }
            else
            {
                Debug.LogError($"Mismatched calls of {nameof(BeginIndentOverride)} & {nameof(EndIndentOverride)}!");
            }
        }
    }
}