using System;
using System.Runtime.CompilerServices;

namespace Baracuda.Utilities
{
    public static class CommandLineUtility
    {
        public static class CommandLineUtils
        {
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
        }
    }
}