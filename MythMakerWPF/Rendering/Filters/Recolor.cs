using MythMaker.Math;
using System.Drawing.Imaging;

namespace MythMaker.Rendering.Filters
{
    public class Recolor : IFilter
    {
        private float hue;

        public Recolor(float hue)
        {
            this.hue = hue;
        }

        unsafe void IFilter.Run(BitmapData bmpData, float scaling)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            for (var y = 0; y < bmpData.Height; y++)
            {
                for (var x = 0; x < bmpData.Width; x++)
                {
                    int start = y * bmpData.Stride + x * 4;

                    ColorHSL hsl = new ColorHSL(p[start + 2], p[start + 1], p[start + 0]);
                    hsl.H = hue;
                    ColorRGB rgb = hsl.ToRGB();

                    p[start + 2] = rgb.R;
                    p[start + 1] = rgb.G;
                    p[start + 0] = rgb.B;
                }
            }
        }
    }
}
