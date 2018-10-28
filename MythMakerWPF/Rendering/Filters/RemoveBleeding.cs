using MythMaker.Math;
using System.Drawing.Imaging;

namespace MythMaker.Rendering.Filters
{
    public class RemoveBleeding : IFilter
    {
        private Vector bleeding;

        public RemoveBleeding(float bleeding)
        {
            this.bleeding = new Vector(bleeding, bleeding);
        }

        public RemoveBleeding(Vector bleeding)
        {
            this.bleeding = bleeding;
        }

        unsafe void IFilter.Run(BitmapData bmpData, float scaling)
        {
            IntVector bleeding = (IntVector)(this.bleeding * scaling);

            byte* p = (byte*)(void*)bmpData.Scan0;

            for (var y = 0; y < bmpData.Height; y++)
            {
                for (var x = 0; x < bmpData.Width; x++)
                {
                    int start = y * bmpData.Stride + x * 4;

                    int distX = System.Math.Min(x, bmpData.Width - x - 1);
                    int distY = System.Math.Min(y, bmpData.Height - y - 1);

                    // edges
                    if (distX < bleeding.X || distY < bleeding.Y)
                    {
                        p[start + 3] = 0;
                    }
                    // corners
                    else if (distX < bleeding.X * 2 && distY < bleeding.Y * 2)
                    {
                        distX = -distX + bleeding.X * 2;
                        distY = -distY + bleeding.Y * 2;
                        bool inside = System.Math.Sqrt(distX * distX + distY * distY) <= (bleeding.X + bleeding.Y) / 2;
                        p[start + 3] = inside ? (byte)255 : (byte)0;
                    }
                }
            }
        }
    }
}
