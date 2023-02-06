using Baracuda.Utilities.Helper;
using System;
using UnityEditor;
using UnityEngine;

namespace Baracuda.Utilities.Inspector.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(FlagsToggleAttribute))]
    public class FlagsToggleDrawer : UnityEditor.PropertyDrawer
    {
        private Type _underLying = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                GUIHelper.DrawMessageBox("Property must be an enum!");
                return;
            }

            _underLying ??= property.GetUnderlyingType() ?? throw new Exception();
            property.enumValueFlag = GUIHelper.DrawFlagsEnumAsToggle(property.enumValueFlag, _underLying, true);
        }
    }
}