using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Baracuda.Utility.Pools;
using UnityEngine;
using UnityEngine.Pool;

namespace Baracuda.Utility.Utilities
{
    public static class StringExtensions
    {
        #region String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ValidStringOrDefault(this string input, string defaultValue)
        {
            return input.IsNotNullOrWhitespace() ? input : defaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhitespace(this string input)
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

        public static void CopyToClipboard(this string value, bool removeRichText = true)
        {
            GUIUtility.systemCopyBuffer = removeRichText ? value.RemoveRichText() : value;
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

        public static string Dehumanize(this string value)
        {
            value = value.Trim();

            var result = new StringBuilder();
            var shouldCapitalizeNext = true;

            foreach (var character in value)
            {
                if (character is '_' or ' ')
                {
                    shouldCapitalizeNext = true;
                }
                else if (shouldCapitalizeNext)
                {
                    result.Append(char.ToUpper(character, CultureInfo.CurrentCulture));
                    shouldCapitalizeNext = false;
                }
                else
                {
                    result.Append(character);
                }
            }

            return result.ToString().Replace(" ", "");
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

            var chars = ListPool<char>.Get();

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
                        if ((char.IsUpper(target[i]) && !char.IsUpper(target[i + 1]))
                            || (char.IsUpper(target[i]) && !char.IsUpper(target[i - 1])))
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
            ListPool<char>.Release(chars);
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
            var sb = StringBuilderPool.Get();
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

            return StringBuilderPool.BuildAndRelease(sb);
        }

        #endregion


        #region StringBuilder

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendIf(this StringBuilder stringBuilder, char value, bool condition)
        {
            return condition ? stringBuilder.Append(value) : stringBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendIf(this StringBuilder stringBuilder, string value, bool condition)
        {
            return condition ? stringBuilder.Append(value) : stringBuilder;
        }

        #endregion


        #region Collections

        public static string CombineToString(this IEnumerable<string> enumerable, string separator = " ")
        {
            if (enumerable == null)
            {
                return string.Empty;
            }
            var stringBuilder = StringBuilderPool.Get();
            foreach (var argument in enumerable)
            {
                stringBuilder.Append(argument);
                stringBuilder.Append(separator);
            }

            return StringBuilderPool.BuildAndRelease(stringBuilder).TrimEnd(separator.ToCharArray());
        }

        public static string[] RemoveNullOrWhiteSpace(this IEnumerable<string> enumerable, char separator = ' ')
        {
            var list = ListPool<string>.Get();

            foreach (var value in enumerable)
            {
                if (value.IsNotNullOrWhitespace())
                {
                    list.Add(value);
                }
            }

            var result = list.ToArray();
            ListPool<string>.Release(list);
            return result;
        }

        #endregion


        #region Hashing

        /// <summary>
        ///     Computes the FNV-1a hash for the input string.
        ///     The FNV-1a hash is a non-cryptographic hash function known for its speed and good distribution properties.
        ///     Useful for creating Dictionary keys instead of using strings.
        ///     https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        /// </summary>
        /// <param name="value">The input string to hash.</param>
        /// <returns>An integer representing the FNV-1a hash of the input string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeFnv1AHash(this string value)
        {
            var hash = 2166136261;
            for (var index = 0; index < value.Length; index++)
            {
                hash = (hash ^ value[index]) * 16777619;
            }
            return unchecked((int)hash);
        }

        #endregion


        #region Rich Text

        public static Color SoftWhite { get; } = new(0.92f, 0.92f, 0.95f);

        public static Color HotPink { get; } = new(1f, 0.41f, 0.71f);
        public static Color DeepPink { get; } = new(1f, 0.08f, 0.58f);
        public static Color MediumVioletRed { get; } = new(0.78f, 0.08f, 0.52f);
        public static Color MediumSlateBlue { get; } = new(0.48f, 0.41f, 0.93f);
        public static Color TypeMagenta { get; } = new(0.76f, 0.57f, 1f);
        public static Color LightSkyBlue { get; } = new(0.64f, 0.81f, 0.98f);
        public static Color SteelBlue { get; } = new(0.27f, 0.51f, 0.71f);
        public static Color CornflowerBlue { get; } = new(0.39f, 0.58f, 0.93f);
        public static Color DarkSlateBlue { get; } = new(0.28f, 0.24f, 0.55f);
        public static Color VarBlue { get; } = new(0.52f, 0.56f, 0.91f);
        public static Color DarkGrey { get; } = new(0.29f, 0.29f, 0.29f);
        public static Color Gold { get; } = new(1f, 0.92f, 0.62f);
        public static Color Coral { get; } = new(1f, 0.5f, 0.31f);
        public static Color Tomato { get; } = new(1f, 0.39f, 0.28f);
        public static Color OrangeRed { get; } = new(1f, 0.27f, 0f);
        public static Color SoftLime { get; } = new(0.53f, 1f, 0.71f);

        /*
         *  RichTextExtensions operations
         */

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string Colorize(this string content)
        // {
        //     return Debug.Colorize(content, VarBlue);
        // }
        //
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static string Colorize(this string content, Color color)
        // {
        //     return Debug.Colorize(content, color);
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Colorize(this string content, LogType type)
        {
            var stringBuilder = StringBuilderPool.Get();
            stringBuilder.Append("<color=#");
            stringBuilder.Append(ColorUtility.ToHtmlStringRGBA(type.ToColor()));
            stringBuilder.Append('>');
            stringBuilder.Append(content);
            stringBuilder.Append("</color>");
            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToColorBool(this bool value)
        {
            return value.ToString().Colorize(value ? Color.green : Color.red);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Bold(this string content)
        {
            var stringBuilder = StringBuilderPool.Get();
            stringBuilder.Append("<b>");
            stringBuilder.Append(content);
            stringBuilder.Append("</b>");
            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Italics(this string content)
        {
            var stringBuilder = StringBuilderPool.Get();
            var str = stringBuilder;
            str.Append("<c>");
            str.Append(content);
            str.Append("</c>");
            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Strike(this string content)
        {
            var stringBuilder = StringBuilderPool.Get();
            stringBuilder.Append("<s>");
            stringBuilder.Append(content);
            stringBuilder.Append("</s>");
            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Underline(this string content)
        {
            var stringBuilder = StringBuilderPool.Get();
            stringBuilder.Append("<u>");
            stringBuilder.Append(content);
            stringBuilder.Append("</u>");
            return StringBuilderPool.BuildAndRelease(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FontSize(this string content, int size)
        {
            return $"<size={size.ToString()}>{content}</size>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToRichTextPrefix(this Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexCode(this Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }

        /*
         *  RichTextExtensions removal
         */

        public static string RemoveRichText(this string content)
        {
            content = RemoveRichTextDynamicTag(content, "color");
            content = RemoveRichTextTag(content, "b");
            content = RemoveRichTextTag(content, "i");

            // Text Mesh Pro
            content = RemoveRichTextDynamicTag(content, "align");
            content = RemoveRichTextDynamicTag(content, "size");
            content = RemoveRichTextDynamicTag(content, "cspace");
            content = RemoveRichTextDynamicTag(content, "font");
            content = RemoveRichTextDynamicTag(content, "indent");
            content = RemoveRichTextDynamicTag(content, "line-height");
            content = RemoveRichTextDynamicTag(content, "line-indent");
            content = RemoveRichTextDynamicTag(content, "link");
            content = RemoveRichTextDynamicTag(content, "margin");
            content = RemoveRichTextDynamicTag(content, "margin-left");
            content = RemoveRichTextDynamicTag(content, "margin-right");
            content = RemoveRichTextDynamicTag(content, "mark");
            content = RemoveRichTextDynamicTag(content, "mspace");
            content = RemoveRichTextDynamicTag(content, "noparse");
            content = RemoveRichTextDynamicTag(content, "nobr");
            content = RemoveRichTextDynamicTag(content, "page");
            content = RemoveRichTextDynamicTag(content, "pos");
            content = RemoveRichTextDynamicTag(content, "space");
            content = RemoveRichTextDynamicTag(content, "sprite index");
            content = RemoveRichTextDynamicTag(content, "sprite name");
            content = RemoveRichTextDynamicTag(content, "sprite");
            content = RemoveRichTextDynamicTag(content, "style");
            content = RemoveRichTextDynamicTag(content, "voffset");
            content = RemoveRichTextDynamicTag(content, "width");
            content = RemoveRichTextTag(content, "u");
            content = RemoveRichTextTag(content, "s");
            content = RemoveRichTextTag(content, "sup");
            content = RemoveRichTextTag(content, "sub");
            content = RemoveRichTextTag(content, "allcaps");
            content = RemoveRichTextTag(content, "smallcaps");
            content = RemoveRichTextTag(content, "uppercase");

            return content;
        }

        private static string RemoveRichTextDynamicTag(this string content, string tag)
        {
            while (true)
            {
                var index = content.IndexOf($"<{tag}=", StringComparison.Ordinal);
                if (index != -1)
                {
                    var endIndex = content.Substring(index, content.Length - index).IndexOf('>');
                    if (endIndex > 0)
                    {
                        content = content.Remove(index, endIndex + 1);
                    }

                    continue;
                }

                content = RemoveRichTextTag(content, tag, false);
                return content;
            }
        }

        private static string RemoveRichTextTag(this string content, string tag, bool isStart = true)
        {
            while (true)
            {
                var index = content.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>", StringComparison.Ordinal);
                if (index != -1)
                {
                    content = content.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                    continue;
                }

                if (isStart)
                {
                    content = RemoveRichTextTag(content, tag, false);
                }

                return content;
            }
        }

        public static Color ToColor(this LogType logType)
        {
            return logType switch
            {
                LogType.Log => SoftWhite,
                LogType.Error => Color.red,
                LogType.Exception => Color.red,
                LogType.Assert => Color.red,
                LogType.Warning => Color.yellow,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }

        #endregion
    }
}