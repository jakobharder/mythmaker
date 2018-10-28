using MythMaker.Math;
using System.Drawing;
using System.Drawing.Imaging;

namespace MythMaker.Rendering
{
    public partial class Render
    {
        public void DrawImage(Image image, Vector position, Vector size, Alignment alignment)
        {
            PointF p = new PointF();
            if (alignment == Alignment.TopLeft || alignment == Alignment.MiddleLeft)
                p.X = position.X;
            else
                p.X = position.X - size.X * 0.5f;
            if (alignment == Alignment.TopCenter || alignment == Alignment.TopLeft)
                p.Y = position.Y;
            else
                p.Y = position.Y - size.Y * 0.5f;

            Graphics.DrawImage(image, new RectangleF(
                scaling * p.X,
                scaling * p.Y,
                scaling * size.X,
                scaling * size.Y));
        }

        #region DrawImage overloads
        public void DrawImage(Image image, Vector position, Alignment alignment)
        {
            DrawImage(image, new Rect(position.X, position.Y, image.Size.Width, image.Size.Height), alignment);
        }

        public void DrawImage(Image image, Rect area, Alignment alignment)
        {
            DrawImage(image, area.Location, area.Size, alignment);
        }
        #endregion

        public void DrawFastImage(Image image, Vector position)
        {
            var previousMode = Graphics.InterpolationMode;
            Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            Graphics.DrawImage(image, new RectangleF(
                scaling * position.X,
                scaling * position.Y,
                scaling * image.Width,
                scaling * image.Height));
            Graphics.InterpolationMode = previousMode;
        }

        public void DrawColorized(Image image, Vector position, float hue)
        {
            // https://stackoverflow.com/questions/1079820/rotate-hue-using-imageattributes-in-c-sharp
            const float wedge = 120f / 360;

            var hueDegree = -((hue - 191.0f) / 360.0f) % 1;
            if (hueDegree < 0) hueDegree += 1;

            var matrix = new float[5][];

            if (hueDegree <= wedge)
            {
                //Red..Green
                var theta = hueDegree / wedge * (System.Math.PI / 2);
                var c = (float)System.Math.Cos(theta);
                var s = (float)System.Math.Sin(theta);

                matrix[0] = new float[] { c, 0, s, 0, 0 };
                matrix[1] = new float[] { s, c, 0, 0, 0 };
                matrix[2] = new float[] { 0, s, c, 0, 0 };
                matrix[3] = new float[] { 0, 0, 0, 1, 0 };
                matrix[4] = new float[] { 0, 0, 0, 0, 1 };

            }
            else if (hueDegree <= wedge * 2)
            {
                //Green..Blue
                var theta = (hueDegree - wedge) / wedge * (System.Math.PI / 2);
                var c = (float)System.Math.Cos(theta);
                var s = (float)System.Math.Sin(theta);

                matrix[0] = new float[] { 0, s, c, 0, 0 };
                matrix[1] = new float[] { c, 0, s, 0, 0 };
                matrix[2] = new float[] { s, c, 0, 0, 0 };
                matrix[3] = new float[] { 0, 0, 0, 1, 0 };
                matrix[4] = new float[] { 0, 0, 0, 0, 1 };

            }
            else
            {
                //Blue..Red
                var theta = (hueDegree - 2 * wedge) / wedge * (System.Math.PI / 2);
                var c = (float)System.Math.Cos(theta);
                var s = (float)System.Math.Sin(theta);

                matrix[0] = new float[] { s, c, 0, 0, 0 };
                matrix[1] = new float[] { 0, s, c, 0, 0 };
                matrix[2] = new float[] { c, 0, s, 0, 0 };
                matrix[3] = new float[] { 0, 0, 0, 1, 0 };
                matrix[4] = new float[] { 0, 0, 0, 0, 1 };
            }

            var ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(new ColorMatrix(matrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var previousMode = Graphics.InterpolationMode;
            Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            Graphics.DrawImage(image,
                (IntRect)(new Rect(position.X, position.Y, image.Width, image.Height) * scaling),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, ia);
            Graphics.InterpolationMode = previousMode;
        }

        private ColorMatrix GetHueShiftColorMax(float hueShiftDegrees)
        {
            /* Return the matrix

                A00  A01  A02  0  0
                A10  A11  A12  0  0
                A20  A21  A22  0  0
                  0    0    0  1  0
                  0    0    0  0  1
            */
            float theta = hueShiftDegrees / 360 * 2 * (float)System.Math.PI; //Degrees --> Radians
            float c = (float)System.Math.Cos(theta);
            float s = (float)System.Math.Sin(theta);

            float A00 = 0.213f + 0.787f * c - 0.213f * s;
            float A01 = 0.213f - 0.213f * c + 0.413f * s;
            float A02 = 0.213f - 0.213f * c - 0.787f * s;

            float A10 = 0.715f - 0.715f * c - 0.715f * s;
            float A11 = 0.715f + 0.285f * c + 0.140f * s;
            float A12 = 0.715f - 0.715f * c + 0.715f * s;

            float A20 = 0.072f - 0.072f * c + 0.928f * s;
            float A21 = 0.072f - 0.072f * c - 0.283f * s;
            float A22 = 0.072f + 0.928f * c + 0.072f * s;

            ColorMatrix cm = new ColorMatrix(new float[][]
                  {
                      new float[] { A00, A01, A02, 0, 0 },
                      new float[] { A10, A11, A12, 0, 0},
                      new float[] { A20, A21, A22, 0, 0},
                      new float[] { 0, 0, 0, 0, 0},
                      new float[] { 0, 0, 0, 0, 1}
                  }
            );

            return cm;
        }
    }
}
