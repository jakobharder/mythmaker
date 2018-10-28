using MythMaker.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Rendering.Utils
{
    class BitmapUtils
    {
        public static Color GetAverageColor(Bitmap bmp, IntRect region)
        {
            var bmpData = bmp.LockBits(region,
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var color = GetAverageColor(bmpData, region);
            bmp.UnlockBits(bmpData);
            return color;
        }

        public static unsafe Color GetAverageColor(BitmapData bmpData, IntRect region)
        {
            Color c = Color.FromArgb(255, 0, 0, 0);

            byte* p = (byte*)(void*)bmpData.Scan0;

            float r = 0;
            float g = 0;
            float b = 0;
            for (int y = region.Y; y < region.Bottom; y++)
            {
                byte* line = p + y * bmpData.Stride;

                float lr = 0;
                float lg = 0;
                float lb = 0;
                for (int x = region.X; x < region.Right; x++)
                {
                    lb += line[x * 4 + 0];
                    lg += line[x * 4 + 1];
                    lr += line[x * 4 + 2];
                }
                r += lr / region.Width;
                g += lg / region.Width;
                b += lb / region.Width;
            }
            r /= region.Height;
            g /= region.Height;
            b /= region.Height;

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }
    }
}
