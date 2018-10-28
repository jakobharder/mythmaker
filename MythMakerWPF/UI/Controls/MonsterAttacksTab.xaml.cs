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
    public partial class MonsterAttacksTab : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Myth.Elements.MonsterAttack newAttack = null;

        public int NumberOfAttacks { get { return 2; } }

        private MonsterCard Card { get { return DataContext as MonsterCard; } }

        private void Notify(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        public MonsterAttacksTab()
        {
            InitializeComponent();
            createNewAttack();
            updateAttacks();
        }

        private void createNewAttack()
        {
            if (newAttack != null)
                throw new Exception();

            newAttack = new Myth.Elements.MonsterAttack();
            newAttack.PropertyChanged += Attack_PropertyChanged;
        }

        private void updateAttacks()
        {
            int number = (Card?.Attacks.Length).GetValueOrDefault();

            // always one more than data, for auto adding
            number++;

            int diff = AttackStack.Children.Count - number;
            if (diff < 0)
            {
                for (int i = 0; i < -diff; i++)
                {
                    var attack = new MonsterAttack();
                    attack.Title.Content = "Attack " + (AttackStack.Children.Count + 1);
                    AttackStack.Children.Add(attack);
                }
            }
            else if (diff > 0)
            {
                AttackStack.Children.RemoveRange(number, diff);
            }

            for (int i = 0; i < number - 1; i++)
            {
                // AttackStack.Children[i]
                // set data context and make sure they are property linked
                var attack = AttackStack.Children[i] as MonsterAttack;
                if (attack.DataContext != Card.Attacks[i])
                    attack.DataContext = Card.Attacks[i];
            }

            // the last is always for new attacks
            var lastAttack = AttackStack.Children[number - 1] as MonsterAttack;
            if (lastAttack.DataContext != newAttack)
                lastAttack.DataContext = newAttack;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var card = e.OldValue as MonsterCard;
            if (card != null)
            {
                card.PropertyChanged -= Card_PropertyChanged;
                foreach (var attack in card.Attacks)
                    attack.PropertyChanged -= Attack_PropertyChanged;
            }

            card = e.NewValue as MonsterCard;
            if (card != null)
            {
                card.PropertyChanged += Card_PropertyChanged;
                foreach (var attack in card.Attacks)
                    attack.PropertyChanged += Attack_PropertyChanged;
            }

            updateAttacks();
        }

        private void Card_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Attack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Card == null)
                return;

            if (newAttack.HasAttacks || newAttack.HasFateRecipe)
            {
                // add to card
                Card.Attacks = Card.Attacks.Concat(new Myth.Elements.MonsterAttack[] { newAttack }).ToArray();
                newAttack = null;
                createNewAttack();
                updateAttacks();
            }
            else if (false)
            {
                // delete card
            }
        }
    }
}
