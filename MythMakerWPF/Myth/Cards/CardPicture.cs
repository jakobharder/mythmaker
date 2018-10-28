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
        public Math.Vector ImageOffset { get; set; }
        [DataMember]
        public Math.Vector ImageScaling { get; set; }
        [DataMember]
        public Math.Vector ImagePreScaling { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (ImagePreScaling.X == 0)
                ImagePreScaling = new Math.Vector(1, 1);
            if (ImageScaling.X == 0)
                ImageScaling = new Math.Vector(1, 1);
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

        public event PropertyChangedEventHandler PropertyChanged;

        public CardPicture()
        {
            ImagePreScaling = new Math.Vector(1, 1);
            ImageScaling = new Math.Vector(1, 1);
        }
    }
}
