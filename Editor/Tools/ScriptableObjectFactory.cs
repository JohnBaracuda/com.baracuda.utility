using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baracuda.Bedrock.Reflection;
using Baracuda.Bedrock.Utilities;
using UnityEngine;
using GUIUtility = Baracuda.Utilities.Editor.Utilities.GUIUtility;
using Object = UnityEngine.Object;
using UnityEditorUtility = Baracuda.Utilities.Editor.Utilities.UnityEditorUtility;

namespace Baracuda.Utilities.Editor.Tools
{
    public class ScriptableObjectFactory : UnityEditor.EditorWindow
    {
        #region Fields & Properties

        private UnityEditorInternal.ReorderableList _displayedList;
        private List<CreatableObject> _creatableObjects;
        private readonly List<CreatableObject> _filteredCreatableObjects = new(100);
        private string _searchFilter = string.Empty;
        private string _fileName = string.Empty;
        private Vector2 _scrollPos;
        private int _amountToCreate = 1;
        private const int ColumnWidth = 330;
        private const int MaxAmount = 100;
        private const int MinAmount = 1;
        private bool _setInitialFocus = true;
        private bool _isReady;
        private bool _enableInputCheck = true;
        private bool _isMultiSelect;
        private bool _resetFocusBuffer;

        internal static event Action WindowClosed;

        public static event Action<IEnumerable<Object>> AssetsCreated;

        #endregion


        #region Setup

        [UnityEditor.MenuItem("Assets/InstantiatePrefab/ScriptableObject %&s", priority = -100)]
        public static void OpenWindow()
        {
            var window = GetWindow<ScriptableObjectFactory>("Object Factory");
            window.Show(false);
        }

        public static void OpenWindow(string activeFilter)
        {
            var window = GetWindow<ScriptableObjectFactory>("Object Factory");
            window.Show(false);
            window._searchFilter = activeFilter;
        }

        private void OnEnable()
        {
            _searchFilter =
                UnityEditor.EditorPrefs.GetString($"{nameof(ScriptableObjectFactory)}{nameof(_searchFilter)}",
                    _searchFilter);
            ScriptableObjectFactorySettings.SettingsChanged += OnSettingsChanged;
            Initialize();
        }

        private void OnDisable()
        {
            UnityEditor.EditorPrefs.SetString($"{nameof(ScriptableObjectFactory)}{nameof(_searchFilter)}",
                _searchFilter);
            ScriptableObjectFactorySettings.SettingsChanged -= OnSettingsChanged;
            WindowClosed?.Invoke();
        }

        #endregion


        #region Setup Profiling

        private void OnSettingsChanged(bool refresh)
        {
            if (refresh)
            {
                Initialize();
            }
            else
            {
                Repaint();
            }
        }

        private async void Initialize()
        {
            try
            {
                _isReady = false;
                _amountToCreate = MinAmount;
                _displayedList = new UnityEditorInternal.ReorderableList(new List<CreatableObject>(), typeof(CreatableObject), false, false,
                    false, false);
                _displayedList.drawElementCallback += DrawElementCallback;
                _displayedList.onSelectCallback += OnSelectCallback;
                _displayedList.multiSelect = true;
                _creatableObjects = await ProfileAssemblies();
                _isReady = true;
                _setInitialFocus = true;
                _enableInputCheck = true;
                OnSelectCallback(_displayedList);
                Repaint();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void OnSelectCallback(UnityEditorInternal.ReorderableList reorderable)
        {
            _isMultiSelect = reorderable.selectedIndices.Count > 1;

            if (_isMultiSelect || reorderable.list.Count == 0 || reorderable.list.Count <= reorderable.index)
            {
                return;
            }

            var clicked = (CreatableObject)reorderable.list[reorderable.index];
            _fileName = clicked.DefaultFileName;

            if (_isMouseEvent && GUIUtility.IsDoubleClick(clicked))
            {
                CreateAssets();
            }
        }

        private static async ValueTask<List<CreatableObject>> ProfileAssemblies()
        {
            var assemblies =
                AssemblyProfiler.GetFilteredAssemblies(null,
                    ScriptableObjectFactorySettings.GetIgnoredAssemblyPrefixes());
            var ignoreNames = ScriptableObjectFactorySettings.GetIgnoredNames();

            var result = await Task.Run(() =>
            {
                var creatable = new List<CreatableObject>(100);
                for (var i = 0; i < assemblies.Length; i++)
                {
                    var assembly = assemblies[i];
                    var assemblyTypes = assembly.GetTypes();

                    creatable.AddRange(from type in assemblyTypes
                        where IsTypeValidForCreation(type, ignoreNames)
                        select new CreatableObject(type));
                }

                return creatable;
            });

            return result;
        }

        private static bool IsTypeValidForCreation(Type type, string[] ignoreNames)
        {
            if (!type.IsSubclassOrAssignable(typeof(ScriptableObject)))
            {
                return false;
            }

            if (type.IsAbstract)
            {
                return false;
            }

            if (type.IsGenericType)
            {
                return false;
            }

            if (type.IsSubclassOrAssignable(typeof(UnityEditor.EditorWindow)))
            {
                return false;
            }

            if (type.IsSubclassOrAssignable(typeof(UnityEditor.Editor)))
            {
                return false;
            }

            if (type.HasAttribute<ExcludeFromObjectFactoryAttribute>())
            {
                return false;
            }

            if (ignoreNames.Contains(type.Name))
            {
                return false;
            }

            return true;
        }

        #endregion


        #region GUI List

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var creatable = (CreatableObject)_displayedList.list[index];
            var lineRect = new Rect(0, rect.y, GUIUtility.GetViewWidth(), 1);
            UnityEditor.EditorGUI.DrawRect(lineRect, new Color(0f, 0f, 0f, 0.2f));

            if (index.IsEven())
            {
                var backgroundRect = new Rect(0, rect.y, GUIUtility.GetViewWidth(), rect.height);
                UnityEditor.EditorGUI.DrawRect(backgroundRect, new Color(0f, 0f, 0f, 0.03f));
            }

            if (_displayedList.IsSelected(index))
            {
                var selectionRect = new Rect(0, rect.y, 3, rect.height);
                UnityEditor.EditorGUI.DrawRect(selectionRect, new Color(0.7f, 1f, 0.75f, 0.9f));
            }

            UnityEditor.EditorGUI.LabelField(rect, creatable.ToString());

            var rectOffset = ColumnWidth;

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.AssemblyName))
            {
                var assemblyRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(assemblyRect, creatable.AssemblyName);
                rectOffset += ColumnWidth;
            }

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.CreateAttributePath))
            {
                var attributeRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(attributeRect, creatable.CreateAssetPath);
                rectOffset += ColumnWidth;
            }

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.BaseTypes))
            {
                var tagRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(tagRect, creatable.BaseTypes);
            }
        }

        private void DrawColumnDescriptions()
        {
            var rectOffset = ColumnWidth;
            var rect = GUIUtility.GetControlRect();
            UnityEditor.EditorGUI.LabelField(rect, "Name");

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.AssemblyName))
            {
                var assemblyRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(assemblyRect, "Assembly Name");
                rectOffset += ColumnWidth;
            }

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.CreateAttributePath))
            {
                var attributeRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(attributeRect, "InstantiatePrefab Asset Path");
                rectOffset += ColumnWidth;
            }

            if (ScriptableObjectFactorySettings.SearchOptions.HasFlagFast(SearchOptions.BaseTypes))
            {
                var tagRect = new Rect(rect.x + rectOffset, rect.y, rect.width - rectOffset, rect.height);
                UnityEditor.EditorGUI.LabelField(tagRect, "BaseTypes");
            }
        }

        #endregion


        #region GUI

        private void OnGUI()
        {
            _isMouseEvent = Event.current.isMouse;
            GUI.enabled = _isReady;

            DrawHeader();
            DrawBody();
            DrawFooter();
            HandleInput();

            if (_resetFocusBuffer)
            {
                if (_displayedList.list.Count > 0)
                {
                    _displayedList.Select(0);
                    OnSelectCallback(_displayedList);
                }

                _resetFocusBuffer = false;
            }
        }

        private void DrawBody()
        {
            if (_isReady)
            {
                _displayedList.list = GetFilteredList(_searchFilter);

                var index = _displayedList.index;
                var count = _displayedList.list.Count;
                if (count > 0 && index < count)
                {
                    if (_displayedList.IsSelected(index) is false)
                    {
                        _displayedList.Select(0);
                        OnSelectCallback(_displayedList);
                    }

                    GUIUtility.BoldLabel(_displayedList.list[index].ToString());
                    GUIUtility.DrawLine(new Color(0f, 0f, 0f, 0.3f));
                    DrawColumnDescriptions();
                }

                _scrollPos = UnityEditor.EditorGUILayout.BeginScrollView(_scrollPos);
                _displayedList.DoLayoutList();
                UnityEditor.EditorGUILayout.EndScrollView();
            }
        }

        private void ResetFocus()
        {
            Repaint();
            _resetFocusBuffer = true;
        }

        private void SelectNext()
        {
            var index = _displayedList.index;
            var count = _displayedList.list.Count;
            if (count > 0 && index < count)
            {
                if (_displayedList.IsSelected(index) is false)
                {
                    _displayedList.Select(0);
                    OnSelectCallback(_displayedList);
                }
                else
                {
                    _displayedList.Select(index + 1);
                    OnSelectCallback(_displayedList);
                }
            }

            Repaint();
        }

        private void SelectPrevious()
        {
            var index = _displayedList.index;
            var count = _displayedList.list.Count;
            if (count > 0 && index < count)
            {
                if (_displayedList.IsSelected(index) is false)
                {
                    _displayedList.Select(0);
                    OnSelectCallback(_displayedList);
                }
                else if (index > 0)
                {
                    _displayedList.Select(index - 1);
                    OnSelectCallback(_displayedList);
                }
            }

            Repaint();
        }

        private async void SetInputTimeout()
        {
            _enableInputCheck = false;
            await Task.Delay(35);
            _enableInputCheck = true;
            Repaint();
        }

        private bool _isMouseEvent;

        private void HandleInput()
        {
            if (!_isReady)
            {
                return;
            }

            if (!Event.current.isKey)
            {
                return;
            }

            if (!_enableInputCheck)
            {
                return;
            }

            SetInputTimeout();

            var keyCode = Event.current.keyCode;

            if (keyCode == KeyCode.DownArrow)
            {
                SelectNext();
                return;
            }

            if (keyCode == KeyCode.UpArrow)
            {
                SelectPrevious();
                return;
            }

            if (keyCode == KeyCode.Return)
            {
                CreateAssets();
            }
        }

        private void CreateAssets()
        {
            try
            {
                var createdObjects = new List<Object>(_displayedList.selectedIndices.Count);
                foreach (var index in _displayedList.selectedIndices)
                {
                    createdObjects.AddRange(CreateAssetInternal(index));
                }

                UnityEditor.Selection.objects = createdObjects.ToArray();
                AssetsCreated?.Invoke(createdObjects);
                Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ResetFocus();
            }
        }

        private IEnumerable<Object> CreateAssetInternal(int index)
        {
            if (!_displayedList.IsSelected(index))
            {
                Debug.Log(index + " is not selected");
                return null;
            }

            _amountToCreate = Mathf.Clamp(_amountToCreate, MinAmount, MaxAmount);
            var creatableObject = (CreatableObject)_displayedList.list[index];
            var path = UnityEditorUtility.GetCurrentAssetDirectory();

            if (_amountToCreate > 10)
            {
                var result = UnityEditor.EditorUtility.DisplayDialog("Caution",
                    $"You are about to create {_amountToCreate} {creatableObject.Type.Name} assets at <>{path}",
                    "Confirm", "Cancel");

                if (!result)
                {
                    return null;
                }
            }

            return creatableObject.Create(path, _isMultiSelect ? creatableObject.DefaultFileName : _fileName,
                _amountToCreate);
        }

        private void DrawHeader()
        {
            GUIUtility.Space();
            UnityEditor.EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName(nameof(_searchFilter));
            var newFilter = GUIUtility.SearchBar(_searchFilter);
            if (newFilter != _searchFilter)
            {
                ResetFocus();
            }

            _searchFilter = newFilter;
            if (_setInitialFocus)
            {
                UnityEditor.EditorGUI.FocusTextInControl(nameof(_searchFilter));
                _setInitialFocus = false;
            }

            if (GUIUtility.OptionsButton())
            {
                ScriptableObjectFactorySettings.OpenWindow();
            }

            if (GUIUtility.RefreshButton())
            {
                Initialize();
            }

            UnityEditor.EditorGUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            GUIUtility.DrawLine();
            GUILayout.BeginHorizontal();
            GUIUtility.BoldLabel(UnityEditorUtility.GetCurrentAssetDirectory());

            if (!_isMultiSelect)
            {
                if (ScriptableObjectFactorySettings.EnableMultiAssetCreation)
                {
                    GUILayout.Label("Amount", GUILayout.Width(50));
                    _amountToCreate =
                        UnityEditor.EditorGUILayout.IntSlider(_amountToCreate, MinAmount, MaxAmount,
                            GUILayout.Width(150));
                }

                GUILayout.Label("Filename: ", GUILayout.Width(60));
                _fileName = UnityEditor.EditorGUILayout.TextField(_fileName, GUILayout.Width(240));
            }

            if (GUILayout.Button("InstantiatePrefab", GUILayout.Width(120)))
            {
                CreateAssets();
            }

            GUILayout.EndHorizontal();
        }

        private IList GetFilteredList(string filter)
        {
            if (filter.IsNotNullOrWhitespace())
            {
                _filteredCreatableObjects.Clear();

                for (var i = 0; i < _creatableObjects.Count; i++)
                {
                    var creatable = _creatableObjects[i];
                    if (creatable.IsValidForFilter(filter.NoSpace(), ScriptableObjectFactorySettings.SearchOptions))
                    {
                        _filteredCreatableObjects.Add(creatable);
                    }
                }

                return _filteredCreatableObjects;
            }

            return _creatableObjects;
        }

        #endregion
    }
}