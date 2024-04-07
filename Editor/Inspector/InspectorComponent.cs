using Baracuda.Utilities.Types;

namespace Baracuda.Utilities.Editor.Inspector
{
    [UnityEditor.CustomEditor(typeof(InspectorComponent))]
    public class InspectorComponentEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty _property;
        private UnityEditor.Editor _inlinedEditor;

        private void OnEnable()
        {
            _property = serializedObject.FindProperty("inlined");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UnityEditor.EditorGUILayout.PropertyField(_property);

            if (_property.objectReferenceValue == null)
            {
                _inlinedEditor = null;
            }
            else if (_inlinedEditor == null)
            {
                _inlinedEditor = CreateEditor(_property.objectReferenceValue);
            }

            if (_inlinedEditor != null)
            {
                _inlinedEditor.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}