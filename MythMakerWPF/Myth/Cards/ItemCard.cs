using MythMaker.Math;
using MythMaker.Myth.Elements;
using MythMaker.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace MythMaker.Myth.Cards
{
    class UnitConv
    {
        static public float FromPt(float dpi300)
        {
            return dpi300 / 72 * 300;
        }

        static public float PtToPixel(float dpi300)
        {
            return (dpi300 / 72 * 300);
        }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class ItemCard : SmallCard
    {
        #region DataMember & Serialization
        [DataMember(Name = "Bonus", EmitDefaultValue = false)]
        string bonusForSerialization;
        [DataMember(Name = "Special", EmitDefaultValue = false)]
        ItemSpecial[] specialForSerialization;
        [DataMember(Name = "ItemColor", IsRequired = true)]
        private int itemColor;
        [DataMember(Name = "SellValue", EmitDefaultValue = false)]
        public int sellValue = 0;
        [DataMember(Name = "BuyValue", EmitDefaultValue = false)]
        public int buyValue = 0;
        [DataMember(Name = "Range", EmitDefaultValue = false)]
        public int range = 0;
        [DataMember(Name = "ItemType", IsRequired = false)]
        public int itemType = ItemType.Default;
        [DataMember(Name = "Class", EmitDefaultValue = false)]
        string classForSerialization;
        [DataMember(Name = "TN", EmitDefaultValue = false)]
        public int tn = 0;
        [DataMember(Name = "Vitality", EmitDefaultValue = false)]
        public int vitality = 0;

        [OnSerializing]
        protected new void OnSerializing(StreamingContext context)
        {
            base.OnSerializing(context);

            classForSerialization = _class.Length > 0 ? string.Join(", ", _class) : null;
            bonusForSerialization = bonus != "" ? bonus : null;
            specialForSerialization = special.Length > 0 ? special : null;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            bonusForSerialization = "";
            classForSerialization = "";
            specialForSerialization = new ItemSpecial[] { };
        }

        [OnDeserialized]
        public new void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);

            _class = classForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            bonus = bonusForSerialization;
            special = specialForSerialization;

            Init();

            foreach (var special in special)
                special.PropertyChanged += Special_PropertyChanged;
        }
        #endregion

        #region DataMember Component Properties
        public ItemColor ItemColor
        {
            get { return itemColor; }
            set
            {
                itemColor = value;
                OnCardPropertyChanged("ItemColor");
            }
        }

        public ItemType ItemType
        {
            get { return itemType; }
            set
            {
                itemType = value;
                OnCardPropertyChanged("ItemType");
            }
        }

        string bonus = "";
        public string Bonus
        {
            get { return bonus; }
            set
            {
                bonus = value;
                OnCardPropertyChanged("Bonus");
            }
        }

        string[] _class = new string[] { };
        public string[] Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnCardPropertyChanged("Class");
            }
        }

        public int BuyValue
        {
            get { return buyValue; }
            set
            {
                buyValue = value;
                OnCardPropertyChanged("BuyValue");
            }
        }

        public int SellValue
        {
            get { return sellValue; }
            set
            {
                sellValue = value;
                OnCardPropertyChanged("BuyValue");
            }
        }

        public int TN
        {
            get { return tn; }
            set
            {
                tn = value;
                OnCardPropertyChanged("TN");
            }
        }

        public int Vitality
        {
            get { return vitality; }
            set
            {
                vitality = value;
                OnCardPropertyChanged("Vitality");
            }
        }

        public Range Range
        {
            get { return new Range(range); }
            set
            {
                range = value;
                OnCardPropertyChanged("Range");
            }
        }

        ItemSpecial[] special = new ItemSpecial[] { };
        public ItemSpecial[] Special
        {
            get { return special; }
            set
            {
                if (special == null)
                    throw new System.Exception();

                foreach (var special in special)
                    special.PropertyChanged -= Special_PropertyChanged;
                special = value;
                foreach (var special in special)
                    special.PropertyChanged += Special_PropertyChanged;
                OnCardPropertyChanged("Special");
            }
        }

        private void Special_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCardPropertyChanged("Special");
        }
        #endregion

        public Bitmap CardImage;
        public Bitmap CardPreview;
        public static Size PreviewSize = new Size(590, 860);

        public ItemCard(MythDocument document) : base(document)
        {
            Init();
            title = "New Item";
            RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutFront);
        }

        private void Init()
        {
            imageAutoScale = new Vector(500, 450);
            imageIdealScale = new Vector(300, 300);
            resourcePrefix = "items/";
        }

        public override string ToString()
        {
            return Title;
        }

        #region private methods
        private string toValueString(int value)
        {
            if (value == 0)
                return "-";
            return value.ToString();
        }

        private void renderGeneral(Rendering.Render r, Graphics g)
        {
            // icons
            float iconWidth = 0;
            if (Document != null && Document.Logo != null)
            {
                iconWidth = 60;
                var projectLogo = Document.Logo.GeneratePreview(new Size(48, 48));
                r.DrawImage(projectLogo, new Vector(476, 100), Alignment.MiddleCenter);
            }

            // title
            SolidBrush brushWhite = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            r.DrawString(Title, brushWhite, new RectangleF(85.5f, 102.5f, 430 - iconWidth, 0), MythFontStyle.Mason | MythFontStyle.AutoScale | MythFontStyle.Middle);

            // type
            Image itemType = ItemType != 0 ? System.Drawing.Image.FromFile("resources/item-type-" + ItemType.ToString().ToLower() + ".png") : null;
            if (itemType != null)
                r.DrawImage(itemType, new Vector(473, 187), Alignment.MiddleCenter);

            // picture
            if (Picture != null)
            {
                Bitmap itemPicture = Picture.Image.GetBitmap(Picture.Scaling);
                Bitmap itemShadow = DropShadow.CreateShadow(itemPicture, 12, 1.0f, 1.5f);
                Vector itemPicturePosition = new Vector(590 / 2 + 0.5f, 257.5f) + Picture.ImageOffset;
                r.DrawImage(itemShadow, itemPicturePosition, Alignment.MiddleCenter);
                r.DrawImage(itemPicture, itemPicturePosition, Alignment.MiddleCenter);
            }

            // item class
            r.DrawString(string.Join(", ", Class), brushWhite, new RectangleF(2, 348, 590, 30), MythFontStyle.Centered | MythFontStyle.MasonSmall | MythFontStyle.DropShadow);

            // bonus text
            TextParsePattern[] patterns = new TextParsePattern[]
            {
                new TextParsePattern("[^,]+:", MythFontStyle.SemiBold)
            };
            r.DrawBox(Bonus, new RectangleF(92, 429, 390, 100), MythFontStyle.Italic, Alignment.TopLeft, patterns);

            // TN
            if (TN > 0)
            {
                Bitmap tn = new Bitmap(System.Drawing.Image.FromFile("resources/item-shield.png"));
                r.DrawImage(tn, new Vector(131, 187), Alignment.MiddleCenter);
                r.DrawString("+" + TN.ToString(), null, new PointF(130, 169.5f), MythFontStyle.Centered | MythFontStyle.MasonSmall);
            }

            // vitality
            if (Vitality > 0)
            {
                Bitmap vitality = new Bitmap(System.Drawing.Image.FromFile("resources/item-vitality.png"));
                r.DrawImage(vitality, new Vector(132, 336.5f), Alignment.MiddleCenter);
                r.DrawString("+" + Vitality.ToString(), brushWhite, new PointF(132, 329.5f), MythFontStyle.Centered | MythFontStyle.MasonSmall);
            }
        }

        private void renderBottom(Rendering.Render r, Graphics g)
        {
            // template
            Image buySell = System.Drawing.Image.FromFile("resources/item-buy-sell.png");
            r.DrawImage(buySell, new Rect(411.8f, 703.3f, 102, 77), Alignment.TopLeft);
            if (ItemType > 0)
                r.DrawString("Range", null, new PointF(87, 738), MythFontStyle.Mason);

            // content
            SolidBrush brushRed = new SolidBrush(Color.FromArgb(255, 210, 35, 41));
            r.DrawString(toValueString(BuyValue), brushRed, new RectangleF(423.1f, 723, 27, 22), MythFontStyle.Bold | MythFontStyle.Centered);
            r.DrawString(toValueString(SellValue), brushRed, new RectangleF(457.9f, 730, 27, 22), MythFontStyle.Bold | MythFontStyle.Centered);
            if (ItemType > 0)
            {
                string rangeText = Range == 10 ? "Tile" : toValueString(Range);
                r.DrawString(rangeText, brushRed, new PointF(171.5f, 738), MythFontStyle.Mason);
            }
        }

        private void renderSpecial(Rendering.Render r, Graphics g)
        {
            // template
            int factor = 2;
            System.Drawing.Font masonSpecial = new System.Drawing.Font(Fonts.Settings.Mason, 5.4f);
            SolidBrush brushColor = new SolidBrush(ItemColor.Color);
            Pen penColor = new Pen(brushColor, 3 * factor);

            string specialTitle = ItemType > 0 ? "SPECIAL" : "DESCRIPTION";
            g.DrawString(specialTitle, masonSpecial, brushColor, new PointF(factor * 87.5f, factor * 476.5f));
            g.DrawLine(penColor, factor * 91.5f, factor * 504.5f, factor * 499, factor * 504.5f);

            // content
            Image fateRecipeBackground = System.Drawing.Image.FromFile("resources/fate-recipe-background.png");
            float start = 507.5f;
            bool first = true;
            var patternList = new List<TextParsePattern>();
            patternList.Add(new TextParsePattern(System.Text.RegularExpressions.Regex.Escape("*") + "[^ ]+ ", MythFontStyle.Italic));
            foreach (var extraKeyword in Document.ExtraKeywords)
                patternList.Add(new TextParsePattern(extraKeyword, MythFontStyle.Bold));
            var patterns = patternList.ToArray();

            // measure content
            MythFontStyle smallFont = 0;
            SizeF totalMeasured = new SizeF(395, 0);
            foreach (var special in Special)
            {
                if (special.Fates.Length > 0)
                {
                    SizeF fateSize = new SizeF(69, 67);
                    int fatesPerLine = System.Math.Min(2, special.Fates.Length);

                    SizeF measured = r.MeasureBox(special.Text,
                        new SizeF(408 - fateSize.Width * fatesPerLine, 82),
                        MythFontStyle.Regular,
                        Alignment.MiddleLeft,
                        patterns);
                    measured.Height = System.Math.Max(measured.Height, 82 + (special.Fates.Length > 2 ? fateSize.Height : 0));

                    start += measured.Height + 2;
                }
                else
                {
                    SizeF rendered = r.MeasureBox(
                        special.Text,
                        new SizeF(400, 100),
                        first ? MythFontStyle.Italic : MythFontStyle.Regular,
                        Alignment.TopLeft,
                        patterns);
                    start += rendered.Height + 9;
                }
                first = false;
            }
            totalMeasured.Height = start - 507.5f;
            if (totalMeasured.Height >= 195)
                smallFont = MythFontStyle.Small;
            //g.FillRectangle(SystemBrushes.Highlight, new RectangleF(92 * factor, 507.5f * factor, totalMeasured.Width * factor, totalMeasured.Height * factor));

            // render content
            first = true;
            start = 507.5f;
            foreach (var special in Special)
            {
                if (special.Fates.Length > 0)
                {
                    Vector fateSize = new Vector(69, 67);
                    int fatesPerLine = System.Math.Min(2, special.Fates.Length);

                    SizeF measured = r.MeasureBox(special.Text,
                        new SizeF(408 - fateSize.X * fatesPerLine, 82),
                        MythFontStyle.Regular,
                        Alignment.MiddleLeft,
                        patterns);
                    measured.Height = System.Math.Max(measured.Height, 82 + (special.Fates.Length > 2 ? fateSize.Y : 0));

                    r.DrawImage(fateRecipeBackground, new Rect(590 / 2, start, fateRecipeBackground.Width, measured.Height), Alignment.TopCenter);
                    Vector fateOffset = new Vector(468.5f, start) - new Vector(fateSize.X, 0) * System.Math.Min(1, special.Fates.Length - 1);
                    for (int i = 0; i < special.Fates.Length; i++)
                    {
                        r.DrawImage(Myth.Fate.GetImage(special.Fates[i]), new Rect(fateOffset.X, fateOffset.Y, 83, 82), Alignment.TopCenter);
                        fateOffset.X += fateSize.X;
                        if (i == 1) // second row
                            fateOffset = new Vector(468.5f, start) - new Vector(fateSize.X / 2, -fateSize.Y);
                    }

                    r.DrawBox(
                        special.Text, 
                        new RectangleF(92, start, 408 - fateSize.Y * fatesPerLine, measured.Height), 
                        MythFontStyle.Regular | smallFont, 
                        Alignment.MiddleLeft, 
                        patterns);
                    start += measured.Height + 2;
                }
                else
                {
                    SizeF rendered = r.DrawBox(
                        special.Text, 
                        new RectangleF(92, start + (first ? 6 : 3), 400, 100), 
                        (first ? MythFontStyle.Italic : MythFontStyle.Regular) | smallFont, 
                        Alignment.TopLeft, 
                        patterns);
                    start += rendered.Height + 9;
                }
                first = false;
            }

            //if (smallFont != 0)
            //    g.DrawString("small font " + ((int)totalMeasured.Height).ToString(), masonSpecial, brushColor, new PointF(factor * 340.5f, factor * 476.5f));
        }

        public override Bitmap RenderFront(float scaling = 2)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Image background = ImageCache.Get("resources/item-background-" + ItemColor.ToString().ToLower() + ".png");
            Render r = new Render(590, 860, scaling);

            // template
            r.DrawFastImage(background, new Vector(0, 0));

            stopwatch.Stop();
            var elapsedTemplate = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // general part
            renderGeneral(r, r.Graphics);

            // special
            renderSpecial(r, r.Graphics);

            // range & buy/sell
            renderBottom(r, r.Graphics);

            // removeBleeding
            if (scaling > 1.1f) // todo, for now this hacky way
                r.ApplyFilter(new Rendering.Filters.RemoveBleeding(34));

            stopwatch.Stop();
            var elapsedContent = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            CardImage = r.Bitmap;

            // preview
            /*using (Graphics gr = Graphics.FromImage(preview))
            {
                Size size = new Size((int)(PreviewSize.Width / 192.0f * dpi.X), (int)(PreviewSize.Height / 192.0f * dpi.Y));

                gr.DrawImage(result, new Rectangle(0, 0, size.Width, size.Height));
                CardPreview = preview;
            }*/

            // full image
            {
                fullImage = ImageConverter.GetBitmapSource(r.Bitmap);
                fullImage.Freeze();
                //tinyPreview = fullImage;
                //OnPropertyChanged("TinyPreview");
            }

            stopwatch.Stop();
            var elapsedPreviews = stopwatch.ElapsedMilliseconds;

            return r.Bitmap;
        }

        public override void RenderPreview(float scaling = 2)
        {
            // tiny preview
            using (Render tiny = new Render(48, 48, scaling))
            {
                tiny.Graphics.Clear(Color.Transparent);

                if (Picture != null)
                {
                    Bitmap itemPicture = Picture.Image.GetBitmap(Picture.ImagePreScaling * 0.19f);
                    tiny.DrawImage(itemPicture, new Vector(24, 24), Alignment.MiddleCenter);
                }
                TinyPreview = ImageConverter.GetBitmapSource(tiny.Bitmap);
                TinyPreview.Freeze();
            }
        }

        /*public void GeneratePDF()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with MythMaker";
            PdfPage page = document.AddPage();
            page.Size = PageSize.A4;
            page.Width = 2480.3f;
            page.Height = 3507.9f;
            XGraphics g = XGraphics.FromPdfPage(page);

            Image background = System.Drawing.Image.FromFile("resources/item-background-brown.png");
            XFont masonTitle = new XFont("MasonSansOT-Bold", UnitConv.FromPt(9));
            XFont masonSpecial = new XFont("MasonSansOT-Bold", UnitConv.FromPt(5.5f));
            XFont helCnO = new XFont("HelveticaNeueLT Pro 57 Cn", UnitConv.FromPt(6), XFontStyle.Italic);

            XSolidBrush brushWhite = new XSolidBrush(Color.FromArgb(255, 255, 255, 255));
            XSolidBrush brushBrown = new XSolidBrush(Color.FromArgb(255, 129, 72, 22));
            XSolidBrush brushBlack = new XSolidBrush(Color.FromArgb(255, 0, 0, 0));

            XPen penBrown = new XPen(Color.FromArgb(255, 129, 72, 22), 2);

            // template
            g.DrawImage(background, new Rectangle(0, 0, 590, 860));
            g.DrawString("SPECIAL", masonSpecial, brushBrown, new PointF(87.5f, 476.5f));
            g.DrawLine(penBrown, 92, 505, 498, 505);

            // content
            g.DrawString(this.Title, masonTitle, brushWhite, new PointF(85, 81.3f));
            g.DrawString(this.Description, helCnO, brushBlack, new XRect(90, 513.5f, 390, 0));

            // Save the document...
            const string filename = "HelloWorld.pdf";
            document.Save(filename);
            // ...and start a viewer.
            //System.Diagnostics.Process.Start(filename);
        }*/
        #endregion
    }
}
