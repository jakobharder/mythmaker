using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MythMaker.Rendering
{
    public class ImageCache
    {
        private static Dictionary<string, Image> cache = new Dictionary<string, Image>();

        public static Image Get(string file)
        {
            if (!cache.ContainsKey(file))
            {
                if (System.IO.File.Exists(file))
                {
                    using (var image = Image.FromFile(file))
                        cache[file] = new Bitmap(image);
                }
                else
                {
                    cache[file] = Image.FromFile("resources/notfound.png");
                }
            }

            return cache[file];
        }
    }
}
