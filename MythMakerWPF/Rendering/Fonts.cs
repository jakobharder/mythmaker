using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace MythMaker.Rendering
{
    class Fonts
    {
        #region font settings
        public class FontSettings
        {
            public string Mason
            {
                get
                {
                    var str = Properties.Settings.Default["FontsMasonSans"] as string;
                    return str == "" ? null : str;
                }
            }
            public string HelCn
            {
                get
                {
                    var str = Properties.Settings.Default["FontsHelveticaNeueCn"] as string;
                    return str == "" ? null : str;
                }
            }
            public string HelMdCn
            {
                get
                {
                    var str = Properties.Settings.Default["FontsHelveticaNeueMdCn"] as string;
                    return str == "" ? null : str;
                }
            }

            public void Update(string mason, string helCn, string helMdCn)
            {
                Properties.Settings.Default["FontsMasonSans"] = mason;
                Properties.Settings.Default["FontsHelveticaNeueCn"] = helCn;
                Properties.Settings.Default["FontsHelveticaNeueMdCn"] = helMdCn;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        private static PrivateFontCollection fontCollection;
        public static FontSettings Settings { get; } = new FontSettings();

        private static System.Drawing.Font[] myth;
        public static System.Drawing.Font[] Myth
        {
            get
            {
                if (myth == null)
                    loadFonts();
                return myth;
            }
        }

        private static System.Drawing.Font[] helCn, helCnO, helMdCnO, helBdCn;

        public static bool Validate()
        {
            return Validate(Settings.Mason, Settings.HelCn, Settings.HelMdCn);
        }

        public static bool Validate(string mason, string helCn, string helMdCn)
        {
            // test mason
            System.Drawing.Font font = new System.Drawing.Font(mason, 10.0f, System.Drawing.FontStyle.Regular);
            if (font.Name != mason)
                return false;

            // test helvetica regular, italic, bold
            font = new System.Drawing.Font(helCn, 10.0f, System.Drawing.FontStyle.Regular);
            if (font.Name != helCn)
                return false;
            font = new System.Drawing.Font(helCn, 10.0f, System.Drawing.FontStyle.Italic);
            if (font.Name != helCn)
                return false;
            font = new System.Drawing.Font(helCn, 10.0f, System.Drawing.FontStyle.Bold);
            if (font.Name != helCn)
                return false;

            // test helvetica medium italic
            font = new System.Drawing.Font(helMdCn, 10.0f, System.Drawing.FontStyle.Italic);
            if (font.Name != helMdCn)
                return false;

            return true;
        }

        public static void Reset()
        {
            myth = null;
        }

        private static void loadFonts()
        {
            fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(System.IO.Path.GetFullPath(@"resources/Myth.ttf"));

            // make sure fontCollection is kept alive, otherwise the fonts will be unloaded
            var mythFamily = fontCollection.Families[0];
            myth = new System.Drawing.Font[] {
                new System.Drawing.Font(mythFamily, 7.6f, System.Drawing.FontStyle.Regular),
                new System.Drawing.Font(mythFamily, 6.0f, System.Drawing.FontStyle.Regular)
            };

            helCn = new System.Drawing.Font[] {
                new System.Drawing.Font(Settings.HelCn, 5.82f, System.Drawing.FontStyle.Regular),
                new System.Drawing.Font(Settings.HelCn, 4.83f, System.Drawing.FontStyle.Regular)
            };
            helCnO = new System.Drawing.Font[] {
                new System.Drawing.Font(Settings.HelCn, 5.82f, System.Drawing.FontStyle.Italic),
                new System.Drawing.Font(Settings.HelCn, 4.83f, System.Drawing.FontStyle.Italic)
            };
            helMdCnO = new System.Drawing.Font[] {
                new System.Drawing.Font(Settings.HelMdCn, 5.82f, System.Drawing.FontStyle.Italic),
                new System.Drawing.Font(Settings.HelMdCn, 4.83f, System.Drawing.FontStyle.Italic)
            };
            helBdCn = new System.Drawing.Font[] {
                new System.Drawing.Font(Settings.HelCn, 5.82f, System.Drawing.FontStyle.Bold),
                new System.Drawing.Font(Settings.HelCn, 4.83f, System.Drawing.FontStyle.Bold)
            };
        }
    }
}
