using MythMaker.Math;
using System;
using System.Drawing.Imaging;

namespace MythMaker.Rendering.Filters
{
    public class RecolorFast : IFilter
    {
        private ColorRGB color;
        private float hue;

        public RecolorFast(float hue)
        {
            this.hue = hue;
            color = new ColorHSL(hue, 1, 0.18f).ToRGB();
        }

        unsafe void IFilter.Run(BitmapData bmpData, float scaling)
        {
            byte* p = (byte*)(void*)bmpData.Scan0;

            for (var y = 0; y < bmpData.Height; y++)
            {
                for (var x = 0; x < bmpData.Width; x++)
                {
                    int start = y * bmpData.Stride + x * 4;

                    float _Min = System.Math.Min(System.Math.Min(p[start + 2], p[start + 1]), p[start + 0]) / 255.0f;
                    float _Max = System.Math.Max(System.Math.Max(p[start + 2], p[start + 1]), p[start + 0]) / 255.0f;
                    float L = (float)((_Max + _Min) / 2.0f);

                    //float L = System.Math.Min(0.49999f, (p[start + 2] + p[start + 1] + p[start + 0]) / 255.0f / 3);

                    float H = hue;

                    float t2;
                    float th = H / 6;

                    t2 = L * 2 * 255.0f;

                    float tr, tg, tb;
                    tr = th + (1.0f / 3.0f);
                    if (tr > 1) tr -= 1f;
                    tg = th;
                    tb = (th - (1.0f / 3.0f));
                    if (tb < 0) tb += 1f;

                    //tr = ColorCalc(tr, 0, t2);
                    if (6.0f * tr < 1.0f) tr = 6.0f * tr;
                    else if (2.0f * tr < 1.0f) tr = 1;
                    else if (1.5f * tr < 1.0f) tr = (2.0f / 3.0f - tr) * 6.0f;
                    else tr = 0;
                    //tg = ColorCalc(tg, 0, t2);
                    if (6.0f * tg < 1.0f) tg = 6.0f * tg;
                    else if (2.0f * tr < 1.0f) tg = 1;
                    else if (1.5f * tr < 1.0f) tg = (2.0f / 3.0f - tg) * 6.0f;
                    else tg = 0;
                    //tb = ColorCalc(tb, 0, t2);
                    if (6.0f * tb < 1.0f) tb = 6.0f * tb;
                    else if (2.0f * tb < 1.0f) tb = 1;
                    else if (1.5f * tb < 1.0f) tb = (2.0f / 3.0f - tb) * 6.0f;
                    else tb = 0;

                    p[start + 2] = (byte)(tr * t2);
                    p[start + 1] = (byte)(tg * t2);
                    p[start + 0] = (byte)(tb * t2);
                }
            }
        }
    }
}
