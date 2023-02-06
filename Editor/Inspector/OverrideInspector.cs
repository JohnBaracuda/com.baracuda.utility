using Baracuda.Utilities.Helper;
using Baracuda.Utilities.Inspector.InspectorFields;
using Baracuda.Utilities.Pooling;
using Baracuda.Utilities.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Inspector
{
    public class OverrideInspector<TObject> : StyledInspector<TObject> where TObject : Object
    {
        #region Fields

        private SerializedProperty _script;
        private string _filterString;
        private bool _useDefaultInspector;
        private bool _hideMonoScript;
        private bool _showFilterField;
        private bool _hasDescription;
        private GUIContent _description;
        private GUIContent _descriptionTitle;
        private readonly Dictionary<FoldoutData, List<InspectorMember>> _headerFields = new(10);
        private readonly List<InspectorMember> _headerLessFields = new(8);

        private string Filter => InspectorSearch.IsActive ? InspectorSearch.Filter : _filterString;

        #endregion

        #region Setup

        protected override void OnEnable()
        {
            if (target == null || serializedObject.targetObject == null)
            {
                _useDefaultInspector = true;
                return;
            }


            var targetType = target.GetType();
            var foldoutInspectorAttribute = targetType.GetCustomAttribute<DefaultInspectorAttribute>(true);
            _useDefaultInspector = foldoutInspectorAttribute != null;

            if (_useDefaultInspector)
            {
                return;
            }

            base.OnEnable();

            _script = serializedObject.FindProperty("m_Script");
            _hideMonoScript = targetType.HasAttribute<HideMonoScriptAttribute>(true);

            if (targetType.TryGetCustomAttribute<DescriptionAttribute>(out var descriptionAttribute))
            {
                _description = new GUIContent(descriptionAttribute.Description, descriptionAttribute.Description);
                _descriptionTitle = new GUIContent("Description", descriptionAttribute.Description);
                _hasDescription = true;
            }

            FoldoutData activeHeader = null;

            var inspectorMembers = InspectorFieldUtils.GetInspectorMembers(serializedObject);
            var count = 0;
            for (var i = 0; i < inspectorMembers.Length; i++)
            {
                var inspectorMember = inspectorMembers[i];

                if (inspectorMember.Member.TryGetCustomAttribute<FoldoutAttribute>(out var attribute))
                {
                    var title = attribute.FoldoutName switch
                    {
                        FoldoutName.ObjectName => Target.name,
                        FoldoutName.HumanizedObjectName => Target.name.Humanize(),
                        _ => attribute.Title
                    };

                    activeHeader = new FoldoutData(title, attribute.ToolTip);
                    var defaultState = attribute.Unfold;
                    if (!_headerFields.TryAdd(activeHeader, new List<InspectorMember> {inspectorMember}))
                    {
                        _headerFields[activeHeader].Add(inspectorMember);
                    }

                    SetDefaultFoldoutState(activeHeader, defaultState);

                    count++;
                }
                else
                {
                    if (activeHeader.Title == null)
                    {
                        _headerLessFields.Add(inspectorMember);
                        count++;
                        continue;
                    }

                    _headerFields[activeHeader].Add(inspectorMember);
                    count++;
                }
            }

            _showFilterField =
                targetType.TryGetCustomAttribute<SearchField>(out var searchAttribute)
                    ? searchAttribute.Enabled
                    : count > 8;
        }

        protected override void OnDisable()
        {
            if (_useDefaultInspector)
            {
                return;
            }

            base.OnDisable();
        }

        #endregion

        #region GUI

        public override void OnInspectorGUI()
        {
            if (_useDefaultInspector)
            {
                base.OnInspectorGUI();
                return;
            }

            GUIHelper.Space(!InspectorSearch.IsActive);

            if (_hasDescription)
            {
                DrawDescription();
                GUIHelper.Space();
            }

            if (!_hideMonoScript && !InspectorSearch.IsActive)
            {
                DrawScriptField();
                GUIHelper.Space();
            }

            if (_showFilterField && !InspectorSearch.IsActive)
            {
                _filterString = GUIHelper.SearchBar(_filterString);
                GUIHelper.Space();
            }

            DrawMember();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(Target);
            }
        }

        private void DrawMember()
        {
            if (!InspectorSearch.IsActiveByContextQuery() && Filter.IsNotNullOrWhitespace())
            {
                DrawFilteredMember(Filter);
                return;
            }
            DrawAllMember();
        }

        private void DrawFilteredMember(string filter)
        {
            serializedObject.Update();

            var pooledList = ListPool<InspectorMember>.Get();

            foreach (var member in _headerLessFields)
            {
                if (member.Label.text.ContainsIgnoreCaseAndSpace(filter))
                {
                    pooledList.Add(member);
                }
            }


            var pooledDictionary = DictionaryPool<FoldoutData, List<InspectorMember>>.Get();

            foreach (var (header, list) in _headerFields)
            {
                if (header.Title.ContainsIgnoreCaseAndSpace(filter))
                {
                    if (!pooledDictionary.TryAdd(header, list))
                    {
                        pooledDictionary[header].AddRange(list);
                    }

                    continue;
                }

                foreach (var member in list)
                {
                    if (member.Label.text.ContainsIgnoreCaseAndSpace(filter))
                    {
                        if (pooledDictionary.ContainsKey(header))
                        {
                            pooledDictionary[header].Add(member);
                            continue;
                        }

                        if (!pooledDictionary.TryAdd(header, new List<InspectorMember>() {member}))
                        {
                            pooledDictionary[header].Add(member);
                            continue;
                        }
                    }
                }
            }

            if (pooledList.Any() && InspectorSearch.IsActive)
            {
                GUIHelper.Space();
            }
            foreach (var member in pooledList)
            {
                member.ProcessGUI();
            }
            if (pooledList.Any())
            {
                GUIHelper.Space();
            }

            foreach (var (header, list) in pooledDictionary)
            {
                Foldout.ForceHeader(header);
                if (!(list.First()?.HasHeaderAttribute ?? false))
                {
                    GUIHelper.Space();
                }
                foreach (var member in list)
                {
                    member.ProcessGUI();
                }
                GUIHelper.Space();
            }

            DictionaryPool<FoldoutData, List<InspectorMember>>.Release(pooledDictionary);
            ListPool<InspectorMember>.Release(pooledList);
            
            serializedObject.ApplyModifiedProperties();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawAllMember()
        {
            serializedObject.Update();

            if (_headerLessFields.Any() && InspectorSearch.IsActive)
            {
                GUIHelper.Space();
            }

            for (var i = 0; i < _headerLessFields.Count; i++)
            {
                _headerLessFields[i].ProcessGUI();
            }

            if (_headerLessFields.Any())
            {
                GUIHelper.Space();
            }

            foreach (var (header, list) in _headerFields)
            {
                if (Foldout[header])
                {
                    if (!(list.First()?.HasHeaderAttribute ?? false))
                    {
                        GUIHelper.Space();
                    }
                    foreach (var member in list)
                    {
                        member.ProcessGUI();
                    }

                    GUIHelper.Space();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawScriptField()
        {
            try
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                if (_script != null)
                {
                    EditorGUILayout.PropertyField(_script);
                }
                GUI.enabled = enabled;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void DrawDescription()
        {
            EditorGUILayout.LabelField(_descriptionTitle, _description);
        }

        #endregion

        #region Save & Load State

        protected override void SaveStateData(string editorPrefsKey)
        {
            base.SaveStateData(editorPrefsKey);
            EditorPrefs.SetString($"{nameof(_filterString)}{editorPrefsKey}", _filterString);
        }

        protected override void LoadStateData(string editorPrefsKey)
        {
            base.LoadStateData(editorPrefsKey);
            _filterString = EditorPrefs.GetString($"{nameof(_filterString)}{editorPrefsKey}");
        }

        #endregion
    }
}