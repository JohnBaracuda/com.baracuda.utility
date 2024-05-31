using Baracuda.Utilities.Types;
using System;
using System.Linq;
using UnityEngine;

namespace Baracuda.Utilities.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : UnityEditor.PropertyDrawer
    {
        private SerializedTypeFilterAttribute _serializedTypeFilter;
        private string[] _typeNames;
        private string[] _typeFullNames;

        private void Initialize()
        {
            if (_typeFullNames != null)
            {
                return;
            }

            _serializedTypeFilter =
                (SerializedTypeFilterAttribute) Attribute.GetCustomAttribute(fieldInfo,
                    typeof(SerializedTypeFilterAttribute));

            var filteredTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => _serializedTypeFilter == null ? DefaultFilter(t) : _serializedTypeFilter.Filter(t))
                .ToArray();

            _typeNames = filteredTypes.Select(t => t.ReflectedType == null ? t.Name : "t.ReflectedType.Name + t.Name")
                .ToArray();
            _typeFullNames = filteredTypes.Select(t => t.AssemblyQualifiedName).ToArray();
        }

        private static bool DefaultFilter(Type type)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericType;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            Initialize();
            var typeIdProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (string.IsNullOrEmpty(typeIdProperty.stringValue))
            {
                typeIdProperty.stringValue = _typeFullNames.First();
                property.serializedObject.ApplyModifiedProperties();
            }

            var currentIndex = Array.IndexOf(_typeFullNames, typeIdProperty.stringValue);
            var selectedIndex = UnityEditor.EditorGUI.Popup(position, label.text, currentIndex, _typeNames);

            if (selectedIndex >= 0 && selectedIndex != currentIndex)
            {
                typeIdProperty.stringValue = _typeFullNames[selectedIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}