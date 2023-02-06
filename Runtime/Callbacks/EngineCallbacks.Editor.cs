#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Baracuda.Utilities.Callbacks
{
    [InitializeOnLoad]
    public partial class EngineCallbacks : AssetModificationProcessor
    {
        #region Fields

        private static readonly List<Action> exitPlayModeDelegate = new();
        private static readonly List<Action> enterPlayModeDelegate = new();
        private static readonly List<Action> exitEditModeDelegate = new();
        private static readonly List<Action> enterEditModeDelegate = new();

        private static readonly List<IOnExitPlay> exitPlayModeListener = new();
        private static readonly List<IOnEnterPlay> enterPlayModeListener = new();
        private static readonly List<IOnExitEdit> exitEditModeListener = new();
        private static readonly List<IOnEnterEdit> enterEditModeListener = new();

        #endregion


        #region Internal Subscribtions

        private static void AddOnExitPlayInternal(IOnExitPlay listener)
        {
            exitPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitPlayInternal(IOnExitPlay listener)
        {
            exitPlayModeListener.Remove(listener);
        }

        private static void AddOnEnterPlayInternal(IOnEnterPlay listener)
        {
            enterPlayModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterPlayInternal(IOnEnterPlay listener)
        {
            enterPlayModeListener.Remove(listener);
        }

        private static void AddOnExitEditInternal(IOnExitEdit listener)
        {
            exitEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnExitEditInternal(IOnExitEdit listener)
        {
            exitEditModeListener.Remove(listener);
        }

        private static void AddOnEnterEditInternal(IOnEnterEdit listener)
        {
            enterEditModeListener.AddNullChecked(listener);
        }

        private static void RemoveOnEnterEditInternal(IOnEnterEdit listener)
        {
            enterEditModeListener.Remove(listener);
        }

        #endregion


        #region Initialization

        static EngineCallbacks()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        #endregion


        #region Play Mode State Changed

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnEnterEditMode();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    OnExitEditMode();

                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnEnterPlayMode();

                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    OnExitPlayMode();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void OnExitPlayMode()
        {
            for (var i = 0; i < exitPlayModeListener.Count; i++)
            {
                exitPlayModeListener[i].OnExitPlayMode();
            }
            for (var i = 0; i < exitPlayModeDelegate.Count; i++)
            {
                exitPlayModeDelegate[i]();
            }
        }

        private static void OnEnterPlayMode()
        {
            for (var i = 0; i < enterPlayModeListener.Count; i++)
            {
                enterPlayModeListener[i].OnEnterPlayMode();
            }
            for (var i = 0; i < enterPlayModeDelegate.Count; i++)
            {
                enterPlayModeDelegate[i]();
            }
        }

        private static void OnExitEditMode()
        {
            for (var i = 0; i < exitEditModeListener.Count; i++)
            {
                exitEditModeListener[i].OnExitEditMode();
            }
            for (var i = 0; i < exitEditModeDelegate.Count; i++)
            {
                exitEditModeDelegate[i]();
            }
        }

        private static void OnEnterEditMode()
        {
            for (var i = 0; i < enterEditModeListener.Count; i++)
            {
                enterEditModeListener[i].OnEnterEditMode();
            }
            for (var i = 0; i < enterEditModeDelegate.Count; i++)
            {
                enterEditModeDelegate[i]();
            }
        }

        #endregion


        #region Asset Handling

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitPlay exitPlayModeCallback)
            {
                exitPlayModeListener.Remove(exitPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterPlay enterPlayModeCallback)
            {
                enterPlayModeListener.Remove(enterPlayModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnExitEdit exitEditModeCallback)
            {
                exitEditModeListener.Remove(exitEditModeCallback);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (asset is IOnEnterEdit enterEditModeCallback)
            {
                enterEditModeListener.Remove(enterEditModeCallback);
            }

            return AssetDeleteResult.DidNotDelete;
        }

        #endregion
    }
}
#endif
