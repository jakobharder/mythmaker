using MythMaker.Myth.Cards;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MythMaker.UI.Controls
{
    /// <summary>
    /// Interaction logic for MinionGeneral.xaml
    /// </summary>
    public partial class ItemSpecialsTab : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Myth.Elements.ItemSpecial newSpecial = null;

        private ItemCard Card { get { return DataContext as ItemCard; } }

        private void Notify(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        public ItemSpecialsTab()
        {
            InitializeComponent();
            createNewSpecial();
            updateSpecials();
        }

        private void createNewSpecial()
        {
            if (newSpecial != null)
                throw new Exception();

            newSpecial = new Myth.Elements.ItemSpecial();
            newSpecial.PropertyChanged += Special_PropertyChanged;
        }

        private void updateSpecials()
        {
            int number = (Card?.Special.Length).GetValueOrDefault();

            // always one more than data, for auto adding
            number++;

            int diff = SpecialStack.Children.Count - number;
            if (diff < 0)
            {
                for (int i = 0; i < -diff; i++)
                {
                    var special = new ItemSpecial();
                    special.Title.Content = "Special " + (SpecialStack.Children.Count + 1);
                    SpecialStack.Children.Add(special);
                }
            }
            else if (diff > 0)
            {
                SpecialStack.Children.RemoveRange(number, diff);
            }

            for (int i = 0; i < number - 1; i++)
            {
                // AttackStack.Children[i]
                // set data context and make sure they are property linked
                var attack = SpecialStack.Children[i] as ItemSpecial;
                if (attack.DataContext != Card.Special[i])
                    attack.DataContext = Card.Special[i];
            }

            // the last is always for new attacks
            var lastAttack = SpecialStack.Children[number - 1] as ItemSpecial;
            if (lastAttack.DataContext != newSpecial)
                lastAttack.DataContext = newSpecial;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var card = e.OldValue as ItemCard;
            if (card != null)
            {
                card.PropertyChanged -= Card_PropertyChanged;
                foreach (var attack in card.Special)
                    attack.PropertyChanged -= Special_PropertyChanged;
            }

            card = e.NewValue as ItemCard;
            if (card != null)
            {
                card.PropertyChanged += Card_PropertyChanged;
                foreach (var attack in card.Special)
                    attack.PropertyChanged += Special_PropertyChanged;
            }

            updateSpecials();
        }

        private void Card_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Special_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Card == null)
                return;

            if (newSpecial.HasContent)
            {
                // add to card
                Card.Special = Card.Special.Concat(new Myth.Elements.ItemSpecial[] { newSpecial }).ToArray();
                newSpecial = null;
                createNewSpecial();
                updateSpecials();
            }
            else if (false)
            {
                // delete card
            }
        }
    }
}
