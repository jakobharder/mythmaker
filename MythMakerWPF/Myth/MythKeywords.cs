using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker
{
    class MythKeywords
    {
        public static string[] Keywords = new string[]
        {
            "Ammo", "Faith", "Rage", "Shield", "Malice", "Shadows", "Ongoing", "Resurrection", "Focus", "Relic"
        };

        public static string GetRegex()
        {
            return string.Join("|", Keywords);
        }

        public static bool IsKeyword(string text)
        {
            foreach (var keyword in Keywords)
                if (keyword == text)
                    return true;
            return false;
        }
    }
}
