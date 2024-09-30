using System;
using UnityEngine.AddressableAssets;

namespace Baracuda.Utility.Scenes
{
    [Serializable]
    public class AssetReferenceScene
#if UNITY_EDITOR
        : AssetReferenceT<UnityEditor.SceneAsset>
#else
    : AssetReference
#endif
    {
        public AssetReferenceScene(string guid) : base(guid)
        {
        }
    }
}