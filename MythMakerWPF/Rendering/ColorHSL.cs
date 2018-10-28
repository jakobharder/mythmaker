using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// based on: https://stackoverflow.com/questions/4793729/rgb-to-hsl-and-back-calculation-problems

namespace MythMaker.Rendering
{
    public class ColorHSL
    {
        public float H;
        public float S;
        public float L;

        public ColorHSL(System.Drawing.Color color)
        {
            FromRGB(color.R, color.G, color.B);
        }

        public ColorHSL(float hue, float saturation, float lightness)
        {
            H = hue;
            S = saturation;
            L = lightness;
        }

        public ColorHSL(byte r, byte g, byte b)
        {
            FromRGB(r, g, b);
        }

        private void FromRGB(byte R, byte G, byte B)
        {
            float _R = (R / 255f);
            float _G = (G / 255f);
            float _B = (B / 255f);

            float _Min = System.Math.Min(System.Math.Min(_R, _G), _B);
            float _Max = System.Math.Max(System.Math.Max(_R, _G), _B);
            float _Delta = _Max - _Min;

            H = 0;
            S = 0;
            L = (float)((_Max + _Min) / 2.0f);

            if (_Delta != 0)
            {
                if (L < 0.5f)
                {
                    S = (float)(_Delta / (_Max + _Min));
                }
                else
                {
                    S = (float)(_Delta / (2.0f - _Max - _Min));
                }


                if (_R == _Max)
                {
                    H = (_G - _B) / _Delta;
                }
                else if (_G == _Max)
                {
                    H = 2f + (_B - _R) / _Delta;
                }
                else if (_B == _Max)
                {
                    H = 4f + (_R - _G) / _Delta;
                }
            }
        }

        public ColorRGB ToRGB()
        {
            byte r, g, b;
            if (S == 0)
            {
                r = (byte)System.Math.Round(L * 255d);
                g = (byte)System.Math.Round(L * 255d);
                b = (byte)System.Math.Round(L * 255d);
            }
            else
            {
                double t1, t2;
                double th = H / 6.0d;

                if (L < 0.5d)
                {
                    t2 = L * (1d + S);
                }
                else
                {
                    t2 = (L + S) - (L * S);
                }
                t1 = 2d * L - t2;

                double tr, tg, tb;
                tr = th + (1.0d / 3.0d);
                tg = th;
                tb = th - (1.0d / 3.0d);

                tr = ColorCalc(tr, t1, t2);
                tg = ColorCalc(tg, t1, t2);
                tb = ColorCalc(tb, t1, t2);
                r = (byte)System.Math.Round(tr * 255d);
                g = (byte)System.Math.Round(tg * 255d);
                b = (byte)System.Math.Round(tb * 255d);
            }
            return new ColorRGB(r, g, b);
        }

        private static double ColorCalc(double c, double t1, double t2)
        {
            if (c < 0) c += 1d;
            if (c > 1) c -= 1d;
            if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
            if (2.0d * c < 1.0d) return t2;
            if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
            return t1;
        }
    }
}
