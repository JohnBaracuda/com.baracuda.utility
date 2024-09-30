using System;
using Baracuda.Utilities.Editor.Utilities;
using Baracuda.Utility.Attributes;
using UnityEngine;
using GUIUtility = Baracuda.Utility.Editor.Utilities.GUIUtility;

namespace Baracuda.Utility.Editor.Drawer
{
    [UnityEditor.CustomPropertyDrawer(typeof(FlagsToggleAttribute))]
    public class FlagsToggleDrawer : UnityEditor.PropertyDrawer
    {
        private Type _underLying;

        public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != UnityEditor.SerializedPropertyType.Enum)
            {
                GUIUtility.DrawMessageBox("Property must be an enum!");
                return;
            }

            _underLying ??= property.GetUnderlyingType() ?? throw new Exception();
            property.enumValueFlag = GUIUtility.DrawFlagsEnumAsToggle(property.enumValueFlag, _underLying, true);
        }
    }
}