using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Baracuda.Utility.Collections;
using Baracuda.Utility.Editor.Drawer;
using Baracuda.Utility.Reflection;
using Baracuda.Utility.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using GUIUtility = Baracuda.Utility.Editor.Utilities.GUIUtility;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Editor.Tools
{
    public abstract class AssetCollectionWindow : UnityEditor.EditorWindow
    {
        #region Fields & Properties

        protected float LabelWidth { get; set; } = 260;
        protected float MenuWidth { get; set; } = 220;
        protected int IndentLevel { get; set; } = 1;

        private Vector2 _scrollLeft;
        private Vector2 _scrollRight;
        private ActiveSelection _selection;
        private string _filterKey;
        private string _foldoutKey;
        private string _selectionKey;
        private string _filterString = string.Empty;
        private bool _foldout = true;
        private string _createSearchFilter;
        private bool _isRenameMode;
        private string _elementName;
        private int _elementCount;
        private Action<Object> _onEnter;

        private readonly Dictionary<Type, Action<Object>> _footerDrawer = new(8);
        private readonly List<(string title, UnityEditorInternal.ReorderableList orderedList, IList rawList, ValueObject<bool> show)>
            _assetCollections = new(8);

        private class ActiveSelection
        {
            public readonly UnityEditor.SerializedObject SerializedObject;
            public readonly Object Target;
            public readonly string Path;

            private readonly UnityEditor.Editor _editor;
            private readonly Action _drawInspectorGUI;

            public void DrawHeader()
            {
                UnityEditor.EditorGUIUtility.labelWidth = 0;
                GUIUtility.BeginIndentOverride(0);
                _editor.DrawHeader();
                GUIUtility.EndIndentOverride();
            }

            public void DrawInspectorGUI()
            {
                _drawInspectorGUI();
            }

            public ActiveSelection(Object target)
            {
                Target = target;
                SerializedObject = new UnityEditor.SerializedObject(Target);
                _editor = UnityEditor.Editor.CreateEditor(Target);

                if (Target is GameObject gameObject)
                {
                    var targets = new List<(string name, UnityEditor.Editor editor)>();
                    var foldout = new FoldoutHandler(color: new Color(0f, 0f, 0f, 0.27f));
                    foreach (var component in gameObject.GetComponents(typeof(Component)))
                    {
                        targets.Add((component.GetType().Name.Humanize(), UnityEditor.Editor.CreateEditor(component)));
                    }
                    _drawInspectorGUI = () =>
                    {
                        GUIUtility.Space();
                        for (var i = 0; i < targets.Count; i++)
                        {
                            if (foldout[targets[i].name])
                            {
                                GUIUtility.Space();
                                targets[i].editor.OnInspectorGUI();
                                GUIUtility.Space();
                            }
                        }
                    };
                }
                else
                {
                    _drawInspectorGUI = _editor.OnInspectorGUI;
                }
                Path = UnityEditor.AssetDatabase.GetAssetPath(target);
            }
        }

        private class ValueObject<T>
        {
            public T Value;

            public ValueObject(T value)
            {
                Value = value;
            }

            public static implicit operator ValueObject<T>(T value)
            {
                return new ValueObject<T>(value);
            }

            public static implicit operator T(ValueObject<T> valueObject)
            {
                return valueObject.Value;
            }
        }

        #endregion


        #region API

        protected void AddAssetCollection<TObject>(string collectionTitle, List<TObject> list,
            string emptyCollectionDisplay)
            where TObject : Object
        {
            AddAssetCollectionInternal<TObject>(collectionTitle, list, null, emptyCollectionDisplay);
        }

        protected void AddAssetCollection<TObject>(string collectionTitle, List<TObject> list,
            Action<Object> drawFooter = null)
            where TObject : Object
        {
            list.RemoveDuplicates();
            AddAssetCollectionInternal<TObject>(collectionTitle, list, drawFooter);
        }

        protected void SetDefaultFilterString(string filter)
        {
            _createSearchFilter = filter;
        }

        protected void AddOnEnterAction(Action<Object> action)
        {
            _onEnter = action;
        }

        #endregion


        #region Virtual & Abstract

        protected abstract void Initialize();

        #endregion


        #region Setup

        private void OnEnable()
        {
            _filterKey = nameof(_filterString) + GetType().Name;
            _selectionKey = nameof(_selectionKey) + GetType().Name;
            _foldoutKey = nameof(_foldoutKey) + GetType().Name;
            InitializeInternal();
            ScriptableObjectFactory.AssetsCreated -= OnAssetsCreated;
            ScriptableObjectFactory.AssetsCreated += OnAssetsCreated;
        }

        private void OnDisable()
        {
            ScriptableObjectFactory.AssetsCreated -= OnAssetsCreated;
            UnityEditor.EditorPrefs.SetString(_filterKey, _filterString);
            UnityEditor.EditorPrefs.SetBool(_foldoutKey, _foldout);
        }

        private void InitializeInternal()
        {
            GUI.FocusControl(null);
            _assetCollections.Clear();
            _footerDrawer.Clear();
            _selection = default;
            _isRenameMode = false;
            _onEnter = null;
            _elementCount = 0;
            _foldout = UnityEditor.EditorPrefs.GetBool(_foldoutKey, true);
            _filterString = UnityEditor.EditorPrefs.GetString(_filterKey, _filterString);

            Initialize();

            var lastSelected = UnityEditor.EditorPrefs.GetString(_selectionKey);
            foreach (var (_, reorderableList, _, _) in _assetCollections)
            {
                _elementCount += reorderableList.list.Count;

                for (var i = 0; i < reorderableList.list.Count; i++)
                {
                    if (((Object)reorderableList.list[i]).name == lastSelected)
                    {
                        reorderableList.Select(i);
                        foreach (var tuple in _assetCollections)
                        {
                            if (tuple.orderedList == reorderableList)
                            {
                                continue;
                            }

                            tuple.orderedList.Deselect(tuple.orderedList.index);
                        }

                        var target = (Object)reorderableList.list[reorderableList.index];
                        _selection = new ActiveSelection(target);
                        UnityEditor.EditorPrefs.SetString(_selectionKey, target.name);
                        return;
                    }
                }
            }
            foreach (var (_, reorderableList, _, _) in _assetCollections)
            {
                for (var i = 0; i < reorderableList.list.Count; i++)
                {
                    if (((Object)reorderableList.list[i]).IsNotNull())
                    {
                        reorderableList.Select(i);
                        foreach (var tuple in _assetCollections)
                        {
                            if (tuple.orderedList == reorderableList)
                            {
                                continue;
                            }

                            tuple.orderedList.Deselect(tuple.orderedList.index);
                        }

                        var target = (Object)reorderableList.list[reorderableList.index];
                        _selection = new ActiveSelection(target);
                        UnityEditor.EditorPrefs.SetString(_selectionKey, target.name);
                        return;
                    }
                }
            }
        }

        private void OnAssetsCreated(IEnumerable<Object> objects)
        {
            UnityEditor.EditorPrefs.SetString(_selectionKey, objects.FirstOrDefault()?.name);
            InitializeInternal();
        }

        #endregion


        #region List Setup

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddAssetCollectionInternal<T>(string collectionTitle, IList list, Action<Object> drawFooter,
            string emptyCollectionDisplay = null) where T : Object
        {
            _elementCount += list.Count;
            var orderedList = new UnityEditorInternal.ReorderableList(list, typeof(T), false, false, false, false)
            {
                elementHeight = 30,
                footerHeight = 0
            };

            orderedList.onMouseUpCallback += reorderableList =>
            {
                var clicked = (Object)reorderableList.list[reorderableList.index];

                if (clicked == _selection.Target && GUIUtility.IsDoubleClick(clicked))
                {
                    try
                    {
                        UnityEditor.Selection.activeObject = _selection.Target;
                        UnityEditor.EditorGUIUtility.PingObject(_selection.Target);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            };

            orderedList.onSelectCallback += reorderableList =>
            {
                foreach (var tuple in _assetCollections)
                {
                    if (tuple.orderedList == reorderableList)
                    {
                        continue;
                    }

                    tuple.orderedList.Deselect(tuple.orderedList.index);
                }

                var target = (Object)reorderableList.list[reorderableList.index];
                if (_selection.Target != target)
                {
                    _isRenameMode = false;
                }
                _selection = new ActiveSelection(target);
                _elementName = target.name;
                UnityEditor.EditorPrefs.SetString(_selectionKey, target.name);
            };

            orderedList.drawNoneElementCallback += rect => { GUI.Label(rect, emptyCollectionDisplay); };

            orderedList.drawElementCallback += (rect, index, active, focused) =>
            {
                orderedList.elementHeight = 30;
                var element = (Object)orderedList.list[index];
                if (element == null)
                {
                    return;
                }

                var iconRect = new Rect(rect.x - 2, rect.y + 4, 26, 26);
                if (element is Component component)
                {
                    GUI.Label(iconRect,
                        UnityEditor.EditorGUIUtility.ObjectContent(component.gameObject, component.gameObject.GetType())
                            .image);
                }
                else
                {
                    GUI.Label(iconRect, UnityEditor.EditorGUIUtility.ObjectContent(element, element.GetType()).image);
                }

                if (active && _isRenameMode)
                {
                    var labelRect = new Rect(rect.x + 30, rect.y + 7, rect.width - 30, rect.height - 14);
                    GUI.SetNextControlName("textField");
                    _elementName = GUI.TextField(labelRect, _elementName);
                    UnityEditor.EditorGUI.FocusTextInControl("textField");
                }
                else
                {
                    var labelRect = new Rect(rect.x + 30, rect.y, rect.width - 30, rect.height);
                    GUI.Label(labelRect, element.name);
                }
            };

            _assetCollections.Add((collectionTitle, orderedList, list, true));
            _footerDrawer.TryAdd(typeof(T), drawFooter);
        }

        #endregion


        #region GUI

        private void OnGUI()
        {
            DrawLeftSide();
            DrawRightSide();
            HandleInput();
        }

        private void DrawLeftSide()
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.BeginVertical(GUILayout.Height(position.height - 4),
                GUILayout.Width(GetMenuWidth()));
            {
                GUIUtility.Space(2);
                UnityEditor.EditorGUILayout.BeginHorizontal();

                if (GUIUtility.OptionsButton())
                {
                    _foldout = !_foldout;
                }

                if (_foldout)
                {
                    if (GUIUtility.RefreshButton())
                    {
                        InitializeInternal();
                        _filterString = string.Empty;
                    }

                    if (GUIUtility.SelectButton())
                    {
                        try
                        {
                            UnityEditor.Selection.activeObject = _selection.Target;
                            UnityEditor.EditorGUIUtility.PingObject(_selection.Target);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    UnityEditor.EditorGUILayout.EndHorizontal();
                    if (_elementCount > 5)
                    {
                        GUIUtility.Space();
                        _filterString = GUIUtility.SearchBar(_filterString);
                    }

                    _scrollLeft = UnityEditor.EditorGUILayout.BeginScrollView(_scrollLeft);

                    foreach (var assetCollection in _assetCollections)
                    {
                        if (assetCollection.rawList.Count <= 0)
                        {
                            continue;
                        }

                        if (_filterString.IsNotNullOrWhitespace())
                        {
                            var tempList = ListPool<Object>.Get();
                            foreach (var item in assetCollection.rawList)
                            {
                                var obj = (Object)item;
                                if (IsValidForFilterString(obj, _filterString))
                                {
                                    tempList.Add(obj);
                                }

                                static bool IsValidForFilterString(Object obj, string filter)
                                {
                                    if (obj.name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return true;
                                    }
                                    if (obj.GetType().Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                                    {
                                        return true;
                                    }
                                    if (obj.GetType().BaseType?.Name
                                            .Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)
                                    {
                                        return true;
                                    }

                                    return false;
                                }
                            }

                            assetCollection.orderedList.list = tempList;
                            assetCollection.orderedList.DoLayoutList();
                            ListPool<Object>.Release(tempList);
                        }
                        else
                        {
                            assetCollection.show.Value =
                                GUIUtility.DynamicFoldout(assetCollection.show, assetCollection.title);
                            if (!assetCollection.show)
                            {
                                continue;
                            }
                            assetCollection.orderedList.list = assetCollection.rawList;
                            assetCollection.orderedList.DoLayoutList();
                        }
                    }

                    UnityEditor.EditorGUILayout.EndScrollView();

                    GUIUtility.DrawLine();
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    if (GUIUtility.AddButton())
                    {
                        ScriptableObjectFactory.OpenWindow(_createSearchFilter);
                    }
                    if (GUIUtility.RemoveButton())
                    {
                        if (GUIUtility.DestroyDialogue(_selection.Target))
                        {
                            UnityEditor.EditorGUILayout.EndHorizontal();
                            UnityEditor.EditorGUILayout.EndVertical();
                            InitializeInternal();
                            return;
                        }
                    }
                    UnityEditor.EditorGUILayout.EndHorizontal();
                }
                else
                {
                    UnityEditor.EditorGUILayout.EndHorizontal();
                }
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }

        private void DrawRightSide()
        {
            GUILayout.FlexibleSpace();
            GUIUtility.DrawRect(new Rect(GetMenuWidth() - 1, 0, 1, position.height));
            UnityEditor.EditorGUILayout.BeginVertical(GUILayout.Width(position.width - GetMenuWidth()));

            if (_selection != null && _selection.Target != null)
            {
                _scrollRight = UnityEditor.EditorGUILayout.BeginScrollView(_scrollRight);
                UnityEditor.EditorGUI.indentLevel = IndentLevel;
                UnityEditor.EditorGUIUtility.labelWidth = LabelWidth;
                _selection.SerializedObject.Update();
                _selection.DrawHeader();
                _selection.DrawInspectorGUI();
                _selection.SerializedObject.ApplyModifiedProperties();
                FoldoutHandler.SetDirty();
                UnityEditor.EditorGUILayout.EndScrollView();
                GUIUtility.DrawLine();
                GUIUtility.Space(4);
                GetFooterDrawer(_selection.Target)?.Invoke(_selection.Target);
                GUI.enabled = false;
                UnityEditor.EditorGUILayout.LabelField(_selection.Path);
                GUI.enabled = true;
                var buttonRect = GUILayoutUtility.GetLastRect();
                buttonRect.x += buttonRect.width - 60;
                buttonRect.width = 60;
                GUIUtility.Space(7);
            }
            else
            {
                InitializeInternal();
            }

            UnityEditor.EditorGUILayout.EndVertical();
            UnityEditor.EditorGUILayout.EndHorizontal();
            GUIUtility.DrawRect(new Rect(0, 0, position.width, 1));
        }

        private void HandleInput()
        {
            var current = Event.current;

            if (_isRenameMode)
            {
                if (current.keyCode == KeyCode.Return && current.type == EventType.KeyUp)
                {
                    UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(_selection.Target),
                        _elementName);
                    Repaint();
                    _isRenameMode = false;
                    return;
                }
                if (current.isMouse)
                {
                    UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(_selection.Target),
                        _elementName);
                    Repaint();
                    _isRenameMode = false;
                    return;
                }
                if (current.keyCode == KeyCode.Escape)
                {
                    Repaint();
                    _isRenameMode = false;
                }
            }
            else
            {
                if (current.keyCode == KeyCode.Delete && current.type == EventType.KeyUp)
                {
                    if (GUIUtility.DestroyDialogue(_selection.Target))
                    {
                        InitializeInternal();
                        return;
                    }
                }

                if (current.keyCode == KeyCode.Return && current.type == EventType.KeyUp)
                {
                    _onEnter?.Invoke(_selection.Target);
                    return;
                }

                if (current.keyCode == KeyCode.F2)
                {
                    _isRenameMode = true;
                    Repaint();
                }
            }
        }

        #endregion


        #region Helper

        private float GetMenuWidth()
        {
            return _foldout ? MenuWidth : 40f;
        }

        private Action<Object> GetFooterDrawer(object target)
        {
            var type = target.GetType();
            foreach (var (key, value) in _footerDrawer)
            {
                if (type.IsSubclassOrAssignable(key))
                {
                    return value;
                }
            }

            return null;
        }

        #endregion
    }
}