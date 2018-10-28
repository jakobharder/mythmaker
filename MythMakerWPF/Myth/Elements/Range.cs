using System.Runtime.Serialization;

namespace MythMaker.Myth.Elements
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public struct Range
    {
        [DataMember]
        private int value;

        public static readonly Range Default = 1;

        public override string ToString()
        {
            if (value == 0)
                return "-";
            if (value < 0)
                return (-value).ToString() + "+";
            if (value == 99)
                return "Tile";
            return value.ToString();
        }

        public Range(int range)
        {
            value = range;
        }

        public static implicit operator Range(string text)
        {
            if (text == "Tile")
                return 99;
            if (text == "-")
                return 0;
            return int.Parse(text);
        }

        public static implicit operator Range(int range)
        {
            return new Range(range);
        }

        public static implicit operator int(Range range)
        {
            return range.value;
        }
    }
}
