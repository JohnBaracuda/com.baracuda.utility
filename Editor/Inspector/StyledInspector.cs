using System.Reflection;
using UnityEditor;

namespace Baracuda.Utilities.Inspector
{
    public class StyledInspector<T> : UnityEditor.Editor where T : UnityEngine.Object
    {
        #region Properties & Fields

        /*
         * Generic Targets
         */

        protected T Target { get; private set; }
        protected T[] Targets { get; private set; }

        /*
         * Foldout Cache
         */

        protected FoldoutHandler Foldout { get; private set; }

        /*
         * Fields
         */

        private string _editorPrefsKey;

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        protected void SetDefaultFoldoutState(FoldoutData data, bool stateValue)
        {
            Foldout.DefaultFoldoutStates.TryAdd(data.Title, stateValue);
        }

        protected static void PopulateSerializedProperties(StyledInspector<T> inspector)
        {
            const BindingFlags FLAGS =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic;

            var fields = inspector.GetType().GetFields(FLAGS);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(SerializedProperty))
                {
                    fieldInfo.SetValue(inspector, inspector.FindProperty(fieldInfo.Name));
                }
            }
        }

        protected SerializedProperty FindProperty(string member)
        {
            var parsedMemberName = member.StartsWith('_') ? member.Remove(0,1) : member;
            return serializedObject.FindProperty(parsedMemberName);
        }


        protected virtual void OnEnable()
        {
            Target = target as T;
            Targets = targets.ConvertTo<T>();

            if (Target == null)
            {
                return;
            }

            _editorPrefsKey = $"{Target!.name}{Target.GetType().FullName}";
            LoadStateData(_editorPrefsKey);
        }

        protected virtual void OnDisable()
        {
            SaveStateData(_editorPrefsKey);
        }

        protected virtual void LoadStateData(string editorPrefsKey)
        {
            Foldout = new FoldoutHandler(editorPrefsKey);
        }

        protected virtual void SaveStateData(string editorPrefsKey)
        {
            Foldout?.SaveState();
        }
    }
}
