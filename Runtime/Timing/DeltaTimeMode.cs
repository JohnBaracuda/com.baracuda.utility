using System;
using UnityEngine;

namespace Baracuda.Utility.Timing
{
    public enum DeltaTimeMode
    {
        DeltaTime = 0,
        FixedDeltaTime = 1,
        SmoothDeltaTime = 2,
        UnscaledDeltaTime = 3
    }

    public static class DeltaTimeModeExtension
    {
        public static float Value(this DeltaTimeMode deltaTimeMode)
        {
            switch (deltaTimeMode)
            {
                case DeltaTimeMode.DeltaTime:
                    return Time.deltaTime;

                case DeltaTimeMode.FixedDeltaTime:
                    return Time.fixedDeltaTime;

                case DeltaTimeMode.SmoothDeltaTime:
                    return Time.smoothDeltaTime;

                case DeltaTimeMode.UnscaledDeltaTime:
                    return Time.unscaledDeltaTime;

                default:
                    throw new ArgumentOutOfRangeException(nameof(deltaTimeMode), deltaTimeMode, null);
            }
        }
    }
}