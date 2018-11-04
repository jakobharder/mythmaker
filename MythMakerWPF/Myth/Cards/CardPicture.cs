using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Myth.Cards
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class CardPicture : INotifyPropertyChanged
    {
        #region DataMember & Serialization
        [DataMember(Name = "Image", IsRequired = true)]
        private MythBitmap image;
        [DataMember]
        public Math.Vector ImageOffset { get; private set; }
        [DataMember]
        private Math.Vector ImageScaling { get; set; }
        [DataMember]
        public Math.Vector ImagePreScaling { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ImagePreScaling == null || ImagePreScaling.X == 0 || ImagePreScaling.Y == 0)
                ImagePreScaling = new Math.Vector(1, 1);
            if (ImageScaling == null || ImageScaling.X == 0 || ImageScaling.Y == 0)
                ImageScaling = new Math.Vector(1, 1);
            if (ImageOffset == null)
                ImageOffset = new Math.Vector(0, 0);

            ImageOffset.PropertyChanged += ImageOffset_PropertyChanged;
        }

        private void ImageOffset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
        }
        #endregion

        #region DataMember Component Properties
        public MythBitmap Image
        {
            get { return image; }
            set
            {
                image = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }
        #endregion

        public Math.Vector Scaling
        {
            get { return ImagePreScaling * ImageScaling.X; }
        }

        public int UserScalingPercentage
        {
            get { return (int)(ImageScaling.X * 100 + 0.5f); }
            set
            {
                ImageScaling.X = value / 100.0f;
                ImageScaling.Y = value / 100.0f;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Image"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CardPicture()
        {
            ImagePreScaling = new Math.Vector(1, 1);
            ImageScaling = new Math.Vector(1, 1);
            ImageOffset = new Math.Vector(0, 0);
            ImageOffset.PropertyChanged += ImageOffset_PropertyChanged;
        }
    }
}
