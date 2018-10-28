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
using MythMaker.Myth;
using MythMaker.Myth.Cards;
using MythMaker.Rendering;

namespace MythMaker.UI.Pages
{
    /// <summary>
    /// Interaction logic for ItemsPage.xaml
    /// </summary>
    public partial class ItemsPage : UserControl
    {
        public ItemsPage()
        {
            InitializeComponent();

            MythDocument.OnActiveChanged += MythDocument_OnActiveChanged;
            AttributeTabs.IsEnabled = false;
        }

        private void MythDocument_OnActiveChanged(object sender, EventArgs e)
        {
            listItems.SelectedItem = null;
            listItems.Items.Clear();

            var doc = MythDocument.Active;
            if (doc != null)
            {
                btnAddItem.IsEnabled = true;
                listItems.IsEnabled = true;

                foreach (var item in MythDocument.Active.Items)
                    listItems.Items.Add(item);
            }
            else
            {
                btnAddItem.IsEnabled = false;
                listItems.IsEnabled = false;
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var item = MythDocument.Active.NewItem();
            listItems.Items.Add(item);
            listItems.SelectedItem = item;
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var card = (listItems.SelectedIndex >= 0) ? listItems.SelectedItem as ItemCard : null;
            AttributeTabs.IsEnabled = card != null;
            if (card != null)
                card.Refresh();
            cardContainer.Card = card;
            GeneralAttributes.DataContext = card;
            ItemSpecialsTab.DataContext = card;
            MythDocument.Active?.AutoSave();
        }

        #region Debug Tab
        private bool sampleToggled = false;

        private void BtnToggleSample_Click(object sender, RoutedEventArgs e)
        {
            var card = (listItems.SelectedIndex >= 0) ? listItems.SelectedItem as ItemCard : null;
            sampleToggled = !sampleToggled;
            cardContainer.Sample = sampleToggled ? ImageCache.Get("resources/sample-" + card.ItemColor.ToString().ToLower() + ".png") : null;
        }

        private void BtnReRender_Click(object sender, RoutedEventArgs e)
        {
            cardContainer.Card?.Refresh();
        }
        #endregion
    }
}
