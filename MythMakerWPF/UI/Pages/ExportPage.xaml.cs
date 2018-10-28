using MythMaker.Myth;
using System.Windows.Controls;

namespace MythMaker.UI.Pages
{
    /// <summary>
    /// Interaction logic for ExportPage.xaml
    /// </summary>
    public partial class ExportPage : UserControl
    {
        public ExportPage()
        {
            InitializeComponent();
        }

        private void ExportWithoutBleeding_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Export(false);
        }

        private void ExportWithBleeding_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Export(true);
        }

        private void Export(bool bleeding)
        {
            string path = Properties.Settings.Default["LastExportPath"] as string;

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.IO.Directory.Exists(path))
                dialog.SelectedPath = path;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["LastExportPath"] = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                MythDocument.Active.ExportPrinterStudio(dialog.SelectedPath, bleeding);
            }
        }
    }
}
