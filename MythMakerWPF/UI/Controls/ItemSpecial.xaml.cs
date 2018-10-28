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

namespace MythMaker.UI.Controls
{
    /// <summary>
    /// Interaction logic for MonsterAttack.xaml
    /// </summary>
    public partial class ItemSpecial : UserControl
    {
        private Myth.Elements.ItemSpecial Special
        {
            get { return DataContext as Myth.Elements.ItemSpecial; }
        }

        public ItemSpecial()
        {
            InitializeComponent();

            foreach (var combo in FateDicePanel.Children)
            {
                if (combo is ComboBox fateCombo)
                {
                    fateCombo.Items.Add("");
                    foreach (var fate in Myth.Fate.Fates)
                        fateCombo.Items.Add(fate);
                }
            }
        }

        private bool freezeComboBox = false;
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Special != null)
            {
                freezeComboBox = true;
                for (int i = 0; i < FateDicePanel.Children.Count; i++)
                {
                    if (FateDicePanel.Children[i] is ComboBox fateCombo)
                    {
                        if (i < Special.Fates.Length)
                            fateCombo.Text = Special.Fates[i];
                        else
                            fateCombo.Text = "";
                    }
                }
                freezeComboBox = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Special != null && !freezeComboBox)
            {
                List<string> fates = new List<string>();
                foreach (var combo in FateDicePanel.Children)
                {
                    if (combo is ComboBox fateCombo && fateCombo.SelectedItem is string fateText && fateText != "")
                        fates.Add(fateText);
                }
                Special.Fates = fates.ToArray();
            }
        }
    }
}
