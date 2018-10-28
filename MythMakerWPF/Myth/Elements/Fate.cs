using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MythMaker.Myth
{
    class Fate
    {
        public static string[] Fates = new string[] { "Rage", "Guile", "Nature", "Arcane", "Faith", "Darkness" };
        private static Image[] images = null;
        private static Dictionary<string, int> mapping;

        public static Image GetImage(string fate)
        {
            if (images == null)
            {
                images = new Image[Fates.Length];
                mapping = new Dictionary<string, int>();
                for (int i = 0; i < Fates.Length; i++)
                {
                    images[i] = Image.FromFile("resources/fate-" + Fates[i].ToLower() + ".png");
                    mapping[Fates[i].ToLower()] = i;
                }
            }

            return images[mapping[fate.ToLower()]];
        }
    }
}
