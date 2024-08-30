using System;
using Baracuda.Bedrock.Types;
using Object = UnityEngine.Object;

namespace Baracuda.Bedrock.PlayerLoop
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
    public sealed partial class Gameloop : UnityEditor.AssetModificationProcessor
    {
        public delegate void WillDeleteAssetCallback(string assetPath, Object asset);

        private static readonly Broadcast editorUpdateCallbacks = new(Capacity32);
        private static readonly Broadcast exitPlayModeDelegate = new(Capacity32);
        private static readonly Broadcast enterPlayModeDelegate = new(Capacity32);
        private static readonly Broadcast exitEditModeDelegate = new(Capacity32);
        private static readonly Broadcast enterEditModeDelegate = new(Capacity32);
        private static readonly Broadcast<WillDeleteAssetCallback> beforeDeleteAsset = new(Capacity4);


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

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion
    }
#endif
}