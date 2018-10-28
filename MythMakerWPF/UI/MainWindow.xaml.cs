using System;
using System.Collections.Generic;
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
using MythMaker.Myth;

namespace MythMaker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string WindowTitle = "MythMaker v0.2";

        public MainWindow()
        {
            InitializeComponent();

            MythDocument.OnActiveChanged += MythDocument_OnActiveChanged;
            MythDocument.Active = null;
        }

        private void MythDocument_OnActiveChanged(object sender, MythDocumentChangedEventArgs e)
        {
            if (e.Old != null)
                e.Old.PropertyChanged -= MythDocument_PropertyChanged;
            if (e.New != null)
                e.New.PropertyChanged += MythDocument_PropertyChanged;

            MythDocument_PropertyChanged(null, null);

            var isEnabled = (MythDocument.Active != null);
            itemsPage.IsEnabled = isEnabled;
            monstersPage.IsEnabled = isEnabled;
            exportPage.IsEnabled = isEnabled;
        }

        private void MythDocument_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var title = WindowTitle;
            if (MythDocument.Active != null)
            {
                title += " - " + MythDocument.Active.Title;
                if (MythDocument.Active.Modified)
                    title += " (unsaved)";
                else
                    title += " (saved)";
            }
            Title = title;
        }

        private void selectFonts_Click(object sender, RoutedEventArgs e)
        {
            var selectFonts = new SelectFonts();
            selectFonts.Owner = Window.GetWindow(this);
            var result = selectFonts.ShowDialog();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MythDocument.Active?.AutoSave();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MythDocument.Active?.AutoSave();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Rendering.Fonts.Validate())
                selectFonts_Click(null, null);
        }
    }
}
