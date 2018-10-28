using System.Drawing;
using System.Runtime.Serialization;

namespace MythMaker.Math
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public struct Vector
    {
        [DataMember]
        public float X { get; set; }
        [DataMember]
        public float Y { get; set; }

        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator /(Vector a, float b)
        {
            return new Vector(a.X / b, a.Y / b);
        }

        public static implicit operator PointF(Vector a)
        {
            return new PointF(a.X, a.Y);
        }

        public static explicit operator Point(Vector a)
        {
            return new Point((int)(a.X + 0.5f), (int)(a.Y + 0.5f));
        }

        public static explicit operator IntVector(Vector a)
        {
            return new IntVector((int)(a.X + 0.5f), (int)(a.Y + 0.5f));
        }
    }
}
