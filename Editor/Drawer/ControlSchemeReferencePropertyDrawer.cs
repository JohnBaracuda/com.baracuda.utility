using System.Collections.Generic;
using System.Linq;
using Baracuda.Utility.Collections;
using Baracuda.Utility.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Baracuda.Utility.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(ControlSchemeReference))]
    public class ControlSchemeReferencePropertyDrawer : UnityEditor.PropertyDrawer
    {
        private string[] _schemes;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (_schemes is null)
            {
                PopulateSchemes();
            }

            var relativeProperty = property.FindPropertyRelative("value");
            var mask = GetMask(relativeProperty.stringValue, _schemes);
            var newMask = UnityEditor.EditorGUI.MaskField(position, label.text, mask, _schemes);

            relativeProperty.stringValue = FromMask(newMask, _schemes);
        }

        private int GetMask(string current, string[] allElements)
        {
            var currentElements = current.Split(";");

            var mask = 0;
            for (var i = 0; i < allElements.Length; i++)
            {
                if (currentElements.Contains(allElements[i]))
                {
                    mask |= 1 << i;
                }
            }

            return mask;
        }

        private string FromMask(int mask, string[] allElements)
        {
            var elements = new List<string>();

            for (var i = 0; i < allElements.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    elements.Add(allElements[i]);
                }
            }

            return string.Join(";", elements);
        }

        private void PopulateSchemes()
        {
            var buffer = ListPool<string>.Get();

            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(InputActionAsset)}");

            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);

                if (asset != null)
                {
                    foreach (var controlScheme in asset.controlSchemes)
                    {
                        buffer.AddUnique(controlScheme.name);
                    }
                }
            }

            _schemes = buffer.ToArray();
            ListPool<string>.Release(buffer);
        }
    }
}