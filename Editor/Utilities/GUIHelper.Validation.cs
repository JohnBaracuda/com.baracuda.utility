using System;
using Baracuda.Utilities.Editor.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utility.Editor.Utilities
{
    public static partial class GUIUtility
    {
        /// <summary>
        ///     Destroy the target object but display a validation dialogue
        /// </summary>
        public static bool DestroyDialogue(Object target)
        {
            if (target == null)
            {
                return false;
            }

            if (Event.current.shift)
            {
                UnityEditorUtility.DeleteAsset(target);
                return true;
            }

            var message = $"Do you want to delete: {target.name} \nThis operation cannot be undone!";
            var result =
                UnityEditor.EditorUtility.DisplayDialog("Delete Object", message, "Delete", "Cancel Operation");

            if (result)
            {
                UnityEditorUtility.DeleteAsset(target);
            }

            return result;
        }

        public static void DrawException(Exception exception)
        {
            UnityEditor.EditorGUILayout.HelpBox($"Exception while processing GUI:\n{exception}",
                UnityEditor.MessageType.Error);
        }
    }
}