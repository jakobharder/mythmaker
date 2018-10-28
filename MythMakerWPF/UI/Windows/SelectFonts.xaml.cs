using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MythMaker.Rendering;
using System.Drawing;
using System.Runtime.InteropServices;
using Image = System.Drawing.Image;
using System.IO;
using MythMaker.Math;

namespace MythMaker
{
    public static class ImageConverter
    {
        public static BitmapSource GetBitmapSource(Bitmap bmp)
        {
            var bitmapData = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bmp.HorizontalResolution, bmp.VerticalResolution,
                System.Windows.Media.PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bmp.UnlockBits(bitmapData);
            return bitmapSource;
        }
    }

    public static class MythAppUtils
    {
        public static PointF GetDPI(Window window)
        {
            PresentationSource source = PresentationSource.FromVisual(window);
            PointF dpi = new PointF(192, 192);
            if (source != null)
            {
                dpi.X = (float)(96.0 * source.CompositionTarget.TransformToDevice.M11);
                dpi.Y = (float)(96.0 * source.CompositionTarget.TransformToDevice.M22);
            }
            return dpi;
        }
    }

/// <summary>
/// Interaction logic for SelectFonts.xaml
/// </summary>
public partial class SelectFonts : Window
    {
        private bool freeze = true;
        public string MasonSans { get; set; }
        public string HelveticaNeueCn { get; set; }
        public string HelveticaNeueMdCn { get; set; }

        public SelectFonts()
        {
            MasonSans = MythMaker.Rendering.Fonts.Settings.Mason;
            HelveticaNeueCn = MythMaker.Rendering.Fonts.Settings.HelCn;
            HelveticaNeueMdCn = MythMaker.Rendering.Fonts.Settings.HelMdCn;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string arialNarrow = null;
            foreach (var family in System.Drawing.FontFamily.Families)
            {
                cmbMason.Items.Add(family.Name);
                cmbHelvetica.Items.Add(family.Name);
                cmbHelveticaMd.Items.Add(family.Name);

                if ((MasonSans == null || MasonSans == "") && family.Name.Contains("Mason") && family.Name.Contains("Sans") && family.Name.Contains("Bold"))
                    MasonSans = family.Name;
                if ((HelveticaNeueCn == null || HelveticaNeueCn == "") && family.Name.Contains("Helvetica") && family.Name.Contains("57"))
                    HelveticaNeueCn = family.Name;
                if ((HelveticaNeueMdCn == null || HelveticaNeueMdCn == "") && family.Name.Contains("Helvetica") && family.Name.Contains("67"))
                    HelveticaNeueMdCn = family.Name;
                if (family.Name.Contains("Arial") && family.Name.Contains("Narrow"))
                    arialNarrow = family.Name;
            }

            if (HelveticaNeueCn == null)
                HelveticaNeueCn = arialNarrow;
            if (HelveticaNeueMdCn == null)
                HelveticaNeueMdCn = HelveticaNeueCn;

            cmbMason.SelectedItem = MasonSans;
            cmbHelvetica.SelectedItem = HelveticaNeueCn;
            cmbHelveticaMd.SelectedItem = HelveticaNeueMdCn;
            freeze = false;
            updateText();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MythMaker.Rendering.Fonts.Settings.Update(MasonSans, HelveticaNeueCn, HelveticaNeueMdCn);
            Close();
        }


        private void updateText()
        {
            PointF dpi = MythAppUtils.GetDPI(this);

            int scaleFactor = 1;
            Bitmap original = new Bitmap(Image.FromFile("resources/font-selection.png"));

            Render r = new Render(400, 400, 2);
            r.Graphics.Clear(Color.White);

            SolidBrush black = new SolidBrush(Color.Black);
            System.Drawing.Font mason = new System.Drawing.Font(MasonSans, 8.75f * scaleFactor);
            System.Drawing.Font helCn = new System.Drawing.Font(HelveticaNeueCn, 5.82f * scaleFactor, System.Drawing.FontStyle.Regular);
            System.Drawing.Font helCnO = new System.Drawing.Font(HelveticaNeueCn, 5.82f * scaleFactor, System.Drawing.FontStyle.Italic);
            System.Drawing.Font helMdCnO = new System.Drawing.Font(HelveticaNeueMdCn, 5.82f * scaleFactor, System.Drawing.FontStyle.Italic);
            System.Drawing.Font helBdCn = new System.Drawing.Font(HelveticaNeueCn, 5.82f * scaleFactor, System.Drawing.FontStyle.Bold);

            r.DrawImage(original, new Math.Vector(0, 0), new Math.Vector(400, 400), Alignment.TopLeft);

            scaleFactor = 2;
            r.Graphics.DrawString("Mason Sans", mason, black, new RectangleF(14 * scaleFactor, 9 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));
            r.Graphics.DrawString("Item Names etc.", mason, black, new RectangleF(14 * scaleFactor, 46 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));
            r.Graphics.DrawString("Helvetica Neue Condensed\r\nDescriptions", helCn, black, new RectangleF(20.5f * scaleFactor, 101 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));
            r.Graphics.DrawString("Helvetica Neue Condensed\r\nDescriptions", helCnO, black, new RectangleF(20.5f * scaleFactor, 174 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));
            r.Graphics.DrawString("Helvetica Neue Condensed\r\nItem Card Bonus etc.", helMdCnO, black, new RectangleF(20.5f * scaleFactor, 246 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));
            r.Graphics.DrawString("Helvetica Neue Condensed\r\nKeywords", helBdCn, black, new RectangleF(20.5f * scaleFactor, 318 * scaleFactor, 400 * scaleFactor, 80 * scaleFactor));


            var pictureSize = new IntVector((int)(400 / 96.0f * dpi.X), (int)(400 / 96.0f * dpi.Y));


            using (Render scale = new Render(pictureSize.X, pictureSize.Y, 2))
            {
                scale.DrawImage(r.Bitmap, new Math.Vector(0, 0), pictureSize, Alignment.TopLeft);
                textPreview.Source = ImageConverter.GetBitmapSource(scale.Bitmap);
            }

            btnOK.IsEnabled = Fonts.Validate(MasonSans, HelveticaNeueCn, HelveticaNeueMdCn);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (freeze)
                return;

            MasonSans = cmbMason.SelectedItem as string;
            HelveticaNeueCn = cmbHelvetica.SelectedItem as string;
            HelveticaNeueMdCn = cmbHelveticaMd.SelectedItem as string;
            updateText();
        }
    }
}
