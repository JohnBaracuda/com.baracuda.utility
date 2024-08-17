using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Baracuda.Bedrock.Reflection;
using Baracuda.Bedrock.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class UnityEditorUtility
    {
        private static readonly MethodInfo activeFolderPathMethod =
            typeof(UnityEditor.ProjectWindowUtil).GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);

        private static UnityEditor.Compilation.Assembly[] UnityAssemblies { get; }

        static UnityEditorUtility()
        {
            UnityAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();
        }

        private static bool IsEditorAssemblyInternal(Assembly assembly)
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

        private static bool IsLocatedInResourcesInternal(Object asset)
        {
            return UnityEditor.AssetDatabase.GetAssetPath(asset).Contains("/Resources");
        }

        private static bool TryFindPrefabsOfTypeInternal<TComponent>(out List<TComponent> prefabs) where TComponent : Component
        {
            prefabs = FindPrefabsOfTypeInternal<TComponent>();
            return prefabs != null && prefabs.Count > 0;
        }

        private static List<GameObject> FindPrefabsOfTypeAsGameObjectsInternal<TComponent>() where TComponent : Component
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

        private static List<Component> FindPrefabsWithGenericInterfaceInternal(Type interfaceType)
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

        private static List<GameObject> FindPrefabsWithGenericInterfaceAsGameObjectInternal(Type interfaceType)
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

        private static string[] GUIDsToPathsInternal(string[] guids)
        {
            var paths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }
            return paths;
        }

        private static T FindAssetOfTypeInternal<T>() where T : Object
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

        private static List<T> FindAssetsOfTypeInternal<T>() where T : Object
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

        private static string[] GetAssetPathsOfTypeInternal<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            var paths = new string[guids.Length];

            for (var i = 0; i < guids.Length; i++)
            {
                paths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return paths;
        }

        private static List<ScriptableObject> FindAssetsOfTypeInternal(Type objectType)
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

        private static List<Component> FindPrefabsOfTypeInternal(Type componentType)
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

        private static List<T> FindPrefabsOfTypeInternal<T>() where T : Component
        {
            var assets = new List<T>();
            var guids = UnityEditor.AssetDatabase.FindAssets("t:prefab", new[]
            {
                "Assets/Content"
            });

            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var gameObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject != null && gameObject.TryGetComponent<T>(out var component))
                {
                    assets.Add(component);
                }
            }

            return assets;
        }

        private static string[] FindAllScenePathsInternal()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");
            var scenePaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                scenePaths[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            return scenePaths;
        }

        private static string[] GetAssetPathsInternal(IList<Object> assets)
        {
            var result = new string[assets.Count];
            for (var i = 0; i < assets.Count; i++)
            {
                result[i] = UnityEditor.AssetDatabase.GetAssetPath(assets[i]);
            }
            return result;
        }

        private static bool TryFindAssetsOfTypeInternal<T>(out List<T> assets) where T : Object
        {
            assets = FindAssetsOfTypeInternal<T>();
            return assets != null && assets.Count > 0;
        }

        private static List<T> FindAssetsOfTypeWithInterfaceInternal<T>(Type interfaceType) where T : Object
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

        private static ScriptableObject CreateScriptableObjectAssetInternal(Type type)
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

        private static void DeleteAssetInternal(Object asset)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            UnityEditor.AssetDatabase.DeleteAsset(path);
        }

        private static void SelectObjectInternal(Object target)
        {
            UnityEditor.Selection.activeObject = target;
            UnityEditor.EditorGUIUtility.PingObject(target);
        }
    }
}