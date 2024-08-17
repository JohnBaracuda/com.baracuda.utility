using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baracuda.Utilities.Editor.Tools
{
    public class FindAllReferences : UnityEditor.EditorWindow
    {
        private static Object selectedObj;
        private static string selectedObjPath;
        private static bool selectedIsPrefab;
        private static bool selectedIsMainAsset;
        private static bool isFindingGameObject;
        private static bool isFindingAsset;
        private static readonly List<Object> selectedObjects = new();

        private static readonly Dictionary<Object, List<HierarchyResult>> hierarchyResults = new();
        private static readonly Dictionary<Object, List<ProjectResult>> projectResults = new();

        private GUIContent _hierarchyLabelIcon;
        private GUIContent _sceneLabelIcon;
        private GUIContent _projectLabelIcon;
        private GUIStyle _headerStyle;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private const float LabelTextOffset = 10;
        private Vector2 _scrollPosition;
        private bool _redraw;
        private static int progress;
        private static int totalCount;


        #region MenuItems

        [UnityEditor.MenuItem("GameObject/\u273F Find All References", true)]
        private static bool FindInHierarchyValidate()
        {
            return UnityEditor.Selection.activeGameObject != null;
        }

        [UnityEditor.MenuItem("GameObject/\u273F Find All References", false, 20)]
        private static void FindInHierarchy()
        {
            //selected gameObject and its components
            selectedObj = UnityEditor.Selection.activeGameObject;
            selectedObjects.Clear();
            selectedObjects.Add(selectedObj);
            selectedObjects.AddRange(UnityEditor.Selection.activeGameObject.GetComponents<Component>());

            isFindingGameObject = true;
            isFindingAsset = false;

            hierarchyResults.Clear();
            projectResults.Clear();

            CheckScene(SceneManager.GetActiveScene(), true);

            UnityEditor.EditorUtility.ClearProgressBar();

            ShowWindow();
        }

        [UnityEditor.MenuItem("Assets/\u273F Find All References", true)]
        private static bool FindInProjectValidate()
        {
            if (UnityEditor.Selection.activeObject != null)
            {
                return !(UnityEditor.Selection.activeObject is UnityEditor.DefaultAsset);
            }
            return false;
        }

        [UnityEditor.MenuItem("Assets/\u273F Find All References", false, 0)]
        private static void FindInProject()
        {
            selectedObj = UnityEditor.Selection.activeObject;
            if (UnityEditor.PrefabUtility.GetPrefabAssetType(selectedObj) != UnityEditor.PrefabAssetType.NotAPrefab)
            {
                selectedIsPrefab = true;
                if (!UnityEditor.AssetDatabase.IsMainAsset(selectedObj))
                {
                    selectedObj =
                        UnityEditor.AssetDatabase.LoadMainAssetAtPath(
                            UnityEditor.AssetDatabase.GetAssetPath(selectedObj.GetInstanceID()));
                }
            }
            selectedObjPath = UnityEditor.AssetDatabase.GetAssetPath(selectedObj);
            selectedIsMainAsset = UnityEditor.AssetDatabase.IsMainAsset(selectedObj);

            isFindingAsset = true;
            isFindingGameObject = false;

            hierarchyResults.Clear();
            projectResults.Clear();

            CheckScene(SceneManager.GetActiveScene(), true);
            CheckAssets();

            UnityEditor.EditorUtility.ClearProgressBar();

            ShowWindow();
        }

        #endregion


        #region Methods

        private static void CheckScene(Scene scene, bool inHierarchy, Object asset = null)
        {
            var allObjects = new List<GameObject>();
            allObjects.AddRange(scene.GetRootGameObjects());
            if (inHierarchy && UnityEditor.EditorApplication.isPlaying)
            {
                var tmp = new GameObject();
                DontDestroyOnLoad(tmp);
                allObjects.AddRange(tmp.scene.GetRootGameObjects());
                allObjects.Remove(tmp);
                DestroyImmediate(tmp);
            }

            if (isFindingGameObject)
            {
                totalCount = allObjects.Count;
                progress = 0;
            }

            foreach (var go in allObjects)
            {
                if (isFindingGameObject)
                {
                    UnityEditor.EditorUtility.DisplayProgressBar("Hierarchy", "Searching...",
                        1f * ++progress / totalCount);
                }

                CheckGameObject(go, inHierarchy, asset);
            }
        }

        private static void CheckGameObject(GameObject go, bool inHierarchy, Object asset = null)
        {
            if (isFindingAsset)
            {
                if (selectedIsPrefab && UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(go) == selectedObj)
                {
                    if (inHierarchy)
                    {
                        var r = new HierarchyResult
                        {
                            Tip = "<prefab>"
                        };

                        SaveHierarchyResult(go, r);
                    }
                    else
                    {
                        var childpath = go.name;
                        var parent = go.transform.parent;
                        while (parent != null)
                        {
                            childpath = parent.name + " \u27A4 " + childpath;
                            parent = parent.parent;
                        }

                        var r = new ProjectResult
                        {
                            ChildPath = childpath
                        };

                        SaveProjectResult(asset, r);
                    }
                    return;
                }
            }

            foreach (var c in go.GetComponents<Component>())
            {
                CheckReference(c, inHierarchy, asset);
            }

            foreach (Transform child in go.transform)
            {
                CheckGameObject(child.gameObject, inHierarchy, asset);
            }
        }

        private static void CheckReference(Object obj, bool inHierarchy, Object asset = null)
        {
            if (obj == null || obj is Transform)
            {
                return;
            }

            var so = new UnityEditor.SerializedObject(obj);
            var p = so.GetIterator();
            do
            {
                if (p.propertyType != UnityEditor.SerializedPropertyType.ObjectReference ||
                    p.objectReferenceValue == null ||
                    (inHierarchy && p.propertyPath == "m_GameObject"))
                {
                    continue;
                }

                var isMatched = false;

                if (isFindingGameObject)
                {
                    if (selectedObjects.Contains(p.objectReferenceValue))
                    {
                        isMatched = true;
                    }
                }
                else if (isFindingAsset)
                {
                    if (p.objectReferenceValue == selectedObj ||
                        (selectedIsMainAsset && UnityEditor.AssetDatabase.GetAssetPath(p.objectReferenceValue) ==
                            selectedObjPath))
                    {
                        isMatched = true;
                    }
                }

                if (isMatched)
                {
                    if (inHierarchy)
                    {
                        var c = obj as Component;
                        var r = new HierarchyResult
                        {
                            Component = c,
                            Property = p.objectReferenceValue,
                            PropertyName = GetPropertyName(c, p)
                        };

                        if (obj is MonoBehaviour)
                        {
                            var script = UnityEditor.MonoScript.FromMonoBehaviour(obj as MonoBehaviour);
                            var path = UnityEditor.AssetDatabase.GetAssetPath(script.GetInstanceID());
                            if (path.StartsWith("Assets/"))
                            {
                                r.Script = script;
                            }
                        }

                        if (r.Script == null && r.PropertyName.StartsWith("m_"))
                        {
                            r.PropertyName = r.PropertyName.Substring(2);
                        }

                        SaveHierarchyResult(c.gameObject, r);
                    }
                    else if (asset is UnityEditor.SceneAsset || asset is GameObject)
                    {
                        var c = obj as Component;

                        var r = new ProjectResult
                        {
                            ChildPath = GetPropertyPath(asset, c, p),
                            PropertyName = GetPropertyName(c, p),
                            ReferenceObject = p.objectReferenceValue
                        };

                        SaveProjectResult(asset, r);
                    }
                    else
                    {
                        var r = new ProjectResult
                        {
                            ChildPath = p.propertyPath,
                            PropertyName = p.displayName,
                            ReferenceObject = p.objectReferenceValue
                        };

                        SaveProjectResult(asset, r);
                    }
                }
            } while (p.Next(true));
        }

        private static void CheckAssets()
        {
            selectedObjPath = UnityEditor.AssetDatabase.GetAssetPath(selectedObj.GetInstanceID());
            var selectedGuid = UnityEditor.AssetDatabase.AssetPathToGUID(selectedObjPath);

            //filter
            var filter = "t:Prefab t:ForActiveScene t:ScriptableObject";
            if (selectedObj is Texture2D || selectedObj is Sprite || selectedObj is Shader)
            {
                filter += " t:Material";
            }
            else if (selectedObj is AnimationClip)
            {
                filter += " t:AnimatorController";
            }

            //all objects to check
            var fileGUIDs = UnityEditor.AssetDatabase.FindAssets(filter);
            //Debug.Log("files count: " + fileGUIDs.Length);

            progress = 0;
            totalCount = fileGUIDs.Length;

            //prefab, scene, script, material, shader files
            foreach (var guid in fileGUIDs)
            {
                if (guid == selectedGuid)
                {
                    continue;
                }

                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                UnityEditor.EditorUtility.DisplayProgressBar("Project", "Searching...", 1f * ++progress / totalCount);

                var dependencies = FindAllReferencesHelper.GetDependencies(guid);
                foreach (var dependeFile in dependencies)
                {
                    if (dependeFile.Equals(selectedGuid))
                    {
                        var obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                        CheckDependentFile(obj);
                        break;
                    }
                }
            }
        }

        private static void CheckDependentFile(Object asset)
        {
            if (asset is UnityEditor.Animations.AnimatorController)
            {
                var r = new ProjectResult
                {
                    ChildPath = "animationClips[]",
                    PropertyName = "",
                    ReferenceObject = selectedObj
                };

                SaveProjectResult(asset, r);
            }
            else if (asset is GameObject)
            {
                CheckGameObject(asset as GameObject, false, asset);
            }
            else if (asset is UnityEditor.SceneAsset)
            {
                var assetPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
                if (SceneManager.GetActiveScene().path != assetPath)
                {
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath,
                            UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        CheckScene(scene, false, asset);
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                    }
                    else if (!projectResults.ContainsKey(asset))
                    {
                        var r = new ProjectResult
                        {
                            Tip = "<Cannot show details in playing mode>"
                        };

                        SaveProjectResult(asset, r);
                    }
                }
            }
            else
            {
                CheckReference(asset, false, asset);
            }
        }

        private static string GetPropertyName(Component c, UnityEditor.SerializedProperty p)
        {
            var obj = p.objectReferenceValue;
            var name = p.propertyPath;

            if (c != null)
            {
                if (c is Text && name == "m_FontData.m_Font")
                {
                    return "Font";
                }

                if (c is Button && name.StartsWith("m_OnClick."))
                {
                    return name.Replace("m_OnClick.m_PersistentCalls.m_Calls.Array.data", "OnClick").Replace("m_", "");
                }

                if (c is AudioSource && obj is AudioMixerGroup)
                {
                    return "Output";
                }
            }

            return name.Replace(".Array.data", "");
        }

        private static string GetPropertyPath(Object root, Component c, UnityEditor.SerializedProperty p)
        {
            var path = "";

            var parent = c.transform;
            if (parent.gameObject != root)
            {
                path = c.name;
            }

            path += " \u27A4 (" + c.GetType().Name + ")";

            while ((parent = parent.parent) != null && parent.gameObject != root)
            {
                path = parent.name + " \u27A4 " + path;
            }
            return path;
        }

        private static void SaveHierarchyResult(Object obj, HierarchyResult r)
        {
            if (hierarchyResults.ContainsKey(obj))
            {
                hierarchyResults[obj].Add(r);
            }
            else
            {
                hierarchyResults.Add(obj, new List<HierarchyResult>
                {
                    r
                });
            }
        }

        private static void SaveProjectResult(Object obj, ProjectResult r)
        {
            if (projectResults.ContainsKey(obj))
            {
                projectResults[obj].Add(r);
            }
            else
            {
                projectResults.Add(obj, new List<ProjectResult>
                {
                    r
                });
            }
        }

        #endregion


        #region Window

        private static void ShowWindow()
        {
            GetWindow<FindAllReferences>("\u273F References").Init();
        }

        private void Init()
        {
            _hierarchyLabelIcon = new GUIContent(" Hierarchy",
                UnityEditor.EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"));
            _sceneLabelIcon = new GUIContent("ForActiveScene",
                UnityEditor.EditorGUIUtility.FindTexture("SceneAsset Icon"));
            _projectLabelIcon = new GUIContent(" Project", UnityEditor.EditorGUIUtility.FindTexture("Project"));

            _headerStyle = new GUIStyle("CN CountBadge");
            _headerStyle.alignment = TextAnchor.MiddleLeft;
            _headerStyle.imagePosition = ImagePosition.ImageLeft;
            _headerStyle.padding = new RectOffset(10, 5, -1, 1);

            _titleStyle = new GUIStyle("MeTimeLabel");

            _labelStyle = new GUIStyle("GUIEditor.BreadcrumbMid");
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.padding = new RectOffset(10, 5, 1, 1);

            _redraw = true;
        }

        private void OnGUI()
        {
            GUI.SetNextControlName("FAR_empty");
            UnityEditor.EditorGUILayout.Separator();

            if (selectedObj == null)
            {
                UnityEditor.EditorGUILayout.BeginVertical("RegionBg");
                UnityEditor.EditorGUILayout.HelpBox("Usage:\n" +
                                                    "1. select a object in Hierarchy view or Project view.\n" +
                                                    "2. right click on object, choose \"Find All References\".",
                    UnityEditor.MessageType.Info);
                UnityEditor.EditorGUILayout.EndVertical();
                return;
            }

            //the selected object
            UnityEditor.EditorGUILayout.BeginVertical("RegionBg");
            GUILayout.Space(-15);
            UnityEditor.EditorGUILayout.ObjectField(selectedObj, selectedObj.GetType(), true);

            var msg = "";
            if (hierarchyResults.Count == 0 && projectResults.Count == 0)
            {
                msg = "Found 0 References";
            }
            else
            {
                if (hierarchyResults.Count > 0)
                {
                    var cnt = 0;
                    foreach (var list in hierarchyResults.Values)
                    {
                        cnt += list.Count;
                    }

                    msg += string.Format("Found {0} References In Hierarchy", cnt);
                }
                if (projectResults.Count > 0)
                {
                    var cnt = 0;
                    foreach (var list in projectResults.Values)
                    {
                        cnt += list.Count;
                    }

                    msg += string.Format("\nFound {0} References In Project", cnt);
                }
            }

            UnityEditor.EditorGUILayout.HelpBox(msg, UnityEditor.MessageType.Info);

            UnityEditor.EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            //show results
            _scrollPosition = UnityEditor.EditorGUILayout.BeginScrollView(_scrollPosition);
            OnResultsInHierarchyGUI();
            OnResultsInProjectGUI();
            UnityEditor.EditorGUILayout.EndScrollView();

            if (_redraw)
            {
                _redraw = false;
                GUI.FocusControl("FAR_empty");
            }
        }

        private void OnResultsInHierarchyGUI()
        {
            if (hierarchyResults.Count == 0)
            {
                return;
            }

            UnityEditor.EditorGUILayout.BeginVertical("RegionBg");

            //header
            GUILayout.Space(-15);
            UnityEditor.EditorGUILayout.LabelField(_hierarchyLabelIcon, _headerStyle, GUILayout.Width(200),
                GUILayout.Height(20));

            //title
            var rect = UnityEditor.EditorGUILayout.GetControlRect();
            var controlWidth = rect.width;
            rect.width = 200;
            UnityEditor.EditorGUI.LabelField(rect, "gameObject", _titleStyle);
            rect.x = rect.width + 20;
            var labelWidth = controlWidth - rect.x - 5;
            rect.width = labelWidth * 0.4f;
            UnityEditor.EditorGUI.LabelField(rect, "component", _titleStyle);
            rect.x += rect.width;
            rect.width = labelWidth - rect.width;
            UnityEditor.EditorGUI.LabelField(rect, "property", _titleStyle);
            GUILayout.Space(5);

            _sceneLabelIcon.text = " " + SceneManager.GetActiveScene().name;
            UnityEditor.EditorGUILayout.LabelField(_sceneLabelIcon, GUILayout.Height(18));
            GUILayout.Space(5);

            foreach (var pair in hierarchyResults)
            {
                for (var i = 0; i < pair.Value.Count; i++)
                {
                    var r = pair.Value[i];

                    rect = UnityEditor.EditorGUILayout.GetControlRect();

                    //gameobject
                    if (i == 0)
                    {
                        rect.width = 200;
                        UnityEditor.EditorGUI.ObjectField(rect, pair.Key, typeof(GameObject), true);
                    }
                    else
                    {
                        rect.x += 185;
                        rect.width = 15;
                        UnityEditor.EditorGUI.LabelField(rect, "\u27A5");
                    }

                    if (!string.IsNullOrEmpty(r.Tip))
                    {
                        rect.x += rect.width + 20;
                        rect.width = labelWidth;
                        UnityEditor.EditorGUI.LabelField(rect, r.Tip);
                        GUILayout.Space(5);
                        continue;
                    }

                    //component
                    rect.x += rect.width + 20;
                    var contentWidth = labelWidth * 0.4f;
                    rect.width = contentWidth;
                    if (r.Script != null)
                    {
                        UnityEditor.EditorGUI.LabelField(rect, "", _labelStyle);
                        rect.x += LabelTextOffset;
                        rect.width -= LabelTextOffset;
                        rect.y += 1;
                        UnityEditor.EditorGUI.ObjectField(rect, r.Script, r.Script.GetType(), false);
                        rect.x += contentWidth - LabelTextOffset;
                        rect.y -= 1;
                    }
                    else
                    {
                        UnityEditor.EditorGUI.LabelField(rect,
                            new GUIContent(UnityEditor.AssetPreview.GetMiniThumbnail(r.Component)), _labelStyle);
                        rect.x += 25;
                        rect.width -= 25;
                        UnityEditor.EditorGUI.SelectableLabel(rect, r.Component.GetType().Name);
                        rect.x += rect.width;
                    }

                    //property
                    if (!(r.Script != null && r.Script == selectedObj))
                    {
                        contentWidth = labelWidth - contentWidth;
                        rect.width = contentWidth;
                        UnityEditor.EditorGUI.LabelField(rect, "", _labelStyle);
                        rect.x += LabelTextOffset;
                        rect.width = contentWidth * 0.5f;
                        UnityEditor.EditorGUI.SelectableLabel(rect, r.PropertyName);
                        rect.x += rect.width;
                        rect.width = contentWidth - rect.width - LabelTextOffset;
                        rect.y += 1;
                        UnityEditor.EditorGUI.ObjectField(rect, r.Property, r.Property.GetType(), false);
                    }

                    GUILayout.Space(5);
                }
            }

            UnityEditor.EditorGUILayout.EndVertical();
            GUILayout.Space(15);
        }

        private void OnResultsInProjectGUI()
        {
            if (projectResults.Count == 0)
            {
                return;
            }

            UnityEditor.EditorGUILayout.BeginVertical("RegionBg");

            //header
            GUILayout.Space(-15);
            UnityEditor.EditorGUILayout.LabelField(_projectLabelIcon, _headerStyle, GUILayout.Width(200),
                GUILayout.Height(20));

            //title
            var rect = UnityEditor.EditorGUILayout.GetControlRect();
            var controlWidth = rect.width;
            rect.width = 200;
            UnityEditor.EditorGUI.LabelField(rect, "asset", _titleStyle);
            rect.x += rect.width + 20;
            var labelWidth = controlWidth - rect.x - 5;
            rect.width = labelWidth * 0.6f;
            UnityEditor.EditorGUI.LabelField(rect, "child path (component)", _titleStyle);
            rect.x += rect.width;
            rect.width = labelWidth - rect.width;
            UnityEditor.EditorGUI.LabelField(rect, "property", _titleStyle);
            GUILayout.Space(5);

            foreach (var pair in projectResults)
            {
                for (var i = 0; i < pair.Value.Count; i++)
                {
                    var r = pair.Value[i];

                    rect = UnityEditor.EditorGUILayout.GetControlRect();

                    //object
                    if (i == 0)
                    {
                        rect.width = 200;
                        UnityEditor.EditorGUI.ObjectField(rect, pair.Key, pair.Key.GetType(), false);
                    }
                    else
                    {
                        rect.x += 185;
                        rect.width = 15;
                        UnityEditor.EditorGUI.LabelField(rect, "\u27A5");
                    }

                    if (!string.IsNullOrEmpty(r.Tip))
                    {
                        rect.x += rect.width + 20;
                        rect.width = labelWidth;
                        UnityEditor.EditorGUI.LabelField(rect, r.Tip);
                        GUILayout.Space(5);
                        continue;
                    }

                    //child path
                    rect.x += rect.width + 20;
                    rect.width = labelWidth * 0.6f;
                    UnityEditor.EditorGUI.LabelField(rect, "", _labelStyle);
                    rect.x += LabelTextOffset;
                    rect.width -= LabelTextOffset;
                    UnityEditor.EditorGUI.SelectableLabel(rect, r.ChildPath);

                    //property
                    if (r.ReferenceObject != null)
                    {
                        rect.x += rect.width;
                        rect.width = labelWidth - rect.width - LabelTextOffset;
                        UnityEditor.EditorGUI.LabelField(rect, "", _labelStyle);
                        rect.x += LabelTextOffset;
                        rect.width -= LabelTextOffset;
                        var contentWidth = rect.width;
                        rect.width = contentWidth * 0.4f;
                        UnityEditor.EditorGUI.SelectableLabel(rect, r.PropertyName);
                        rect.x += rect.width;
                        rect.y += 1;
                        rect.width = contentWidth - rect.width;
                        UnityEditor.EditorGUI.ObjectField(rect, r.ReferenceObject, r.ReferenceObject.GetType(), false);
                    }

                    GUILayout.Space(5);
                }
            }

            UnityEditor.EditorGUILayout.EndVertical();
        }

        #endregion


        #region Class

        private class HierarchyResult
        {
            public string Tip;
            public Component Component;
            public UnityEditor.MonoScript Script;
            public Object Property;
            public string PropertyName;
        }

        private class ProjectResult
        {
            public string Tip;
            public string ChildPath;
            public string PropertyName;
            public Object ReferenceObject;
        }

        #endregion
    }

    [UnityEditor.InitializeOnLoadAttribute]
    public class FindAllReferencesHelper : UnityEditor.AssetPostprocessor
    {
        private static readonly string cachePath;

        static FindAllReferencesHelper()
        {
            cachePath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/FindAllReferences"));
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
                BuildCaches();
                Debug.Log("[FindAllReferences] Caches Path: " + cachePath);
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                var ext = Path.GetExtension(path);
                if (ext is ".prefab" or ".unity" or ".mat" or ".controller" or ".asset")
                {
                    var guid = UnityEditor.AssetDatabase.AssetPathToGUID(path);
                    var dependencies = UnityEditor.AssetDatabase.GetDependencies(path);
                    CreateCache(guid, dependencies);
                }
            }

            foreach (var path in deletedAssets)
            {
                var cache = Path.Combine(cachePath, Path.GetFileName(path));
                if (File.Exists(cache))
                {
                    File.Delete(cache);
                }
            }
        }

        public static string[] GetDependencies(string guid)
        {
            if (LoadCache(guid, out var dependencies))
            {
                return dependencies;
            }

            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            dependencies = UnityEditor.AssetDatabase.GetDependencies(assetPath, false);

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
            CreateCache(guid, dependencies);

            return dependencies;
        }

        private static void BuildCaches()
        {
            UnityEditor.EditorUtility.DisplayProgressBar("[FindAllReferences] - Building caches ...",
                "To path: " + cachePath, 0f);

            var filter = "t:Prefab t:ForActiveScene t:ScriptableObject t:Material t:AnimatorController";
            var guids = UnityEditor.AssetDatabase.FindAssets(filter);

            for (var i = 0; i < guids.Length; i++)
            {
                CreateCache(guids[i]);
                UnityEditor.EditorUtility.DisplayProgressBar("[FindAllReferences] - Building caches ...",
                    "To path: " + cachePath, 1f * i / guids.Length);
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

        public static bool LoadCache(string guid, out string[] dependencies)
        {
            var cachePath = Path.Combine(FindAllReferencesHelper.cachePath, guid);
            if (!File.Exists(cachePath))
            {
                dependencies = null;
                return false;
            }

            var sr = new StreamReader(cachePath);
            dependencies = sr.ReadLine()?.Split('|');
            sr.Close();

            return true;
        }

        public static void CreateCache(string guid)
        {
            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            CreateCache(guid, UnityEditor.AssetDatabase.GetDependencies(assetPath, false));
        }

        public static void CreateCache(string guid, string[] dependencies)
        {
            var sw = new StreamWriter(Path.Combine(cachePath, guid));
            for (var i = 0; i < dependencies.Length; i++)
            {
                dependencies[i] = UnityEditor.AssetDatabase.AssetPathToGUID(dependencies[i]);
            }
            sw.WriteLine(string.Join("|", dependencies));
            sw.Close();
        }
    }
}