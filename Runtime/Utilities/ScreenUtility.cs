using System;
using System.Collections.Generic;
using System.Linq;
using Baracuda.Utility.PlayerLoop;
using Baracuda.Utility.Types;
using UnityEngine;

namespace Baracuda.Utility.Utilities
{
    public static class ScreenUtility
    {
        public static ResolutionComparer Comparer { get; } = new();
        public static ResolutionAspectComparer AspectComparer { get; } = new();
        public static ObservableValue<Resolution> ScreenResolution { get; } = new(Screen.currentResolution, AspectComparer);

        static ScreenUtility()
        {
            Gameloop.Update += UpdateScreenResolution;
        }

        public static IReadOnlyList<FullScreenMode> AvailableFullScreenModes { get; } = new[]
        {
#if UNITY_STANDALONE_WIN
            FullScreenMode.ExclusiveFullScreen,
#endif
            FullScreenMode.FullScreenWindow,

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            FullScreenMode.MaximizedWindow,
#endif
#if UNITY_STANDALONE
            FullScreenMode.Windowed,
#endif
        };

        public static Resolution[] GetUniqueResolutions()
        {
            return Screen.resolutions
                .Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
                .Distinct()
                .ToArray();
        }

        private static void UpdateScreenResolution()
        {
            var resolution = Screen.currentResolution;
            ScreenResolution.SetValue(resolution);
        }

        public static string GetResolutionWithAspectRatio(Resolution resolution)
        {
            var aspectRatio = GetAspectRatio(resolution);
            return $"{resolution.width} x {resolution.height} ({aspectRatio})";
        }

        public static string GetAspectRatio(Resolution resolution)
        {
            var gcd = GetGreatestCommonDivisor(resolution.width, resolution.height);
            var widthRatio = resolution.width / gcd;
            var heightRatio = resolution.height / gcd;

            return SimplifyAspectRatio(widthRatio, heightRatio);
        }

        private static string SimplifyAspectRatio(int widthRatio, int heightRatio)
        {
            Span<int> commonWidths = stackalloc int[] { 1, 4, 16, 21 };
            Span<int> commonHeights = stackalloc int[] { 1, 3, 9, 10 };

            var originalRatio = (double)widthRatio / heightRatio;
            var minDifference = double.MaxValue;
            var bestMatch = $"{widthRatio}:{heightRatio}";

            for (var i = 0; i < commonWidths.Length; i++)
            {
                for (var j = 0; j < commonHeights.Length; j++)
                {
                    var standardRatio = (double)commonWidths[i] / commonHeights[j];
                    var difference = Math.Abs(originalRatio - standardRatio);

                    if (difference < minDifference)
                    {
                        minDifference = difference;
                        bestMatch = $"{commonWidths[i]}:{commonHeights[j]}";
                    }
                }
            }

            return bestMatch;
        }

        private static int GetGreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static void SetResolution(Resolution resolution)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
    }
}