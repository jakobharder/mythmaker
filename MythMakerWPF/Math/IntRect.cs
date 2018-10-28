using System.Drawing;

namespace MythMaker.Math
{
    public class IntRect
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Vector Location
        {
            get { return new Vector(X, Y); }
        }

        public int Width { get; set; }

        public int Height { get; set; }
        public int Right
        {
            get { return X + Width - 1; }
        }
        public int Bottom
        {
            get { return Y + Height - 1; }
        }
        // only for pixel actions
        public int RightPixel
        {
            get { return X + Width - 1; }
        }
        public int BottomPixel
        {
            get { return Y + Height - 1; }
        }

        public IntRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static explicit operator IntRect(Rect rect)
        {
            return new IntRect((int)(rect.X + 0.5f), (int)(rect.Y + 0.5f), (int)(rect.Width + 0.5f), (int)(rect.Height + 0.5f));
        }

        public static implicit operator Rectangle(IntRect rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
