using UnityEngine;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static int StringPopupMask(Rect rect, int mask, string[] available)
        {
            return UnityEditor.EditorGUI.MaskField(rect, mask, available);
        }
    }
}