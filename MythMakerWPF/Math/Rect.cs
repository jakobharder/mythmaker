namespace MythMaker.Math
{
    public class Rect
    {
        public float X { get; set; }

        public float Y { get; set; }

        public Vector Location
        {
            get { return new Vector(X, Y); }
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public Vector Size
        {
            get { return new Vector(Width, Height); }
        }

        public float Right
        {
            get { return X + Width; }
            set
            {
                Width = value - X;
            }
        }
        public float Bottom
        {
            get { return Y + Height; }
            set
            {
                Height = value - Y;
            }
        }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


        public Rect(Vector position, Vector size)
        {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public void AddPoint(float x, float y)
        {
            if (x > this.X)
                Width = System.Math.Max(x - this.X, Width);
            else if (x < this.X)
            {
                Width += this.X - x;
                this.X = x;
            }
            if (y > this.Y)
                Height = System.Math.Max(y - this.Y, Height);
            else if (y < this.Y)
            {
                Height += this.Y - y;
                this.Y = y;
            }
        }

        public void AddRect(Rect other)
        {
            if (other.X < X)
            {
                Width += X - other.X;
                X = other.X;
            }
            if (other.Y < Y)
            {
                Height += Y - other.Y;
                Y = other.Y;
            }
            if (other.Right > Right)
                Right = other.Right;
            if (other.Bottom > Bottom)
                Bottom = other.Bottom;
        }

        public void Inflate(float size)
        {
            X -= size;
            Y -= size;
            Width += size * 2;
            Height += size * 2;
        }

        public void Move(Vector move)
        {
            X += move.X;
            Y += move.Y;
        }

        public Vector Center
        {
            get
            {
                return new Vector(X + Width * 0.5f, Y + Height * 0.5f);
            }
        }

        public static Rect operator *(Rect rect, Vector factor)
        {
            return new Rect(rect.X * factor.X, rect.Y * factor.Y, rect.Width * factor.X, rect.Height * factor.Y);
        }

        public static Rect operator *(Rect rect, float factor)
        {
            return new Rect(rect.X * factor, rect.Y * factor, rect.Width * factor, rect.Height * factor);
        }

        public static implicit operator System.Drawing.RectangleF(Rect rect)
        {
            return new System.Drawing.RectangleF(rect.X, rect.Y , rect.Width, rect.Height);
        }

        public static explicit operator System.Drawing.Rectangle(Rect rect)
        {
            return new System.Drawing.Rectangle((int)(rect.X + 0.5f), (int)(rect.Y + 0.5f), (int)(rect.Width + 0.5f), (int)(rect.Height + 0.5f));
        }
    }
}
