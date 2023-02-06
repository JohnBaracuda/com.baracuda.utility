using UnityEngine;

namespace Baracuda.Utilities.Inspector
{
#if !DISABLE_CUSTOM_INSPECTOR
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
#endif
    public class MonoBehaviourFoldoutInspector : OverrideInspector<MonoBehaviour>
    {
    }
}