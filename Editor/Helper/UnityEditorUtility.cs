using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using Baracuda.Utilities.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Editor.Helper
{
    [UnityEditor.InitializeOnLoadAttribute]
    public static class UnityEditorUtility
    {
        private static readonly MethodInfo activeFolderPathMethod =
            typeof(UnityEditor.ProjectWindowUtil).GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);

        public static string GetCurrentAssetDirectory()
        {
            return activeFolderPathMethod.Invoke(null, Array.Empty<object>()).ToString();
        }

        private static UnityEditor.Compilation.Assembly[] UnityAssemblies { get; }

        static UnityEditorUtility()
        {
            UnityAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
        }

        public static bool IsEditorAssembly(this Assembly assembly)
        {
            var editorAssemblies = UnityAssemblies;

            for (var i = 0; i < editorAssemblies.Length; i++)
            {
                var unityAssembly = editorAssemblies[i];

                if (unityAssembly.name != assembly.GetName().Name)
                {
                    continue;
                }

                if (unityAssembly.flags.HasFlag(UnityEditor.Compilation.AssemblyFlags.EditorAssembly))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLocatedInResources(this Object asset)
        {
            return UnityEditor.AssetDatabase.GetAssetPath(asset).Contains("/Resources");
        }

        public static bool TryFindPrefabsOfType<TComponent>(out List<TComponent> prefabs) where TComponent : Component
        {
            prefabs = FindPrefabsOfType<TComponent>();
            return prefabs.IsNotNullOrEmpty();
        }

        public static List<GameObject> FindPrefabsOfTypeAsGameObjects<TComponent>() where TComponent : Component
        {
            var assets = new List<GameObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponent<TComponent>(out _))
                {
                    assets.Add(gameObject);
                }
            }

            return assets;
        }

        public static List<Component> FindPrefabsWithGenericInterface(Type interfaceType)
        {
            var assets = new List<Component>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var components = gameObject.GetComponents<Component>();
                for (var j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (!component)
                    {
                        continue;
                    }

                    if (components[j].GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                    {
                        assets.Add(components[j]);
                    }
                }
            }

            return assets;
        }

        public static List<GameObject> FindPrefabsWithGenericInterfaceAsGameObject(Type interfaceType)
        {
            var assets = new List<GameObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var components = gameObject.GetComponents<Component>();

                for (var j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (!component)
                    {
                        continue;
                    }

                    if (components[j].GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                    {
                        assets.Add(gameObject);
                    }
                }
            }

            return assets;
        }

        public static string[] GUIDsToPaths(string[] guids)
        {
            var paths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return paths;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2>() where T1 : Object where T2 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3>() where T1 : Object where T2 : Object where T3 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3, T4>() where T1 : Object
            where T2 : Object
            where T3 : Object
            where T4 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            list.AddRange(FindAssetsOfType<T4>());
            return list;
        }

        [Pure]
        public static List<Object> FindAssetsOfType<T1, T2, T3, T4, T5>() where T1 : Object
            where T2 : Object
            where T3 : Object
            where T4 : Object
            where T5 : Object
        {
            var list = new List<Object>();
            list.AddRange(FindAssetsOfType<T1>());
            list.AddRange(FindAssetsOfType<T2>());
            list.AddRange(FindAssetsOfType<T3>());
            list.AddRange(FindAssetsOfType<T4>());
            list.AddRange(FindAssetsOfType<T5>());
            return list;
        }

        [Pure]
        public static T FindAssetOfType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var asset in assets)
                {
                    if (asset is T typedAsset)
                    {
                        return typedAsset;
                    }
                }
            }
            return null;
        }

        [Pure]
        public static List<T> FindAssetsOfType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            var assetList = new List<T>();
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var asset in assets)
                {
                    if (asset is T typedAsset)
                    {
                        assetList.Add(typedAsset);
                    }
                }
            }

            return assetList;
        }

        [Pure]
        public static string[] GetAssetPathsOfType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            var paths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return paths;
        }

        /*
         * Non Generic
         */

        [Pure]
        public static List<ScriptableObject> FindAssetsOfType(Type objectType)
        {
            var assets = new List<ScriptableObject>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{objectType}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        [Pure]
        public static List<Component> FindPrefabsOfType(Type componentType)
        {
            var assets = new List<Component>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponents(componentType, out var components))
                {
                    assets.AddRange(components);
                }
            }

            return assets;
        }

        [Pure]
        public static List<T> FindPrefabsOfType<T>()
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab", new[]
            {
                "Assets/Content"
            });
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponent<T>(out var components))
                {
                    assets.Add(components);
                }
            }

            return assets;
        }

        public static string[] FindAllScenePaths()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");
            var scenePaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                scenePaths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return scenePaths;
        }

        [Pure]
        public static string[] GetAssetPaths(IList<Object> assets)
        {
            var result = new string[assets.Count];
            for (var i = 0; i < assets.Count; i++)
            {
                result[i] = UnityEditor.AssetDatabase.GetAssetPath(assets[i]);
            }
            return result;
        }

        [Pure]
        public static string[] GetAssetPaths(ICollection<Object> assets)
        {
            var result = new string[assets.Count];
            var index = 0;
            foreach (var asset in assets)
            {
                result[index] = UnityEditor.AssetDatabase.GetAssetPath(asset);
                index++;
            }
            return result;
        }

        public static bool TryFindAssetsOfType<T>(out List<T> assets) where T : Object
        {
            assets = FindAssetsOfType<T>();
            return assets.IsNotNullOrEmpty();
        }

        public static List<T> FindAssetsOfTypeWithInterface<T>(Type interfaceType) where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && asset.GetType().HasInterface(interfaceType))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<T> FindAssetsOfTypeWithGenericInterface<T>(Type interfaceType) where T : Object
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && asset.GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static ScriptableObject CreateScriptableObjectAsset(Type type)
        {
            if (type.IsGenericType)
            {
                UnityEngine.Debug.LogWarning("Cannot create an asset for a generic type!");
                return null;
            }

            var so = ScriptableObject.CreateInstance(type);
            var filePath = GetCurrentAssetDirectory();
            var fileName = type.Name;
            var completePath =
                $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}.asset";
            var index = 1;
            while (File.Exists(completePath))
            {
                completePath =
                    $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}{index++.ToString()}.asset";
            }

            UnityEditor.AssetDatabase.CreateAsset(so, completePath);

            return so;
        }

        public static void DeleteAsset(Object asset)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            UnityEditor.AssetDatabase.DeleteAsset(path);
        }

        public static void SelectObject(Object target)
        {
            UnityEditor.Selection.activeObject = target;
            UnityEditor.EditorGUIUtility.PingObject(target);
        }
    }
}