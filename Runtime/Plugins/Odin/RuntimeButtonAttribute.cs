using Sirenix.OdinInspector;
using System;

namespace Baracuda.Bedrock.Odin
{
    [EnableIf("@UnityEngine.Application.isPlaying")]
    [Button]
    [IncludeMyAttributes]
    public class RuntimeButtonAttribute : Attribute
    {
    }
}