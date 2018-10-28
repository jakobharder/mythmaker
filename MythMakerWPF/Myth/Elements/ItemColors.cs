using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Drawing;

namespace MythMaker
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public struct ItemColor
    {
        public static string[] DisplayNames = new string[] { "Brown", "White", "Green", "Blue", "Gold", "Orange", "Purple", "Red" };
        private static Color[] colors = new Color[]
        {
            Color.FromArgb(255, 129, 72, 22),
            Color.FromArgb(255, 88, 89, 91),
            Color.FromArgb(255, 65, 173, 73),
            Color.FromArgb(255, 2, 78, 162),
            Color.FromArgb(255, 188, 144, 9),
            Color.FromArgb(255, 243, 111, 41),
            Color.FromArgb(255, 123, 39, 122),
            Color.FromArgb(255, 129, 72, 22)
        };

        public static ItemColor Brown = new ItemColor(0);
        public static ItemColor White = new ItemColor(1);
        public static ItemColor Green = new ItemColor(2);
        public static ItemColor Blue = new ItemColor(3);
        public static ItemColor Gold = new ItemColor(4);
        public static ItemColor Orange = new ItemColor(5);
        public static ItemColor Purple = new ItemColor(6);
        public static ItemColor Red = new ItemColor(7);

        public static ItemColor[] Colors = { Brown, White, Green, Blue, Gold, Orange, Purple, Red };
        public static ItemColor Default { get { return Colors[0]; } }

        [DataMember]
        private int color;

        public Color Color { get { return colors[color]; } }

        public static implicit operator ItemColor(int color)
        {
            if (color < 0 || color > Colors.Length)
                color = 0;
            return ItemColor.Colors[color];
        }

        public static implicit operator int(ItemColor color)
        {
            return color.color;
        }

        private ItemColor(int color)
        {
            this.color = color;
        }

        public override string ToString()
        {
            return DisplayNames[color];
        }

        public static implicit operator ItemColor(string text)
        {
            for (int i = 0; i < DisplayNames.Length; i++)
                if (DisplayNames[i] == text)
                    return Colors[i];
            return Default;
        }
    }
}
