using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Utils
{
    class TextUtils
    {
        public static string[] SplitInHalf(string text)
        {
            var comma = text.IndexOfAny(new char[] { ',', '&' });

            string[] lines;

            if (comma > 0)
            {
                lines = new string[] { text.Substring(0, comma + 1), text.Substring(comma + 1).TrimStart() };
            }
            else
            {
                var split = text.Split(' ');
                var first = split.Length / 2;
                if (first == 0)
                    lines = new string[] { text };
                else
                    lines = new string[] { string.Join(" ", split.Take(first)), string.Join(" ", split.Skip(first)) };
            }

            return lines;
        }
    }
}
