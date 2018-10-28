using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.IO;

namespace MythMaker.Myth
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class MythBitmap
    {
        public int Width { get { return width; } }
        public int Height {  get { return height; } }

        private byte[] data;
        private int width;
        private int height;
        private int stride;

        [DataMember(Name = "ID", IsRequired = true)]
        private string id;
        private string prefix;

        string lastID;

        private Bitmap temporary;

        // use with caution
        public Bitmap Image
        {
            get { return temporary; }
        }

        public MythDocument Document
        {
            get { return document; }
            set
            {
                document = value;
                prefix = document.FileName + ".data/";
            }
        }
        private MythDocument document;

        public static MythBitmap FromBitmap(Bitmap bitmap, string id, MythDocument document)
        {
            var result = new MythBitmap(new byte[] { }, 0, 0, 0);
            result.readBitmap(bitmap);
            result.id = id;
            result.Document = document;
            return result;
        }

        private MythBitmap(byte[] data, int width, int height, int stride)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.stride = stride;
        }

        public Bitmap GetBitmap(Math.Vector? scaling = null)
        {
            if (temporary == null)
                loadBitmap();

            Size size;
            if (!scaling.HasValue)
                size = new Size(width, height);
            else
                size = new Size((int)(width * scaling.Value.X), (int)(height * scaling.Value.Y));

            return GeneratePreview(size);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            string prefixTemp = Document.FileName + ".tmp.data/";
            
            string folder = Path.GetDirectoryName(prefixTemp + id);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (File.Exists(prefixTemp + id))
                File.Delete(prefixTemp + id);

            if (temporary != null)
            {
                // in memory, store it
                temporary.Save(prefixTemp + id);
            }
            else
            {
                // not in memory, copy from sources
                if (lastID == null)
                    lastID = id;
                File.Copy(prefix + lastID, prefixTemp + id);
            }

            // update source prefix, it may have changed
            prefix = Document.FileName + ".data/";
        }

        public bool Validate()
        {
            return File.Exists(Document.FileName + ".data/" + id);
        }

        public void UpdateID(string id)
        {
            lastID = this.id;
            this.id = id;
        }

        private void loadBitmap()
        {
            string prefix = Document.FileName + ".data/";
            using (Image img = System.Drawing.Image.FromFile(prefix + id))
                readBitmap(new Bitmap(img));
        }

        public Bitmap GeneratePreview(Size size)
        {
            if (temporary == null)
                loadBitmap();

            float scale = System.Math.Min((size.Width / (float)width), (size.Height / (float)height));

            Rectangle rect = new Rectangle();
            rect.Width = (int)(scale * width);
            rect.Height = (int)(scale * height);
            rect.X = (size.Width - rect.Width) / 2;
            rect.Y = (size.Height - rect.Height) / 2;

            Bitmap result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.DrawImage(temporary, rect);
            }
            return result;
        }

        public unsafe float GetAverageHue()
        {
            float averageHue = 0;

            var bmpData = temporary.LockBits(new Rectangle(0, 0, temporary.Width, temporary.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte* p = (byte*)(void*)bmpData.Scan0;

            for (var y = 0; y < bmpData.Height; y++)
            {
                float lineHue = 0;
                for (var x = 0; x < bmpData.Width; x += 4) // speed up a little
                {
                    int start = y * bmpData.Stride + x * 4;
                    var hsl = new Rendering.ColorHSL(p[start + 2], p[start + 1], p[start + 0]);
                    lineHue += hsl.H;
                }
                averageHue += lineHue / (bmpData.Width / 4);
            }

            temporary.UnlockBits(bmpData);

            return averageHue / temporary.Height;
        }

        private void readBitmap(Bitmap bitmap)
        {
            var bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int numbytes = bmpdata.Stride * bitmap.Height;
            data = new byte[numbytes];
            IntPtr ptr = bmpdata.Scan0;
            Marshal.Copy(ptr, data, 0, numbytes);
            bitmap.UnlockBits(bmpdata);

            stride = bmpdata.Stride;
            width = bitmap.Width;
            height = bitmap.Height;
            temporary = null;
            createTemporary();

            bitmap.Dispose();
        }

        private void createTemporary()
        {
            temporary = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bmpdata = temporary.LockBits(new Rectangle(0, 0, temporary.Width, temporary.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int numbytes = bmpdata.Stride * temporary.Height;
            IntPtr ptr = bmpdata.Scan0;
            Marshal.Copy(data, 0, ptr, numbytes);
            temporary.UnlockBits(bmpdata);
        }
    }
}
