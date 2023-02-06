using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Helper
{
    [InitializeOnLoad]
    public static class EditorHelper
    {
        private static readonly MethodInfo activeFolderPathMethod =
            typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

        public static string GetCurrentAssetDirectory()
        {
            return activeFolderPathMethod.Invoke(null, Array.Empty<object>()).ToString();
        }

        /*
         *  Editor Assembly Utility
         */

        private static UnityEditor.Compilation.Assembly[] UnityAssemblies { get; }

        static EditorHelper()
        {
            UnityAssemblies = CompilationPipeline.GetAssemblies();
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

                if (unityAssembly.flags.HasFlag(AssemblyFlags.EditorAssembly))
                {
                    return true;
                }
            }

            return false;
        }

        /*
         *  Asset Database
         */

        public static bool IsLocatedInResources(this Object asset)
        {
            return AssetDatabase.GetAssetPath(asset).Contains("/Resources");
        }

        public static bool TryFindPrefabsOfType<TComponent>(out List<TComponent> prefabs) where TComponent : Component
        {
            prefabs = FindPrefabsOfType<TComponent>();
            return prefabs.IsNotNullOrEmpty();
        }

        public static List<TComponent> FindPrefabsOfType<TComponent>() where TComponent : Component
        {
            var assets = new List<TComponent>();
            var guids = AssetDatabase.FindAssets($"t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (gameObject.IsNotNull() && gameObject.TryGetComponents<TComponent>(out var components))
                {
                    assets.AddRange(components);
                }
            }

            return assets;
        }

        public static List<GameObject> FindPrefabsOfTypeAsGameObjects<TComponent>() where TComponent : Component
        {
            var assets = new List<GameObject>();
            var guids = AssetDatabase.FindAssets($"t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
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
            var guids = AssetDatabase.FindAssets($"t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
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
            var guids = AssetDatabase.FindAssets($"t:prefab");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
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

        public static bool TryFindAssetsOfType<T>(out List<T> assets) where T : Object
        {
            assets = FindAssetsOfType<T>();
            return assets.IsNotNullOrEmpty();
        }

        public static List<T> FindAssetsOfType<T>() where T : Object
        {
            var assets = new List<T>();
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<T> FindAssetsOfTypeWithInterface<T>(Type interfaceType) where T : Object
        {
            var assets = new List<T>();
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
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
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null && asset.GetType().HasInterfaceWithGenericTypeDefinition(interfaceType))
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<Object> FindAssetsOfType(Type type)
        {
            var assets = new List<Object>();
            var guids = AssetDatabase.FindAssets($"t:{type}");
            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (asset != null)
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
                Debug.LogWarning("Cannot create an asset for a generic type!");
                return null;
            }

            var so = ScriptableObject.CreateInstance(type);
            var filePath = GetCurrentAssetDirectory();
            var fileName = type.Name;
            var completePath = $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}.asset";
            var index = 1;
            while (File.Exists(completePath))
            {
                completePath = $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : type.Name.Humanize())}{index++.ToString()}.asset";
            }

            AssetDatabase.CreateAsset(so, completePath);

            return so;
        }

        //--------------------------------------------------------------------------------------------------------------

        /*
         * Delete
         */

        public static void DeleteAsset(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }

        //--------------------------------------------------------------------------------------------------------------

        /*
         * Selection
         */

        public static void SelectObject(Object target)
        {
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }
    }
}