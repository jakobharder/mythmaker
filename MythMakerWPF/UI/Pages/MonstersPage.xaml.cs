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
using MythMaker.Myth.Cards;
using MythMaker.Myth.Elements;
using MythMaker.Rendering;

namespace MythMaker.UI.Pages
{
    /// <summary>
    /// Interaction logic for ItemsPage.xaml
    /// </summary>
    public partial class MonstersPage : UserControl
    {
        public MonstersPage()
        {
            InitializeComponent();

            MythDocument.OnActiveChanged += MythDocument_OnActiveChanged;
            AttributeTabs.IsEnabled = false;
        }

        private void MythDocument_OnActiveChanged(object sender, EventArgs e)
        {
            listMonsters.SelectedItem = null;
            listMonsters.Items.Clear();

            var doc = MythDocument.Active;
            if (doc != null)
            {
                btnAddItem.IsEnabled = true;
                listMonsters.IsEnabled = true;

                foreach (var monster in MythDocument.Active.Monsters)
                    listMonsters.Items.Add(monster);
            }
            else
            {
                btnAddItem.IsEnabled = false;
                listMonsters.IsEnabled = false;
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var item = MythDocument.Active.NewMonster();
            listMonsters.Items.Add(item);
            listMonsters.SelectedItem = item;
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var card = (listMonsters.SelectedIndex >= 0) ? listMonsters.SelectedItem as MonsterCard : null;
            AttributeTabs.IsEnabled = card != null;
            if (card != null)
                card.Refresh();
            MinionGeneral.DataContext = card;
            Abilities.DataContext = card;
            cardContainer.Card = card;
            Attacks.DataContext = card;
            Spawns.Visibility = card?.Rank == MonsterRank.Minion ? Visibility.Visible : Visibility.Collapsed;
            MythDocument.Active?.AutoSave();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cardContainer.ShowBack = (AttributeTabs.SelectedIndex >= 2 && AttributeTabs.SelectedIndex <= 3);
        }

        #region Debug Tab
        private bool sampleToggled = false;
        private void BtnToggleSample_Click(object sender, RoutedEventArgs e)
        {
            var card = (listMonsters.SelectedIndex >= 0) ? listMonsters.SelectedItem as MonsterCard : null;
            sampleToggled = !sampleToggled;
            string rank = card.Rank.ToString().ToLower();
            string back = "";
            if (cardContainer.ShowBack)
                back = "-back";
            cardContainer.Sample = sampleToggled ? ImageCache.Get("resources/sample-" + rank + back + ".png") : null;
        }

        private void BtnReRender_Click(object sender, RoutedEventArgs e)
        {
            cardContainer.Card?.Refresh();
        }
        #endregion
    }
}
