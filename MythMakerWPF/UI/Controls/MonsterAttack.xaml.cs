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
    public partial class MonsterAttack : UserControl
    {
        private Myth.Elements.MonsterAttack Attack
        {
            get { return DataContext as Myth.Elements.MonsterAttack; }
        }

        public MonsterAttack()
        {
            InitializeComponent();

            foreach (var combo in FateDicePanel.Children)
            {
                if (combo is ComboBox fateCombo)
                {
                    fateCombo.Items.Add("");
                    foreach (var fate in Myth.Fate.Fates)
                        fateCombo.Items.Add(fate);
                    fateCombo.Items.Add("or");
                }
            }
        }

        private bool freezeComboBox = false;
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Attack != null)
            {
                freezeComboBox = true;
                for (int i = 0; i < FateDicePanel.Children.Count; i++)
                {
                    if (FateDicePanel.Children[i] is ComboBox fateCombo)
                    {
                        if (i < Attack.FateRecipeDice.Length)
                            fateCombo.Text = Attack.FateRecipeDice[i];
                        else
                            fateCombo.Text = "";
                    }
                }
                freezeComboBox = false;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Attack != null && !freezeComboBox)
            {
                List<string> fates = new List<string>();
                foreach (var combo in FateDicePanel.Children)
                {
                    if (combo is ComboBox fateCombo && fateCombo.SelectedItem is string fateText && fateText != "")
                        fates.Add(fateText);
                }
                Attack.FateRecipeDice = fates.ToArray();
            }
        }
    }
}
