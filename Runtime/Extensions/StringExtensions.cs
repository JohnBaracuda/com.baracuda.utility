using Baracuda.Utilities.Pooling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Baracuda.Utilities
{
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ValidStringOrDefault(this string input, string defaultValue)
        {
            return input.IsNotNullOrWhitespace() ? input : defaultValue;
        }

        /*
         *  Null checks & validation
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this string input)
        {
            return input.Equals(string.Empty);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrWhitespace(this string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        /*
         *  Utility string operations
         */

        public static string NoSpace(this string str)
        {
            var len = str.Length;
            var src = str.ToCharArray();
            var dstIdx = 0;

            for (var i = 0; i < len; i++)
            {
                var ch = src[i];

                switch (ch)
                {
                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':

                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':

                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':

                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':

                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        continue;

                    default:
                        src[dstIdx++] = ch;
                        break;
                }
            }

            return new string(src, 0, dstIdx);
        }

        public static void CopyToClipboard(this string str, bool removeRichText = true)
        {
            GUIUtility.systemCopyBuffer = removeRichText ? str.RemoveRichText() : str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            if (source == null)
            {
                return false;
            }
            return source.Contains(toCheck, StringComparison.CurrentCultureIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsIgnoreCaseAndSpace(this string source, string toCheck)
        {
            if (source == null)
            {
                return false;
            }
            return source.NoSpace().Contains(toCheck.NoSpace(), StringComparison.CurrentCultureIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string current, string remove)
        {
            return current.Replace(remove, "");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveFromStart(this string target, string value)
        {
            return target.StartsWith(value) ? target.Remove(0, value.Length) : target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveFromEnd(this string target, string value)
        {
            return target.EndsWith(value) ? target.Remove(target.Length - value.Length, value.Length) : target;
        }

        /*
         *  Humanize
         */

        public static string ToCamel(this string content)
        {
            Span<char> chars = stackalloc char[content.Length];

            for (var i = 0; i < content.Length; i++)
            {
                var current = content[i];
                var last = i > 0 ? content[i - 1] : ' ';
                chars[i] = last == ' ' ? char.ToUpper(current) : char.ToLower(current);
            }

            return new string(chars);
        }

        public static string HumanizeIf(this string target, bool condition, string[] prefixes = null)
        {
            return condition ? target.Humanize(prefixes) : target;
        }

        public static string Humanize(this string target, string[] prefixes = null)
        {
            if (IsConst(target))
            {
                return target.Replace('_', ' ').ToLower().ToCamel();
            }

            if (prefixes != null)
            {
                for (var i = 0; i < prefixes.Length; i++)
                {
                    target = target.Replace(prefixes[i], string.Empty);
                }
            }


            target = target.Replace('_', ' ');

            var chars = UnityEngine.Pool.ListPool<char>.Get();

            for (var i = 0; i < target.Length; i++)
            {
                if (i == 0)
                {
                    chars.Add(char.ToUpper(target[i]));
                }
                else
                {
                    if (i < target.Length - 1)
                    {
                        if (char.IsUpper(target[i]) && !char.IsUpper(target[i + 1])
                            || char.IsUpper(target[i]) && !char.IsUpper(target[i - 1]))
                        {
                            if (i > 1)
                            {
                                chars.Add(' ');
                            }
                        }
                    }

                    chars.Add(target[i]);
                }
            }

            var array = chars.ToArray();
            UnityEngine.Pool.ListPool<char>.Release(chars);
            return new string(array).ReduceWhitespace();

            // nested methods

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsConst(string input)
            {
                for (var i = 0; i < input.Length; i++)
                {
                    var character = input[i];
                    if (!char.IsUpper(character) && character != '_')
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static string ReduceWhitespace(this string value)
        {
            var sb = ConcurrentStringBuilderPool.Get();
            var previousIsWhitespaceFlag = false;
            for (var i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    if (previousIsWhitespaceFlag)
                    {
                        continue;
                    }

                    previousIsWhitespaceFlag = true;
                }
                else
                {
                    previousIsWhitespaceFlag = false;
                }

                sb.Append(value[i]);
            }

            return ConcurrentStringBuilderPool.Release(sb);
        }

        #region StringBuilder

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendIf(this StringBuilder stringBuilder, char value, bool condition)
        {
            return condition ? stringBuilder.Append(value) : stringBuilder;
        }

        #endregion

        #region Collections

        public static string CombineToString(this IEnumerable<string> enumerable, char separator = ' ')
        {
            var stringBuilder = ConcurrentStringBuilderPool.Get();
            foreach (var argument in enumerable)
            {
                stringBuilder.Append(argument);
                stringBuilder.Append(separator);
            }

            return ConcurrentStringBuilderPool.Release(stringBuilder);
        }

        public static string[] RemoveNullOrWhiteSpace(this IEnumerable<string> enumerable, char separator = ' ')
        {
            var list = ConcurrentListPool<string>.Get();

            foreach (var value in enumerable)
            {
                if (value.IsNotNullOrWhitespace())
                {
                    list.Add(value);
                }
            }

            var result = list.ToArray();
            ConcurrentListPool<string>.Release(list);
            return result;
        }

        #endregion
    }
}