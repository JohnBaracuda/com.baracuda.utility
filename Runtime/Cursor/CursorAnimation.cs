using Baracuda.Bedrock.Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Baracuda.Bedrock.Cursor
{
    /// <summary>
    ///     Class for storing custom cursor animation data
    /// </summary>
    [CreateAssetMenu(menuName = "Cursor/Cursor-Animation", fileName = "Cursor-Animation", order = 100)]
    public class CursorAnimation : CursorFile
    {
        [Title("Animation")]
        [SerializeField] private float framesPerSecond = 10f;
        [SerializeField] public CursorAnimationType cursorAnimationType;
        [SerializeField] public Texture2D[] frames;

        internal WaitForSeconds Delay { get; private set; }

        public static implicit operator Texture2D(CursorAnimation file)
        {
            return file ? file.frames?.Length > 0 ? file.frames[0] : null : null;
        }

        protected void OnEnable()
        {
            Delay = new WaitForSeconds(1 / framesPerSecond);
        }

        private void OnValidate()
        {
            Delay = new WaitForSeconds(1 / framesPerSecond);
        }

#if UNITY_EDITOR

        [Button]
        [HorizontalGroup("Debug")]
        private void SetActiveCursor()
        {
            if (Application.isPlaying)
            {
                ServiceLocator.Get<CursorManager>().AddCursorOverride(this);
            }
        }

        [Button]
        [HorizontalGroup("Debug")]
        private void RemoveActiveCursor()
        {
            if (Application.isPlaying)
            {
                ServiceLocator.Get<CursorManager>().RemoveCursorOverride(this);
            }
        }
#endif
    }
}