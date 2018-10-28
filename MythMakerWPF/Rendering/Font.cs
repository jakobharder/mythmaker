using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Rendering
{
    public enum FontFamily
    {
        Mason,
        Helvetica
    }

    public enum FontStyle
    {
        Regular = 0,
        SemiBold = 1,
        Bold = 2,
        Italic = 3,
        ItalicSemiBold = SemiBold + Italic,
        ItalicBold = Bold + Italic
    }

    public class Font
    {
        FontFamily family;
        float size;
        System.Drawing.Font[] fonts = new System.Drawing.Font[6];

        public Font(FontFamily family, float size, float lineHeight, float paragraphMargin = 0)
        {
            this.family = family;
            this.size = size;
            LineHeight = lineHeight;
            ParagraphMargin = paragraphMargin;

            if (family == FontFamily.Mason)
            {
                fonts[(int)FontStyle.Regular] = new System.Drawing.Font(Rendering.Fonts.Settings.Mason, size);
                fonts[(int)FontStyle.SemiBold] = fonts[(int)FontStyle.Regular];
                fonts[(int)FontStyle.Bold] = fonts[(int)FontStyle.Regular];
                fonts[(int)FontStyle.Italic] = fonts[(int)FontStyle.Regular];
                fonts[(int)FontStyle.ItalicSemiBold] = fonts[(int)FontStyle.Regular];
                fonts[(int)FontStyle.ItalicBold] = fonts[(int)FontStyle.Regular];
            }
            else if (family == FontFamily.Helvetica)
            {
                fonts[(int)FontStyle.Regular] = new System.Drawing.Font(Rendering.Fonts.Settings.HelCn, size);
                fonts[(int)FontStyle.SemiBold] = new System.Drawing.Font(Rendering.Fonts.Settings.HelMdCn, size);
                fonts[(int)FontStyle.Bold] = new System.Drawing.Font(Rendering.Fonts.Settings.HelCn, size, 
                    System.Drawing.FontStyle.Bold);
                fonts[(int)FontStyle.Italic] = new System.Drawing.Font(Rendering.Fonts.Settings.HelCn, size, 
                    System.Drawing.FontStyle.Italic);
                fonts[(int)FontStyle.ItalicSemiBold] = new System.Drawing.Font(Rendering.Fonts.Settings.HelMdCn, size, 
                    System.Drawing.FontStyle.Italic);
                fonts[(int)FontStyle.ItalicBold] = new System.Drawing.Font(Rendering.Fonts.Settings.HelCn, size, 
                    System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold);
            }
        }

        public float LineHeight { get; private set; }
        public float ParagraphMargin { get; private set; }

        public System.Drawing.Font GetFont(FontStyle style = FontStyle.Regular)
        {
            return fonts[(int)style];
        }
    }
}
