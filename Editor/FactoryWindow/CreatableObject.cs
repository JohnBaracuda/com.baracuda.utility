using Baracuda.Utilities.Pooling;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.FactoryWindow
{
    internal class CreatableObject
    {
        public Type Type { get; }
        public string AssemblyName { get; }
        public string CreateAssetPath { get; } = string.Empty;
        public string BaseTypes { get; }
        public string DefaultFileName { get; }
        
        public CreatableObject(Type type)
        {
            Type = type;
            AssemblyName = type.Assembly.GetName().Name;

            if (type.TryGetCustomAttribute<CreateAssetMenuAttribute>(out var createAssetAttribute))
            {
                CreateAssetPath = createAssetAttribute.menuName;
                DefaultFileName = createAssetAttribute.fileName.IsNotNullOrWhitespace() ? createAssetAttribute.fileName : Type.Name;
            }
            
            var sb = ConcurrentStringBuilderPool.Get();
            var types = Type.GetBaseTypesExcludeUnityTypes(false);
            for (var i = 0; i < types.Length; i++)
            {
                var baseType = types[i];
                sb.Append(baseType.HumanizedName());
                if (types.Length > 1 && i + 1 < types.Length)
                {
                    sb.Append(' ');
                    sb.Append('|');
                    sb.Append(' ');
                }
            }
            
            DefaultFileName ??= Type.Name;
            BaseTypes = ConcurrentStringBuilderPool.Release(sb);
        }

        public override string ToString()
        {
            return Type.Name;
        }

        public bool IsValidForFilter(string filter, SearchOptions searchOptions)
        {
            if(Type.Name.ContainsIgnoreCase(filter.NoSpace()))
            {
                return true;
            }
            
            if(searchOptions.HasFlagUnsafe(SearchOptions.AssemblyName) && AssemblyName.ContainsIgnoreCase(filter.NoSpace()))
            {
                return true;
            }
            
            if(searchOptions.HasFlagUnsafe(SearchOptions.CreateAttributePath) &&  CreateAssetPath.IsNotNullOrWhitespace() && CreateAssetPath.ContainsIgnoreCase(filter.NoSpace()))
            {
                return true;
            }
            
            if(searchOptions.HasFlagUnsafe(SearchOptions.BaseTypes) &&  BaseTypes.IsNotNullOrWhitespace() && BaseTypes.ContainsIgnoreCase(filter.NoSpace()))
            {
                return true;
            }

            return false;
        }

        public List<UnityEngine.Object> Create(string filePath, string fileName, int amount)
        {
            var createdObject = new List<UnityEngine.Object>(amount);
            for (var i = 0; i < amount; i++)
            {
                var so = CreateInternal(filePath, fileName);
                createdObject.Add(so);
            }
            AssetDatabase.Refresh();
            return createdObject;
        }

        private UnityEngine.Object CreateInternal(string filePath, string fileName)
        {
            var so = ScriptableObject.CreateInstance(Type);
            var completePath = $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : Type.Name.Humanize())}.asset";
            var index = 1;
            while (File.Exists(completePath))
            {
                completePath = $"{filePath}/{(fileName.IsNotNullOrWhitespace() ? fileName : Type.Name.Humanize())}{index++.ToString()}.asset";
            }

            AssetDatabase.CreateAsset(so, completePath);
            return so;
        }
    }
}