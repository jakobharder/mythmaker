using System.Runtime.Serialization;

namespace MythMaker
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public struct ItemType
    {
        public static ItemType[] Types = new ItemType[]
        {
            new ItemType() { type = 0, DisplayName = "None" },
            new ItemType() { type = 1, DisplayName = "Primary" },
            new ItemType() { type = 2, DisplayName = "Secondary" },
            new ItemType() { type = 3, DisplayName = "Two-Handed" },
            new ItemType() { type = 4, DisplayName = "Armor" },
            new ItemType() { type = 5, DisplayName = "Helm" },
            new ItemType() { type = 6, DisplayName = "Accessory" }
        };
        public static ItemType Default { get { return Types[0]; } }

        public string DisplayName;

        [DataMember]
        private int type;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            DisplayName = Types[type].DisplayName;
        }

        public static implicit operator ItemType(int type)
        {
            if (type < 0 || type > Types.Length)
                type = 0;
            return ItemType.Types[type];
        }

        public static implicit operator int(ItemType type)
        {
            return type.type;
        }

        public override string ToString()
        {
            return this.DisplayName;
        }

        public static implicit operator ItemType(string text)
        {
            for (int i = 0; i < Types.Length; i++)
                if (Types[i].DisplayName == text)
                    return Types[i];
            return Default;
        }
    }
}
