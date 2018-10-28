using MythMaker.Myth;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;

namespace MythMaker.UI.Pages
{
    /// <summary>
    /// Interaction logic for ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : UserControl, INotifyPropertyChanged
    {
        #region Component Properties
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        public bool HasOpenDocument
        {
            get { return MythDocument.Active != null; }
        }

        private StringCollection recentFiles;
        public StringCollection RecentFiles
        {
            get { return recentFiles; }
            set
            {
                recentFiles = value;
                foreach (var file in RecentFilesPanel.Children)
                    (file as Button).Click -= RecentFile_Click;
                RecentFilesPanel.Children.Clear();

                if (recentFiles != null)
                {
                    foreach (var file in recentFiles)
                    {
                        var label = new Button()
                        {
                            Content = System.IO.Path.GetFileName(file),
                            Style = Resources["UrlButton"] as System.Windows.Style,
                            DataContext = file
                        };
                        label.Click += RecentFile_Click;
                        RecentFilesPanel.Children.Add(label);
                    }
                }
                Properties.Settings.Default["RecentFiles"] = recentFiles;
                Properties.Settings.Default.Save();
            }
        }

        private void RecentFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenDocument((sender as Button).DataContext as string);
        }
        #endregion

        public ProjectPage()
        {
            InitializeComponent();
            DataContext = this;

            RecentFiles = Properties.Settings.Default["RecentFiles"] as StringCollection;
            if (RecentFiles == null)
                RecentFiles = new StringCollection();
            else
                UpdateRecent();
        }

        private void btnNewProject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string path = Properties.Settings.Default["LastProjectPath"] as string;

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.DefaultExt = "myth";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "MythMake file|*.myth";
            if (System.IO.Directory.Exists(path))
                saveFileDialog.InitialDirectory = path;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["LastProjectPath"] = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                Properties.Settings.Default.Save();
                var document = new MythDocument();
                MythDocument.Active = document;
                NotifyPropertyChanged("HasOpenDocument");
                MythDocument.Active.Save(saveFileDialog.FileName);
                AddToRecent(saveFileDialog.FileName);
            }
        }

        private void btnOpenProject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string path = Properties.Settings.Default["LastProjectPath"] as string;

            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.DefaultExt = "myth";
            openFileDialog.Filter = "MythMaker file|*.myth";
            openFileDialog.RestoreDirectory = true;
            if (System.IO.Directory.Exists(path))
                openFileDialog.InitialDirectory = path;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["LastProjectPath"] = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                Properties.Settings.Default.Save();
                OpenDocument(openFileDialog.FileName);
            }
        }

        private void Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MythDocument.Active.Save(null);
        }

        private void SaveAs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string path = Properties.Settings.Default["LastProjectPath"] as string;

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.DefaultExt = "myth";
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "MythMake file|*.myth";
            if (System.IO.Directory.Exists(path))
                saveFileDialog.InitialDirectory = path;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["LastProjectPath"] = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                Properties.Settings.Default.Save();
                MythDocument.Active.Save(saveFileDialog.FileName);
                AddToRecent(saveFileDialog.FileName);
            }
        }

        private void OpenDocument(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                System.Windows.MessageBox.Show(filePath + " does not exist anymore.");
                UpdateRecent();
                return;
            }

            MythDocument.Active = MythDocument.Open(filePath);
            NotifyPropertyChanged("HasOpenDocument");
            AddToRecent(filePath);
        }

        private void AddToRecent(string filePath)
        {
            var list = RecentFiles;
            if (list.Contains(filePath))
                list.Remove(filePath);
            if (list.Count > 9)
                list.RemoveAt(9);
            list.Insert(0, filePath);
            RecentFiles = list;
        }

        private void UpdateRecent()
        {
            var list = RecentFiles;
            var remove = new List<string>();
            foreach (var file in list)
                if (!System.IO.File.Exists(file))
                    remove.Add(file);

            foreach (var file in remove)
                list.Remove(file);
            RecentFiles = list;
        }
    }
}
