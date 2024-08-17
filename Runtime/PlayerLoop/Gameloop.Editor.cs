using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.PlayerLoop
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
    public sealed partial class Gameloop : UnityEditor.AssetModificationProcessor
    {
        public delegate void WillDeleteAssetCallback(string assetPath, Object asset);

        public static event WillDeleteAssetCallback BeforeDeleteAsset;

        private static readonly List<Action> editorUpdateCallbacks = new(Capacity32);
        private static readonly List<Action> exitPlayModeDelegate = new(Capacity32);
        private static readonly List<Action> enterPlayModeDelegate = new(Capacity32);
        private static readonly List<Action> exitEditModeDelegate = new(Capacity32);
        private static readonly List<Action> enterEditModeDelegate = new(Capacity32);
        private static readonly List<Action<BuildReportData>> buildPreprocessorCallbacks = new(Capacity4);
        private static readonly List<Action<BuildReportData>> buildPostprocessorCallbacks = new(Capacity4);


        #region Asset Handling

        private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath,
            UnityEditor.RemoveAssetOptions options)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            try
            {
                BeforeDeleteAsset?.Invoke(assetPath, asset);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            UnregisterInternal(asset);

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion
    }
#endif
}