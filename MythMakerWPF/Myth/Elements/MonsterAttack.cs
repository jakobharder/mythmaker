using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MythMaker.Myth.Elements
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class MonsterAttack : INotifyPropertyChanged
    {

        public enum AttackType
        {
            Melee = 0,
            Ranged
        }

        #region DataMember Properties
        [DataMember(Name = "Text", IsRequired = true)]
        public string Text = "";
        [DataMember(Name = "Abilities", EmitDefaultValue = false)]
        string abilitiesForSerialization;
        [DataMember(Name = "D10", IsRequired = true)]
        protected int d10 = 0;
        [DataMember(Name = "FD", IsRequired = true)]
        protected int fd = 0;
        [DataMember(Name = "FateRecipe", EmitDefaultValue = false)]
        string fateRecipeForSerialization;
        [DataMember(Name = "FateRecipeDice", EmitDefaultValue = false)]
        string fateRecipeDiceForSerialization;
        [DataMember(Name = "Number", IsRequired = true)]
        protected int hits = 0;
        [DataMember(Name = "TN", IsRequired = true)]
        protected int tn = 0;
        [DataMember(Name = "HitFactor", IsRequired = true)]
        protected int hitFactor = 1;
        [DataMember(Name = "Damage", IsRequired = true)]
        protected int damage = 1;
        [DataMember(Name = "Range", IsRequired = true)]
        protected int range = Range.Default;
        [DataMember(Name = "Type", IsRequired = true)]
        protected AttackType type = AttackType.Melee;
        [DataMember(Name = "RangeIsMinimum", EmitDefaultValue = false)]
        protected bool rangeIsMinimum = false;
        [DataMember(Name = "PerSuccess", EmitDefaultValue = false)]
        protected bool perSuccess = false;

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            abilitiesForSerialization = abilities.Length > 0 ? string.Join(", ", abilities) : null;
            fateRecipeForSerialization = fateRecipe != "" ? fateRecipe : null;
            fateRecipeDiceForSerialization = fateRecipeDice.Length > 0 ? string.Join(", ", fateRecipeDice) : null;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            abilitiesForSerialization = "";
            fateRecipeForSerialization = "";
            fateRecipeDiceForSerialization = "";
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            abilities = abilitiesForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            fateRecipe = fateRecipeForSerialization;
            fateRecipeDice = fateRecipeDiceForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion

        #region DataMember Component Properties
        public int D10
        {
            get { return d10; }
            set
            {
                d10 = value;
                OnPropertyChanged("D10");
                OnPropertyChanged("HasFateRecipe");
            }
        }

        public int FD
        {
            get { return fd; }
            set
            {
                fd = value;
                OnPropertyChanged("FD");
                OnPropertyChanged("HasFateRecipe");
            }
        }

        string fateRecipe = "";
        public string FateRecipe
        {
            get { return fateRecipe; }
            set
            {
                fateRecipe = value;
                OnPropertyChanged("FateRecipe");
            }
        }

        string[] fateRecipeDice = new string[] { };
        public string[] FateRecipeDice
        {
            get { return fateRecipeDice; }
            set
            {
                fateRecipeDice = value;
                OnPropertyChanged("FateRecipeDice");
            }
        }

        public int NumberOfAttacks
        {
            get { return hits; }
            set
            {
                hits = value;
                OnPropertyChanged("NumberOfAttacks");
                OnPropertyChanged("HasAttacks");
            }
        }

        public int TN
        {
            get { return tn; }
            set
            {
                tn = value;
                OnPropertyChanged("TN");
            }
        }

        public int HitFactor
        {
            get { return hitFactor; }
            set
            {
                hitFactor = value;
                OnPropertyChanged("HitFactor");
            }
        }

        public int Damage
        {
            get { return damage; }
            set
            {
                damage = value;
                OnPropertyChanged("Damage");
            }
        }

        public Range Range
        {
            get { return range; }
            set
            {
                range = value;
                OnPropertyChanged("Range");
            }
        }

        string[] abilities = new string[0];
        public string[] Abilities
        {
            get { return abilities; }
            set
            {
                abilities = value;
                OnPropertyChanged("Abilities");
            }
        }

        public AttackType Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public bool RangeIsMinimum
        {
            get { return rangeIsMinimum; }
            set
            {
                rangeIsMinimum = value;
                OnPropertyChanged("RangeIsMinimum");
            }
        }

        public bool PerSuccess
        {
            get { return perSuccess; }
            set
            {
                perSuccess = value;
                OnPropertyChanged("PerSuccess");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public bool HasAttacks
        {
            get { return hits > 0; }
        }

        public bool HasFateRecipe
        {
            get { return fd > 0; }
        }
    }
}
