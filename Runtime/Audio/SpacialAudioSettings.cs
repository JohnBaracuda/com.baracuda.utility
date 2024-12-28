using System;
using UnityEngine;

namespace Baracuda.Utility.Audio
{
    [Serializable]
    public struct SpacialAudioSettings
    {
        public float decibel;
        public float radius;
        public AnimationCurve falloff;
    }
}