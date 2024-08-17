using UnityEngine;

namespace Baracuda.Utilities.Editor.Utilities
{
    public static partial class GUIUtility
    {
        public static bool HideMonoScript { get; set; } = false;

        public static Color LightColor { get; } = new(.1f, .1f, .1f, .9f);
        public static Color DarkColor { get; } = new(.2f, .2f, .2f, .5f);
        public static Color ActiveButtonColor { get; } = new(0.76f, 0.87f, 1f);

        public static GUIContent EmptyContent { get; } = new();

        public static GUIStyle HelpBoxStyle => helpBox ??= Create("HelpBox");
        public static GUIStyle BoxStyle => box ??= Create("Box");
        public static GUIStyle BoldTitleStyle => boldTitleStyle ??= Create(GUI.skin.label, 16, FontStyle.Bold);
        public static GUIStyle BoldLabelStyle => boldLabelStyle ??= Create(GUI.skin.label, null, FontStyle.Bold);
        public static GUIStyle BoldButtonStyle => boldButtonStyle ??= Create(GUI.skin.button, 16, FontStyle.Bold);

        public static GUIStyle BoldFoldoutStyle =>
            boldFoldoutStyle ??= Create("Foldout", fontStyle: FontStyle.Normal, fontSize: 14);

        public static GUIStyle RichTextStyle => richTextStyle ??= Create(GUI.skin.label, 15, default, true);

        public static GUIStyle RichTextArea =>
            richTextArea ??= Create(GUI.skin.textArea, 15, default, true);

        public static GUIStyle TextArea => textArea ??= Create(GUI.skin.textArea, 15, default, false);
        public static GUIStyle SearchBarStyle => searchBarStyle ??= Create("SearchTextField");

        private static GUIStyle Create(GUIStyle other, int? fontSize = null, FontStyle? fontStyle = null,
            bool? richText = null)
        {
            return new GUIStyle(other)
            {
                fontSize = fontSize ?? other.fontSize,
                fontStyle = fontStyle ?? other.fontStyle,
                richText = richText ?? other.richText
            };
        }

        private static GUIStyle boldButtonStyle;
        private static GUIStyle boldTitleStyle;
        private static GUIStyle boldLabelStyle;
        private static GUIStyle boldFoldoutStyle;
        private static GUIStyle helpBox;
        private static GUIStyle box;
        private static GUIStyle richTextStyle;
        private static GUIStyle richTextArea;
        private static GUIStyle textArea;
        private static GUIStyle searchBarStyle;
    }
}