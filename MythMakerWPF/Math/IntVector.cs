using System.Drawing;
using System.Runtime.Serialization;

namespace MythMaker.Math
{
    [DataContract]
    public struct IntVector
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntVector operator +(IntVector a, IntVector b)
        {
            return new IntVector(a.X + b.X, a.Y + b.Y);
        }

        public static IntVector operator -(IntVector a, IntVector b)
        {
            return new IntVector(a.X - b.X, a.Y - b.Y);
        }

        public static implicit operator Point(IntVector a)
        {
            return new Point(a.X, a.Y);
        }

        public static implicit operator Vector(IntVector a)
        {
            return new Vector(a.X, a.Y);
        }
    }
}
