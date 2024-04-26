using System.Text.RegularExpressions;

namespace Baracuda.Utilities
{
    public static class MarkdownUtility
    {
        // Convert Markdown text to TMP rich text and set it to TextMesh
        public static string ConvertMarkdownToTMP(string markdownText)
        {
            var text = markdownText;

            text = ConvertHorizontalLine(text);
            text = ConvertHeaders(text);
            text = ConvertBold(text);
            text = ConvertItalic(text);
            text = ConvertUnderscore(text);
            text = ConvertBulletPoints(text);
            text = ConvertBlockQuotes(text);
            //text = AddExtraSpaceAfterParagraphs(text);

            return text;
        }

        // Convert Markdown headers to TMP format
        private static string ConvertHeaders(string text)
        {
            text = Regex.Replace(text, @"^# (.*?)$", "<size=150%>$1</size>", RegexOptions.Multiline);
            text = Regex.Replace(text, @"^## (.*?)$", "<size=125%>$1</size>", RegexOptions.Multiline);
            text = Regex.Replace(text, @"^### (.*?)$", "<size=110%>$1</size>", RegexOptions.Multiline);
            return text;
        }

        // Convert Markdown bullet points to TMP format, including support for indented sub-points
        private static string ConvertBulletPoints(string text)
        {
            // First, replace top-level bullet points
            text = Regex.Replace(text, @"^\* (.*?)$", "• $1", RegexOptions.Multiline);
            text = Regex.Replace(text, @"^- (.*?)$", "• $1", RegexOptions.Multiline);

            // text = Regex.Replace(text, @"^(?<!\t|\s)\* (.*?)$", "• $1", RegexOptions.Multiline);
            // text = Regex.Replace(text, @"^(?<!\t|\s)- (.*?)$", "• $1", RegexOptions.Multiline);

            // Now, replace indented sub-level bullet points
            // You can adjust the indent, size, or bullet character here as needed
            text = Regex.Replace(text, @"^(\t|\s+)\* (.*?)$", "<indent=20pt>◦ $2</indent>", RegexOptions.Multiline);
            text = Regex.Replace(text, @"^(\t|\s+)- (.*?)$", "<indent=20pt>◦ $2</indent>", RegexOptions.Multiline);

            return text;
        }

        // Convert Markdown bold to TMP format
        private static string ConvertBold(string text)
        {
            return Regex.Replace(text, @"\*\*(.*?)\*\*", "<b>$1</b>", RegexOptions.Multiline);
        }

        // Convert Markdown italic to TMP format
        private static string ConvertItalic(string text)
        {
            return Regex.Replace(text, @"\*(.*?)\*", "<i>$1</i>", RegexOptions.Multiline);
        }

        // Convert Markdown blockquotes to TMP format with indentation
        private static string ConvertBlockQuotes(string text)
        {
            return Regex.Replace(text, @"^> (.*?)$", "<indent=20pt><i>$1</i></indent>",
                RegexOptions.Multiline);
        }

        // Convert Markdown horizontal rules to TMP format
        private static string ConvertHorizontalLine(string text)
        {
            return Regex.Replace(text, @"^-{3,}$",
                "────────────────────────────────────────────────",
                RegexOptions.Multiline);
        }

        // Convert underscores to TMP format
        private static string ConvertUnderscore(string text)
        {
            text = Regex.Replace(text, @"~~(.*?)~~", "<u>$1</u>", RegexOptions.Multiline);
            return Regex.Replace(text, @"__(.*?)__", "<u>$1</u>", RegexOptions.Multiline);
        }

        // Method to add extra half-line space after paragraphs
        private static string AddExtraSpaceAfterParagraphs(string text)
        {
            // Adds a half-line space after paragraphs. Adjust <line-height> as needed for half-spacing.
            return Regex.Replace(text, @"(.*?)(\r\n|\r|\n)(?!$|\r\n|\r|\n|(\*|-|\d+\.|\t| {4,}) )",
                "$1$2<line-height=50%>\n<line-height=100%>", RegexOptions.Multiline);
        }
    }
}