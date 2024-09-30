using System;
using System.Linq;
using Baracuda.Utility.Editor.Drawer;
using Baracuda.Utility.Utilities;
using UnityEngine;
using GUIUtility = Baracuda.Utility.Editor.Utilities.GUIUtility;

namespace Baracuda.Utility.Editor.Tools
{
    [Flags]
    public enum SearchOptions
    {
        None = 0,
        BaseTypes = 1,
        AssemblyName = 2,
        CreateAttributePath = 4
    }

    public class ScriptableObjectFactorySettings : UnityEditor.EditorWindow
    {
        #region Data

        internal static event Action<bool> SettingsChanged;

        private static bool isOpen;
        private UnityEditor.SerializedObject _serializedObject;
        private UnityEditor.SerializedProperty _serializedPropertyPrefixes;
        private UnityEditor.SerializedProperty _serializedPropertyNames;
        [SerializeField] private string[] ignoredAssemblyPrefixes = Array.Empty<string>();
        [SerializeField] private string[] ignoredNames = Array.Empty<string>();

        private FoldoutHandler Foldout { get; set; }

        internal static string[] GetIgnoredAssemblyPrefixes()
        {
            var data = UnityEditor.EditorPrefs.GetString(
                $"{nameof(ScriptableObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}", string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredAssemblyPrefixes(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            UnityEditor.EditorPrefs.SetString(
                $"{nameof(ScriptableObjectFactorySettings)}{nameof(ignoredAssemblyPrefixes)}",
                data);
        }

        internal static string[] GetIgnoredNames()
        {
            var data = UnityEditor.EditorPrefs.GetString(
                $"{nameof(ScriptableObjectFactorySettings)}{nameof(ignoredNames)}",
                string.Empty);
            return data.Split("§").Where(x => x.IsNotNullOrWhitespace()).ToArray();
        }

        private static void SetIgnoredNames(string[] dataArray)
        {
            var data = dataArray.Aggregate(string.Empty, (current, str) => $"{current}§{str}");
            UnityEditor.EditorPrefs.SetString($"{nameof(ScriptableObjectFactorySettings)}{nameof(ignoredNames)}", data);
        }

        internal static bool EnableMultiAssetCreation
        {
            get => UnityEditor.EditorPrefs.GetBool(
                $"{nameof(ScriptableObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}",
                false);
            set => UnityEditor.EditorPrefs.SetBool(
                $"{nameof(ScriptableObjectFactorySettings)}{nameof(EnableMultiAssetCreation)}",
                value);
        }

        internal static SearchOptions SearchOptions
        {
            get => (SearchOptions)UnityEditor.EditorPrefs.GetInt(
                $"{nameof(ScriptableObjectFactory)}{nameof(SearchOptions)}",
                (int)(SearchOptions.AssemblyName | SearchOptions.CreateAttributePath | SearchOptions.BaseTypes));
            set => UnityEditor.EditorPrefs.SetInt($"{nameof(ScriptableObjectFactory)}{nameof(SearchOptions)}",
                (int)value);
        }

        #endregion


        #region Setup

        internal static void OpenWindow()
        {
            if (!isOpen)
            {
                var window = GetWindow<ScriptableObjectFactorySettings>("Settings");
                window.Show(false);
            }
        }

        private void OnEnable()
        {
            isOpen = true;
            Foldout = new FoldoutHandler(nameof(ScriptableObjectFactorySettings));

            ignoredAssemblyPrefixes = GetIgnoredAssemblyPrefixes();
            ignoredNames = GetIgnoredNames();

            _serializedObject = new UnityEditor.SerializedObject(this);
            _serializedPropertyPrefixes = _serializedObject.FindProperty(nameof(ignoredAssemblyPrefixes));
            _serializedPropertyNames = _serializedObject.FindProperty(nameof(ignoredNames));

            ScriptableObjectFactory.WindowClosed += Close;
        }

        private void OnDisable()
        {
            Save();
            isOpen = false;
            Foldout.SaveState();
            ScriptableObjectFactory.WindowClosed -= Close;
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
            UnityEditor.EditorGUI.indentLevel--;

            if (Foldout["Search Options"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIUtility.Space();
                var newSearchOptions = GUIUtility.DrawFlagsEnumAsToggle(SearchOptions, true);
                if (newSearchOptions != SearchOptions)
                {
                    changed = true;
                }
                SearchOptions = newSearchOptions;
                GUIUtility.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            if (Foldout["Misc"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIUtility.Space();

                var enableMulti = EnableMultiAssetCreation;
                var newEnableMulti = UnityEditor.EditorGUILayout.Toggle("Multi Asset Creation", enableMulti);
                if (newEnableMulti != enableMulti)
                {
                    changed = true;
                }
                EnableMultiAssetCreation = newEnableMulti;

                GUIUtility.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            if (Foldout["Assemblies"])
            {
                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.indentLevel++;
                GUIUtility.Space();

                _serializedObject.Update();
                UnityEditor.EditorGUILayout.PropertyField(_serializedPropertyPrefixes);
                UnityEditor.EditorGUILayout.PropertyField(_serializedPropertyNames);
                _serializedObject.ApplyModifiedProperties();
                GUIUtility.Space();
                UnityEditor.EditorGUI.indentLevel--;
                UnityEditor.EditorGUI.indentLevel--;
            }

            GUILayout.FlexibleSpace();
            GUIUtility.DrawLine();
            GUIUtility.Space();

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
            GUIUtility.Space();

            if (changed)
            {
                SettingsChanged?.Invoke(false);
            }
        }

        #endregion
    }
}