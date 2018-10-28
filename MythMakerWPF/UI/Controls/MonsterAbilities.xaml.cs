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
    public partial class MonsterAbilities : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        public MonsterAbilities()
        {
            InitializeComponent();
        }
    }
}
