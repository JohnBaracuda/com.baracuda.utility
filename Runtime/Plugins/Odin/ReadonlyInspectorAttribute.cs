using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System;

namespace Baracuda.Bedrock.Odin
{
    [ReadOnly]
    [ShowInInspector]
    [IncludeMyAttributes]
    [MeansImplicitUse]
    public class ReadonlyInspectorAttribute : Attribute
    {
    }

    [ReadOnly]
    [ShowInInspector]
    [IncludeMyAttributes]
    [MeansImplicitUse]
    [ShowIf("@UnityEngine.Application.isPlaying")]
    public class DebugAttribute : Attribute
    {
    }
}