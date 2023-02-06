using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true)]
#endif
    public class ScriptableObjectFoldoutInspector : OverrideInspector<ScriptableObject>
    {
    }
}