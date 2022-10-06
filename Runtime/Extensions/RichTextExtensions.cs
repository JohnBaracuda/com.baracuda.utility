using System;
using System.Runtime.CompilerServices;
using Baracuda.Utilities.Pooling;
using UnityEngine;

namespace Baracuda.Utilities.Extensions
{
    public static class RichTextExtensions
    {
        /*
         *  Color Presets
         */

        public static Color SoftWhite { get; } = new Color(0.92f, 0.92f, 0.95f);
        
        public static Color HotPink { get; } = new Color(1f, 0.41f, 0.71f);
        public static Color DeepPink { get; } = new Color(1f, 0.08f, 0.58f);
        public static Color MediumVioletRed { get; } = new Color(0.78f, 0.08f, 0.52f);
        public static Color MediumSlateBlue { get; } = new Color(0.48f, 0.41f, 0.93f);
        public static Color TypeMagenta { get; } = new Color(0.76f, 0.57f, 1f);
        public static Color LightSkyBlue { get; } = new Color(0.64f, 0.81f, 0.98f);
        public static Color SteelBlue { get; } = new Color(0.27f, 0.51f, 0.71f);
        public static Color CornflowerBlue { get; } = new Color(0.39f, 0.58f, 0.93f);
        public static Color DarkSlateBlue { get; } = new Color(0.28f, 0.24f, 0.55f);
        public static Color VarBlue { get; } = new Color(0.52f, 0.56f, 0.91f);
        public static Color DarkGrey { get; } = new Color(0.29f, 0.29f, 0.29f);
        public static Color Gold { get; } = new Color(1f, 0.92f, 0.62f);
        public static Color Coral { get; } = new Color(1f, 0.5f, 0.31f);
        public static Color Tomato { get; } = new Color(1f, 0.39f, 0.28f);
        public static Color OrangeRed { get; } = new Color(1f, 0.27f, 0f);
        public static Color SoftLime { get; } = new Color(0.53f, 1f, 0.71f);

        /*
         *  RichTextExtensions operations   
         */
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Colorize(this string content)
        {
            return content.Colorize(VarBlue);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Colorize(this string content, Color color)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<color=#");
            str.Append(ColorUtility.ToHtmlStringRGBA(color));
            str.Append('>');
            str.Append(content);
            str.Append("</color>");
            return str.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Colorize(this string content, LogType type)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<color=#");
            str.Append(ColorUtility.ToHtmlStringRGBA(type.ToColor()));
            str.Append('>');
            str.Append(content);
            str.Append("</color>");
            return str.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Bold(this string content)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<b>");
            str.Append(content);
            str.Append("</b>");
            return str.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Italics(this string content)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<c>");
            str.Append(content);
            str.Append("</c>");
            return str.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Strike(this string content)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<s>");
            str.Append(content);
            str.Append("</s>");
            return str.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Underline(this string content)
        {
            using var pooled = ConcurrentStringBuilderPool.GetDisposable();
            var str = pooled.Value;
            str.Append("<u>");
            str.Append(content);
            str.Append("</u>");
            return str.ToString();
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
    }
}