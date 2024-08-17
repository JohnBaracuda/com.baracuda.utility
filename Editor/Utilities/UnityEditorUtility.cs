using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Editor.Utilities
{
    [UnityEditor.InitializeOnLoadAttribute]
    public static partial class UnityEditorUtility
    {
        [PublicAPI]
        public static string GetCurrentAssetDirectory()
        {
            return activeFolderPathMethod.Invoke(null, Array.Empty<object>()).ToString();
        }

        [PublicAPI]
        public static bool IsEditorAssembly(this Assembly assembly)
        {
            return IsEditorAssemblyInternal(assembly);
        }

        [PublicAPI]
        public static bool IsLocatedInResources(this Object asset)
        {
            return IsLocatedInResourcesInternal(asset);
        }

        [PublicAPI]
        public static bool TryFindPrefabsOfType<TComponent>(out List<TComponent> prefabs) where TComponent : Component
        {
            return TryFindPrefabsOfTypeInternal(out prefabs);
        }

        [PublicAPI]
        public static List<GameObject> FindPrefabsOfTypeAsGameObjects<TComponent>() where TComponent : Component
        {
            return FindPrefabsOfTypeAsGameObjectsInternal<TComponent>();
        }

        [PublicAPI]
        public static List<Component> FindPrefabsWithGenericInterface(Type interfaceType)
        {
            return FindPrefabsWithGenericInterfaceInternal(interfaceType);
        }

        [PublicAPI]
        public static List<GameObject> FindPrefabsWithGenericInterfaceAsGameObject(Type interfaceType)
        {
            return FindPrefabsWithGenericInterfaceAsGameObjectInternal(interfaceType);
        }

        [PublicAPI]
        public static string[] GUIDsToPaths(string[] guids)
        {
            return GUIDsToPathsInternal(guids);
        }

        [PublicAPI]
        public static T FindAssetOfType<T>() where T : Object
        {
            return FindAssetOfTypeInternal<T>();
        }

        [PublicAPI]
        public static List<T> FindAssetsOfType<T>() where T : Object
        {
            return FindAssetsOfTypeInternal<T>();
        }

        [PublicAPI]
        public static string[] GetAssetPathsOfType<T>() where T : Object
        {
            return GetAssetPathsOfTypeInternal<T>();
        }

        [PublicAPI]
        public static List<ScriptableObject> FindAssetsOfType(Type objectType)
        {
            return FindAssetsOfTypeInternal(objectType);
        }

        [PublicAPI]
        public static List<Component> FindPrefabsOfType(Type componentType)
        {
            return FindPrefabsOfTypeInternal(componentType);
        }

        [PublicAPI]
        public static List<T> FindPrefabsOfType<T>() where T : Component
        {
            return FindPrefabsOfTypeInternal<T>();
        }

        [PublicAPI]
        public static string[] FindAllScenePaths()
        {
            return FindAllScenePathsInternal();
        }

        [PublicAPI]
        public static string[] GetAssetPaths(IList<Object> assets)
        {
            return GetAssetPathsInternal(assets);
        }

        [PublicAPI]
        public static bool TryFindAssetsOfType<T>(out List<T> assets) where T : Object
        {
            return TryFindAssetsOfTypeInternal(out assets);
        }

        [PublicAPI]
        public static List<T> FindAssetsOfTypeWithInterface<T>(Type interfaceType) where T : Object
        {
            return FindAssetsOfTypeWithInterfaceInternal<T>(interfaceType);
        }

        [PublicAPI]
        public static ScriptableObject CreateScriptableObjectAsset(Type type)
        {
            return CreateScriptableObjectAssetInternal(type);
        }

        [PublicAPI]
        public static void DeleteAsset(Object asset)
        {
            DeleteAssetInternal(asset);
        }

        [PublicAPI]
        public static void SelectObject(Object target)
        {
            SelectObjectInternal(target);
        }
    }
}