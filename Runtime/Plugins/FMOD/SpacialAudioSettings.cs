using System;
using UnityEngine;

namespace Baracuda.Bedrock.FMOD
{
    [Serializable]
    public struct SpacialAudioSettings
    {
        public float volume;
        public float radius;
        public AnimationCurve falloff;
    }
}