using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baracuda.Utility.Utilities
{
    public static class GameObjectUtility
    {
        /// <summary>
        ///     Creates a divider GameObject and sets its sibling index to 0
        /// </summary>
        [Conditional("DEBUG")]
        public static void CreateDividerGameObject(int count = 25)
        {
            var divider = new GameObject(new string('-', count));
            divider.DontDestroyOnLoad();
            divider.transform.SetSiblingIndex(0);
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceCloneString<T>(this T component, string label) where T : MonoBehaviour
        {
            component.gameObject.name = component.gameObject.name.Replace("(Clone)", $"[{label}]");
        }
    }
}