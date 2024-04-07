using UnityEngine;

namespace Baracuda.Utilities.Editor.Helper
{
    public static partial class GUIUtility
    {
        public static int StringPopupMask(Rect rect, int mask, string[] available)
        {
            return UnityEditor.EditorGUI.MaskField(rect, mask, available);
        }
    }
}