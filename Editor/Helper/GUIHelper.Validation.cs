using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Helper
{
    public static partial class GUIHelper
    {
        /// <summary>
        /// Destroy the target object but display a validation dialogue
        /// </summary>
        public static bool DestroyDialogue(Object target)
        {
            if (target == null)
            {
                return false;
            }

            if (Event.current.shift)
            {
                EditorHelper.DeleteAsset(target);
                return true;
            }

            var message = $"Do you want to delete: {target.name} \nThis operation cannot be undone!";
            var result = EditorUtility.DisplayDialog("Delete Object", message, "Delete", "Cancel Operation");

            if (result)
            {
                EditorHelper.DeleteAsset(target);
            }

            return result;
        }

        public static void DrawException(Exception exception)
        {
            EditorGUILayout.HelpBox($"Exception while processing GUI:\n{exception}", MessageType.Error);
        }
    }
}