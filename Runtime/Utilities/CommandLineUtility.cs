using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Baracuda.Bedrock.Utilities
{
    public static class CommandLineUtility
    {
        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCommandLineValueFromKey(string key)
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index].Equals($"-{key}", StringComparison.OrdinalIgnoreCase))
                {
                    return args[index + 1];
                }
            }

            return string.Empty;
        }

        [PublicAPI]
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetCommandLineFlagFromKey(string key, bool defaultValue = false)
        {
            var args = Environment.GetCommandLineArgs();
            for (var index = 0; index < args.Length; index++)
            {
                if (args[index].Equals($"-{key}", StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(args[index + 1], out var result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }
    }
}