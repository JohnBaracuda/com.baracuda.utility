using System.Collections.Generic;
using Baracuda.Bedrock.PlayerLoop;
using JetBrains.Annotations;
using UnityEngine;

namespace Baracuda.Bedrock.Timing
{
    public static class CountdownSystem
    {
        #region Public API

        [PublicAPI]
        public static void PauseAllCountdowns()
        {
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                countdowns[index].Pause();
            }
        }

        [PublicAPI]
        public static void ResumeAllCountdowns()
        {
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                countdowns[index].Resume();
            }
        }

        [PublicAPI]
        public static void CancelAllCountdowns()
        {
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                countdowns[index].Cancel();
            }
        }

        [PublicAPI]
        public static void CompleteAllCountdowns()
        {
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                countdowns[index].Complete();
            }
        }

        [PublicAPI]
        public static IEnumerator<ICountdown> GetAllActiveCountdowns()
        {
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                yield return countdowns[index];
            }
        }

        #endregion


        #region Cooldown System

        private static readonly List<ICountdown> countdowns = new();

        static CountdownSystem()
        {
            Gameloop.Update -= OnUpdate;
            Gameloop.Update += OnUpdate;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= OnEditorUpdate;
            UnityEditor.EditorApplication.update += OnEditorUpdate;
#endif
        }

        internal static void AddCountdown(ICountdown countdownUpdate)
        {
            countdowns.Add(countdownUpdate);
        }

        internal static void RemoveCountdown(ICountdown countdownUpdate)
        {
            countdowns.Remove(countdownUpdate);
        }

        private static void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (var index = countdowns.Count - 1; index >= 0; index--)
            {
                countdowns[index].UpdateCountdown(deltaTime);
            }
        }

#if UNITY_EDITOR
        private static void OnEditorUpdate()
        {
            if (Application.isPlaying is false)
            {
                OnUpdate();
            }
        }
#endif

        #endregion
    }
}