using Baracuda.Utilities.Helper;
using Baracuda.Utilities.Inspector;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.FactoryWindow
{
    [Flags]
    public enum SearchOptions
    {
        None = 0,
        BaseTypes = 1,
        AssemblyName = 2,
        CreateAttributePath = 4,
    }

    public class ObjectFactorySettings : EditorWindow
    {
        #region Data

        /*
         * Internal
         */

        internal static event Action<bool> SettingsChanged;

        /*
         * Private
         */

        private static bool isOpen;
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedPropertyPrefixes;
        private SerializedProperty _serializedPropertyNames;
        [SerializeField] private string[] ignoredAssemblyPrefixes = Array.Empty<string>();
        [SerializeField] private string[] ignoredNames = Array.Empty<string>();
        /*
         * Properties
         */

        private FoldoutHandler Foldout { get; set; }

        internal static string[] GetIgnoredAssemblyPrefixes()
        {
            var data = EditorPrefs.GetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}", string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredAssemblyPrefixes(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            EditorPrefs.SetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}", data);
        }

        internal static string[] GetIgnoredNames()
        {
            var data = EditorPrefs.GetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredNames)}", string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredNames(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            EditorPrefs.SetString($"{nameof(ObjectFactorySettings)}{nameof(ignoredNames)}", data);
        }


        internal static bool EnableMultiAssetCreation
        {
            get => EditorPrefs.GetBool($"{nameof(ObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}", false);
            set => EditorPrefs.SetBool($"{nameof(ObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}", value);
        }

        internal static SearchOptions SearchOptions
        {
            get => (SearchOptions) EditorPrefs.GetInt($"{nameof(ObjectFactoryWindow)}{nameof(SearchOptions)}", (int)(SearchOptions.AssemblyName | SearchOptions.CreateAttributePath | SearchOptions.BaseTypes));
            set => EditorPrefs.SetInt($"{nameof(ObjectFactoryWindow)}{nameof(SearchOptions)}", (int)value);
        }

        #endregion

        #region Setup

        internal static void OpenWindow()
        {
            if (!isOpen)
            {
                var window = GetWindow<ObjectFactorySettings>("Settings");
                window.Show(false);
            }
        }

        private void OnEnable()
        {
            isOpen = true;
            Foldout = new FoldoutHandler(nameof(ObjectFactorySettings));

            ignoredAssemblyPrefixes = GetIgnoredAssemblyPrefixes();
            ignoredNames = GetIgnoredNames();

            _serializedObject = new SerializedObject(this);
            _serializedPropertyPrefixes = _serializedObject.FindProperty(nameof(ignoredAssemblyPrefixes));
            _serializedPropertyNames = _serializedObject.FindProperty(nameof(ignoredNames));

            ObjectFactoryWindow.WindowClosed += Close;
        }

        private void OnDisable()
        {
            Save();
            isOpen = false;
            Foldout.SaveState();
            ObjectFactoryWindow.WindowClosed -= Close;
        }

        private void Save()
        {
            SetIgnoredAssemblyPrefixes(ignoredAssemblyPrefixes);
            SetIgnoredNames(ignoredNames);
            SettingsChanged?.Invoke(true);
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            var changed = false;
            EditorGUI.indentLevel--;

            if (Foldout["Search Options"])
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                GUIHelper.Space();
                var newSearchOptions = GUIHelper.DrawFlagsEnumAsToggle(SearchOptions, true);
                if (newSearchOptions != SearchOptions)
                {
                    changed = true;
                }
                SearchOptions = newSearchOptions;
                GUIHelper.Space();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            if (Foldout["Misc"])
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                GUIHelper.Space();

                var enableMulti = EnableMultiAssetCreation;
                var newEnableMulti = EditorGUILayout.Toggle("Multi Asset Creation", enableMulti);
                if (newEnableMulti != enableMulti)
                {
                    changed = true;
                }
                EnableMultiAssetCreation = newEnableMulti;

                GUIHelper.Space();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            if (Foldout["Assemblies"])
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                GUIHelper.Space();

                _serializedObject.Update();
                EditorGUILayout.PropertyField(_serializedPropertyPrefixes);
                EditorGUILayout.PropertyField(_serializedPropertyNames);
                _serializedObject.ApplyModifiedProperties();
                GUIHelper.Space();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            GUILayout.FlexibleSpace();
            GUIHelper.DrawLine();
            GUIHelper.Space();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save", GUILayout.Width(140)))
            {
                Save();
            }
            if (GUILayout.Button("Save & Close", GUILayout.Width(140)))
            {
                Save();
                Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUIHelper.Space();

            if (changed)
            {
                SettingsChanged?.Invoke(false);
            }
        }

        #endregion
    }
}