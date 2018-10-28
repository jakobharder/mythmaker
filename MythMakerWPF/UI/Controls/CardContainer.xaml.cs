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
using MythMaker.Myth.Cards;

namespace MythMaker.UI
{
    /// <summary>
    /// Interaction logic for CardContainer.xaml
    /// </summary>
    public partial class CardContainer : UserControl
    {
        public int MaxCardWidth { get; set; }

        private Card card;
        public Card Card
        {
            get { return card; }
            set
            {
                if (card != null)
                {
                    card.PropertyChanged -= Card_PropertyChanged;
                }
                card = value;
                if (card != null)
                {
                    card.PropertyChanged += Card_PropertyChanged;
                }
                Card_PropertyChanged(null, null);
            }
        }

        private ImageSource sample;
        public System.Drawing.Image Sample
        {
            get
            {
                return null;
            }
            set
            {
                sample = value != null ? ImageConverter.GetBitmapSource(new System.Drawing.Bitmap(value)) : null;
                Card_PropertyChanged(null, null);
            }
        }

        private bool showBack;
        public bool ShowBack
        {
            get { return showBack; }
            set
            {
                if (value != showBack)
                {
                    showBack = value;
                    Card_PropertyChanged(null, null);
                }
            }
        }

        private void Card_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (showBack)
                cardImage.Source = sample ?? card?.BackImage;
            else
                cardImage.Source = sample ?? card?.FullImage;
        }

        public CardContainer()
        {
            InitializeComponent();
        }
    }
}
