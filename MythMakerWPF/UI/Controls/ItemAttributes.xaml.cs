using MythMaker.Myth;
using MythMaker.Myth.Cards;
using MythMaker.Myth.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MythMaker.UI.Controls
{
    /// <summary>
    /// Interaction logic for MinionGeneral.xaml
    /// </summary>
    public partial class ItemAttributes : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ItemCard Card { get { return DataContext as ItemCard; } }

        private void Notify(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        public ItemAttributes()
        {
            InitializeComponent();
        }

        private void OpenPicture_Click(object sender, RoutedEventArgs e)
        {
            string path = Properties.Settings.Default["LastPicturePath"] as string;

            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (System.IO.Directory.Exists(path))
                openFileDialog.InitialDirectory = path;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["LastPicturePath"] = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.Save();

                var img = System.Drawing.Image.FromFile(openFileDialog.FileName);

                Uri fullPath = new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute);
                Uri relRoot = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/", UriKind.Absolute);

                string relPath = relRoot.MakeRelativeUri(fullPath).ToString();

                Card.SetImage(MythBitmap.FromBitmap(new System.Drawing.Bitmap(img), "items/" + Card.FileTitle + ".png", Card.Document));
            }
        }
    }
}
