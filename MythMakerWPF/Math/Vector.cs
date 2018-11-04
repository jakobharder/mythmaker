using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace MythMaker.Math
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class Vector : INotifyPropertyChanged
    {
        [DataMember(Name = "X", IsRequired = true)]
        private float x;
        [DataMember(Name = "Y", IsRequired = true)]
        public float y;

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        #region DataMember Component Properties
        public float X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region operators
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
        #endregion
    }
}
