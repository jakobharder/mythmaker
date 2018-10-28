using MythMaker.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace MythMaker.Rendering
{
    public enum Alignment
    {
        TopLeft,
        TopCenter,
        MiddleCenter,
        MiddleLeft
    }

    [Flags]
    public enum MythFontStyle
    {
        Regular = 1,
        SemiBold = 2,
        Bold = 4,
        Centered = 8,
        Mason = 16,
        MasonSmall = 32,
        DropShadow = 64,
        Italic = 128,
        Small = 256,
        AutoScale = 512,
        Middle = 1024,
        MasonBig = 2048
    }

    public partial class Render : IDisposable
    {
        public Graphics Graphics { get; private set; }
        public Bitmap Bitmap { get; private set; }

        private float scaling;
        private SolidBrush brushBlack;
        private System.Drawing.Font[] helCn, helCnO, helMdCnO, helBdCn;
        private float[] lineHeight = new float[] { 29.6f * 2, 25.1f * 2 }; /* factor = 2 */
        private float[] paragraphMargin = new float[] { 6.0f * 2, 10 * 2 }; /* factor = 2 */

        private System.Drawing.Font smHelBdCn;
        private System.Drawing.Font masonNormal;
        private System.Drawing.Font masonSmall;
        private System.Drawing.Font masonBig;

        public class TextPiece
        {
            public string Text;
            public System.Drawing.Font Font;
            public float LineHeight;
            public float ParagraphMargin;

            public TextPiece()
            {
            }

            public TextPiece(string text, Rendering.Font font, Rendering.FontStyle style)
            {
                Text = text;
                Font = font.GetFont(style);
                LineHeight = font.LineHeight;
                ParagraphMargin = font.ParagraphMargin;
            }
        }

        public Render(int width, int height, float scaling)
        {
            this.scaling = scaling;

            Bitmap = new Bitmap((int)(scaling * width), (int)(scaling * height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap.SetResolution(scaling * 300, scaling * 300);
            Graphics = Graphics.FromImage(Bitmap);
            Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Graphics.PixelOffsetMode = PixelOffsetMode.None;

            brushBlack = new SolidBrush(Color.FromArgb(255, 35, 31, 32));
            helCn = new System.Drawing.Font[] {
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 5.82f, System.Drawing.FontStyle.Regular),
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 4.83f, System.Drawing.FontStyle.Regular)
            };
            helCnO = new System.Drawing.Font[] {
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 5.82f, System.Drawing.FontStyle.Italic),
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 4.83f, System.Drawing.FontStyle.Italic)
            };
            helMdCnO = new System.Drawing.Font[] {
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueMdCn"] as string, 5.82f, System.Drawing.FontStyle.Italic),
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueMdCn"] as string, 4.83f, System.Drawing.FontStyle.Italic)
            };
            helBdCn = new System.Drawing.Font[] {
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 5.82f, System.Drawing.FontStyle.Bold),
                new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 4.83f, System.Drawing.FontStyle.Bold)
            };

            smHelBdCn = new System.Drawing.Font(Properties.Settings.Default["FontsHelveticaNeueCn"] as string, 4.9f, System.Drawing.FontStyle.Bold); // how to get BdCn???
            masonBig = new System.Drawing.Font(Properties.Settings.Default["FontsMasonSans"] as string, 8.75f);
            masonNormal = new System.Drawing.Font(Properties.Settings.Default["FontsMasonSans"] as string, 6.9f);
            masonSmall = new System.Drawing.Font(Properties.Settings.Default["FontsMasonSans"] as string, 5.86f);
        }
        
        public void DrawString(string text, SolidBrush brush, PointF position, MythFontStyle style)
        {
            PointF scaledPosition = new PointF(position.X * scaling, position.Y * scaling);
            if (brush == null)
                brush = brushBlack;

            var fmtCenter = new StringFormat();
            if ((style & MythFontStyle.Centered) == MythFontStyle.Centered)
                fmtCenter.Alignment = StringAlignment.Center;

            System.Drawing.Font font = helCnO[0];
            if ((style & MythFontStyle.SemiBold) == MythFontStyle.SemiBold)
                font = helMdCnO[0];
            else if ((style & MythFontStyle.Bold) == MythFontStyle.Bold)
                font = smHelBdCn;
            else if ((style & MythFontStyle.Mason) == MythFontStyle.Mason)
                font = masonNormal;
            else if ((style & MythFontStyle.MasonSmall) == MythFontStyle.MasonSmall)
                font = masonSmall;
            else if ((style & MythFontStyle.MasonBig) == MythFontStyle.MasonBig)
                font = masonBig;

            Graphics.DrawString(text, font, brush, scaledPosition, fmtCenter);
        }

        public void DrawString(string text, SolidBrush brush, RectangleF area, MythFontStyle style)
        {
            RectangleF scaledArea = new RectangleF(area.X * scaling, area.Y * scaling, area.Width * scaling, area.Height * scaling);
            if (brush == null)
                brush = brushBlack;

            var fmtCenter = new StringFormat();
            if ((style & MythFontStyle.Centered) == MythFontStyle.Centered)
                fmtCenter.Alignment = StringAlignment.Center;
            if ((style & MythFontStyle.Middle) == MythFontStyle.Middle)
                fmtCenter.LineAlignment = StringAlignment.Center;

            System.Drawing.Font font = helCnO[0];
            if ((style & MythFontStyle.SemiBold) == MythFontStyle.SemiBold)
                font = helMdCnO[0];
            else if ((style & MythFontStyle.Bold) == MythFontStyle.Bold)
                font = smHelBdCn;
            else if ((style & MythFontStyle.Mason) == MythFontStyle.Mason)
                font = masonNormal;
            else if ((style & MythFontStyle.MasonSmall) == MythFontStyle.MasonSmall)
                font = masonSmall;
            else if ((style & MythFontStyle.MasonBig) == MythFontStyle.MasonBig)
                font = masonBig;
            else if ((style & MythFontStyle.Regular) == MythFontStyle.Regular)
                font = helCn[0];

            if ((style & MythFontStyle.DropShadow) == MythFontStyle.DropShadow)
            {
                RectangleF prerenderRect = new RectangleF(0, 0, scaledArea.Width, scaledArea.Height);
                Bitmap prerender = new Bitmap((int)scaledArea.Width, (int)scaledArea.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                prerender.SetResolution(scaling * 300, scaling * 300);
                Graphics preg = Graphics.FromImage(prerender);
                preg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                preg.Clear(Color.Transparent);
                preg.DrawString(text, font, brushBlack, prerenderRect, fmtCenter);
                //g.DrawImage(prerender, scaledArea.Location);
                Bitmap shadowMap = DropShadow.CreateShadow(prerender, (int)(6 * scaling), 1, 1.5f);
                Graphics.DrawImage(shadowMap, new RectangleF(scaledArea.X - (shadowMap.Width - scaledArea.Width) / 2, 
                    scaledArea.Y - (shadowMap.Height - scaledArea.Height) / 2, shadowMap.Width, shadowMap.Height));
            }

            if ((style & MythFontStyle.AutoScale) == MythFontStyle.AutoScale)
            {
                font = masonBig;
                SizeF measured = Graphics.MeasureString(text, font);
                if (measured.Width > scaledArea.Width)
                {
                    font = masonNormal;
                    measured = Graphics.MeasureString(text, font);
                    if (measured.Width > scaledArea.Width)
                    {
                        font = masonSmall;
                    }
                }
            }

            Graphics.DrawString(text, font, brush, scaledArea, fmtCenter);
        }

        public void DrawString(string text, SolidBrush color, Rendering.Font font, Vector position, Alignment alignment)
        {
            DrawString(text, color, font, position, Rendering.FontStyle.Regular, alignment);
        }

        public void DrawString(string text, SolidBrush color, Rendering.Font font, Vector position, Rendering.FontStyle style, Alignment alignment)
        { 
            PointF scaledPoint = new PointF(position.X * scaling, position.Y * scaling);
            if (color == null)
                color = brushBlack;

            var fmtCenter = new StringFormat();
            if (alignment == Alignment.TopCenter || alignment == Alignment.MiddleCenter)
                fmtCenter.Alignment = StringAlignment.Center;
            if (alignment == Alignment.MiddleLeft || alignment == Alignment.MiddleCenter)
                fmtCenter.LineAlignment = StringAlignment.Center;

            System.Drawing.Font drawingFont = font.GetFont(style);

            Graphics.DrawString(text, drawingFont, color, scaledPoint, fmtCenter);
        }

        public SizeF DrawBox(string text, RectangleF area, MythFontStyle style, Alignment alignment, TextParsePattern[] patterns = null)
        {
            if (alignment == Alignment.MiddleLeft)
            {
                SizeF measured = drawBox(text, area, style, patterns, true);
                RectangleF middle = new RectangleF(area.X, area.Y + (area.Height - measured.Height) / 2, area.Width, measured.Height);
                //g.FillRectangle(SystemBrushes.Highlight, new RectangleF(middle.X * scaling, middle.Y * scaling, middle.Width * scaling, middle.Height * scaling));
                return drawBox(text, middle, style, patterns, false);
            }

            return drawBox(text, area, style, patterns, false);
        }

        public SizeF MeasureBox(string text, SizeF area, MythFontStyle style, Alignment alignment, TextParsePattern[] patterns = null)
        {
            return drawBox(text, new RectangleF(new PointF(0, 0), area), style, patterns, true);
        }

        public SizeF drawBox(string text, RectangleF area, MythFontStyle style, TextParsePattern[] patterns, bool dryRun)
        {
            //g.FillRectangle(SystemBrushes.Highlight, new RectangleF(area.X * scaling, area.Y * scaling, area.Width * scaling, area.Height * scaling));
            int smallFont = ((style & MythFontStyle.Small) == MythFontStyle.Small) ? 1 : 0;

            SizeF rendered = new SizeF();
            TextPiece[] pieces = parseText(text, style, patterns);

            PointF position = new PointF(area.Location.X * scaling, area.Location.Y * scaling);

            Random rand = new Random();

            StringFormat fmt = new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces,
                Trimming = StringTrimming.Word
            };

            SizeF margin = new SizeF(4 * scaling, 0);

            TextPiece remaining = null;
            for (int index = 0; index < pieces.Length;)
            {
                var piece = remaining ?? pieces[index];
                if (piece.Text.Length == 0)
                {
                    index++;
                    continue;
                }
                else if (piece.Text == "\r\n\r\n")
                {
                    position.X = area.Location.X * scaling;
                    position.Y += lineHeight[smallFont] + paragraphMargin[smallFont];
                    index++;
                    continue;
                }

                SizeF layout = new SizeF(area.Width * scaling - position.X + area.Location.X * scaling, lineHeight[smallFont]);

                SizeF size = Graphics.MeasureString(piece.Text, piece.Font, layout, fmt, out int chars, out int lines);
                if (chars == 0)
                    break; // something went wrong
                
                if (chars == piece.Text.Length)
                {
                    if (!dryRun)
                    {
                        if (piece.Font == Fonts.Myth[smallFont])
                            Graphics.DrawString(piece.Text, piece.Font, brushBlack, position - margin + new SizeF(-1 * scaling, 1 * scaling), fmt);
                        else
                            Graphics.DrawString(piece.Text, piece.Font, brushBlack, position - margin, fmt);
                    }
                    position.X += size.Width - margin.Width * 2;
                    if (piece.Text == "a" && piece.Font == Fonts.Myth[smallFont])
                        position.X -= 3 * scaling;
                    rendered.Width = System.Math.Max(rendered.Width, position.X / scaling - area.X);
                    remaining = null;
                    index++;
                }
                else
                {
                    if (!dryRun)
                        Graphics.DrawString(piece.Text.Substring(0, chars), piece.Font, brushBlack, position - margin, fmt);
                    position.X += size.Width - margin.Width * 2;
                    if (piece.Text == "a" && piece.Font == Fonts.Myth[smallFont])
                        position.X -= 3 * scaling;
                    rendered.Width = System.Math.Max(rendered.Width, position.X / scaling - area.X);
                    position.X = area.Location.X * scaling;
                    position.Y += lineHeight[smallFont];
                    remaining = new TextPiece() { Text = piece.Text.Substring(chars), Font = piece.Font };
                }
            }
            
            rendered.Height = (position.Y - area.Y * scaling + lineHeight[smallFont]) / scaling;
            return rendered;
        }

        public Vector DrawText(string text, SolidBrush color, Rendering.Font font, Vector position, Alignment alignment)
        {
            TextPiece[] pieces = new TextPiece[] { new TextPiece(text, font, Rendering.FontStyle.Regular) };
            return DrawText(pieces, color, position, new Vector(0, 0));
        }

        public Vector DrawText(string text, SolidBrush color, Rendering.Font font, Vector position, Vector size, Rendering.FontStyle style)
        {
            TextPiece[] pieces = new TextPiece[] { new TextPiece(text, font, style) };
            return DrawText(pieces, color, position, size);
        }

        public Vector DrawText(TextPiece[] textPieces, SolidBrush color, Vector position, Vector size, 
            TextParsePattern[] patterns = null, bool dryRun = false)
        {
            Vector rendered = new Vector(0, 0);

            position *= scaling;
            size *= scaling;

            Vector start = position;

            Random rand = new Random();

            StringFormat fmt = new StringFormat
            {
                FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces,
                Trimming = StringTrimming.Word
            };

            Vector margin = new Vector(4 * scaling, 0);

            TextPiece remaining = null;
            for (int index = 0; index < textPieces.Length;)
            {
                var piece = remaining ?? textPieces[index];
                if (piece.Text.Length == 0)
                {
                    index++;
                    continue;
                }
                else if (piece.Text == "\n\n" || piece.Text == "\r\n\r\n")
                {
                    position.X = start.X;
                    position.Y += (piece.LineHeight + piece.ParagraphMargin) * scaling;
                    index++;
                    continue;
                }

                SizeF layout = new SizeF(size.X - position.X + start.X, piece.LineHeight * scaling);
                SizeF measured = Graphics.MeasureString(piece.Text, piece.Font, layout, fmt, out int chars, out int lines);
                if (chars == 0)
                    break; // something went wrong

                if (chars == piece.Text.Length)
                {
                    if (!dryRun)
                    {
                        //if (piece.Font == Fonts.Myth[smallFont])
                        //    g.DrawString(piece.Text, piece.Font, color, position - margin + new SizeF(-1 * scaling, 1 * scaling), fmt);
                        //else
                        Graphics.DrawString(piece.Text, piece.Font, color, position - margin, fmt);
                    }
                    position.X += measured.Width - margin.X * 2;
                    // special handling for D10, FD.... should be done in font.....
                    //if (piece.Text == "a" && piece.Font == Fonts.Myth[smallFont])
                    //    position.X -= 3 * scaling;
                    rendered.X = System.Math.Max(rendered.X, position.X - start.X);
                    remaining = null;
                    index++;
                }
                else
                {
                    if (!dryRun)
                        Graphics.DrawString(piece.Text.Substring(0, chars), piece.Font, color, position - margin, fmt);
                    position.X += measured.Width - margin.X * 2;
                    // special handling for D10
                    //if (piece.Text == "a" && piece.Font == Fonts.Myth[smallFont])
                    //    position.X -= 3 * scaling;
                    rendered.X = System.Math.Max(rendered.X, position.X - start.X);
                    position.X = start.X;
                    position.Y += piece.LineHeight * scaling;
                    remaining = new TextPiece() { Text = piece.Text.Substring(chars), Font = piece.Font,
                        LineHeight = piece.LineHeight, ParagraphMargin = piece.ParagraphMargin };
                }
            }

            rendered.Y = (position.Y - start.Y + textPieces[textPieces.Length - 1].LineHeight * scaling);

            rendered /= scaling;
            return rendered;
        }

        private TextPiece[] parseText(string text, MythFontStyle style, TextParsePattern[] patterns)
        {
            int smallFont = ((style & MythFontStyle.Small) == MythFontStyle.Small) ? 1 : 0;

            string keywords = MythKeywords.GetRegex();
            string extraPatterns = patterns != null ? patterns.Join() : "";

            string[] output = Regex.Split(text, "(" + Regex.Escape("\r\n\r\n") + "|" + keywords + "|D10|FD" + extraPatterns + ")");

            bool italic = (style & MythFontStyle.Italic) == MythFontStyle.Italic;

            var list = new List<TextPiece>();

            foreach (var element in output)
            {
                if (element.Length == 0)
                    continue;


                MythFontStyle? match = null;
                if (element == "D10")
                    list.Add(new TextPiece() { Text = "a", Font = Fonts.Myth[smallFont] });
                else if (element == "FD")
                    list.Add(new TextPiece() { Text = "b", Font = Fonts.Myth[smallFont] });
                else if (MythKeywords.IsKeyword(element))
                    list.Add(new TextPiece() { Text = element, Font = helBdCn[smallFont] });
                else if ((match = patterns.GetMatch(element)) != null)
                {
                    if ((match.Value & MythFontStyle.SemiBold) == MythFontStyle.SemiBold)
                        list.Add(new TextPiece() { Text = element, Font = helMdCnO[smallFont] });
                    else if ((match.Value & MythFontStyle.Italic) == MythFontStyle.Italic)
                        list.Add(new TextPiece() { Text = element, Font = helCnO[smallFont] });
                    else if ((match.Value & MythFontStyle.Bold) == MythFontStyle.Bold)
                        list.Add(new TextPiece() { Text = element, Font = helBdCn[smallFont] });
                }
                else
                    list.Add(new TextPiece() { Text = element, Font = italic ? helCnO[smallFont] : helCn[smallFont] });
            }

            return list.ToArray();
        }

        public TextPiece[] ParseText(string text, Font font, TextParsePattern2[] patterns, FontStyle defaultStyle = FontStyle.Regular)
        {
            string keywords = MythKeywords.GetRegex();
            string extraPatterns = patterns != null ? patterns.Join() : "";

            string[] output = Regex.Split(text, "(" + Regex.Escape("\r\n\r\n") + "|" + Regex.Escape("\n\n") + extraPatterns + "|" + keywords + "|D10|FD)");

            var list = new List<TextPiece>();

            foreach (var element in output)
            {
                if (element.Length == 0)
                    continue;

                FontStyle? match = null;
                if (element == "D10")
                    list.Add(new TextPiece() { Text = "a", Font = Fonts.Myth[1], LineHeight = font.LineHeight, ParagraphMargin = font.ParagraphMargin });
                else if (element == "FD")
                    list.Add(new TextPiece() { Text = "b", Font = Fonts.Myth[1], LineHeight = font.LineHeight, ParagraphMargin = font.ParagraphMargin });
                else if (MythKeywords.IsKeyword(element))
                    list.Add(new TextPiece(element, font, FontStyle.Bold));
                else if ((match = patterns.GetMatch(element)) != null)
                    list.Add(new TextPiece(element, font, match.Value));
                else
                    list.Add(new TextPiece(element, font, defaultStyle));
            }

            return list.ToArray();
        }

        void IDisposable.Dispose()
        {
            Graphics.Dispose();
            Bitmap.Dispose();
        }
    }
}
