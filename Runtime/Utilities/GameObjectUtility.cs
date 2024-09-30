using System.Diagnostics;
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
    }
}