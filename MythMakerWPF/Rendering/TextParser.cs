using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MythMaker.Rendering
{
    public class TextParsePattern
    {
        public string Pattern;
        public MythFontStyle Style;

        public TextParsePattern(string pattern, MythFontStyle style)
        {
            Pattern = pattern;
            Style = style;
        }

        public MythFontStyle? GetMatch(string text)
        {
            bool matched = Regex.Match(text, Pattern).Success;
            return matched ? (MythFontStyle?)(Style) : null;
        }
    }

    public class TextParsePattern2
    {
        public string Pattern;
        public Rendering.FontStyle Style;

        public TextParsePattern2(string pattern, FontStyle style)
        {
            Pattern = pattern;
            Style = style;
        }

        public FontStyle? GetMatch(string text)
        {
            bool matched = Regex.Match(text, Pattern).Success;
            return matched ? (FontStyle?)(Style) : null;
        }
    }

    public static class TextParsePatternExtensions
    {
        public static string Join(this TextParsePattern[] patterns)
        {
            string str = "";
            foreach (var pattern in patterns)
                str += "|" + pattern.Pattern;
            return str;
        }

        public static MythFontStyle? GetMatch(this TextParsePattern[] patterns, string text)
        {
            if (patterns == null)
                return null;

            foreach (var pattern in patterns)
            {
                var match = pattern.GetMatch(text);
                if (match != null)
                    return match;
            }
            return null;
        }
    }

    public static class TextParsePatternExtensions2
    {
        public static string Join(this TextParsePattern2[] patterns)
        {
            string str = "";
            foreach (var pattern in patterns)
                str += "|" + pattern.Pattern;
            return str;
        }

        public static FontStyle? GetMatch(this TextParsePattern2[] patterns, string text)
        {
            if (patterns == null)
                return null;

            foreach (var pattern in patterns)
            {
                var match = pattern.GetMatch(text);
                if (match != null)
                    return match;
            }
            return null;
        }
    }

    class TextParser
    {
    }
}
