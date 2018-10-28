using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Myth.Elements
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class ItemSpecial : INotifyPropertyChanged
    {
        #region DataMember Properties & Serialization
        [DataMember(Name = "Text", IsRequired = true)]
        public string text = "";
        [DataMember(Name = "Fates", EmitDefaultValue = false)]
        private string fatesForSerializing;

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            fatesForSerializing = fates.Length > 0 ? string.Join(", ", fates) : null;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            fatesForSerializing = "";
            text = "";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            fates = fatesForSerializing.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion

        #region DataMember Component Properties
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }

        private string[] fates = new string[] { };
        public string[] Fates
        {
            get { return fates; }
            set
            {
                fates = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fates"));
            }
        }
        #endregion

        public bool HasContent
        {
            get { return (text != null && text != "") || (fates != null && fates.Length > 0); }
        }
    }
}
