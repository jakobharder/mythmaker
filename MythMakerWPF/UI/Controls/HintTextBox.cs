/*
 * based on the work by Miňo
 * https://code.msdn.microsoft.com/TextBox-with-null-text-hint-0b384543
 * Apache License Version 2.0
 */

using System.Windows;
using System.Windows.Controls;

namespace MythMaker.UI.Controls
{
    public class HintTextBox : TextBox
    {
        static HintTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HintTextBox),
                new FrameworkPropertyMetadata(typeof(HintTextBox)));
        }

        public static readonly DependencyProperty NullTextProperty =
             DependencyProperty.Register("HintText", typeof(string),
                typeof(HintTextBox), new FrameworkPropertyMetadata("Zadať"));

        public string HintText
        {
            get { return (string)GetValue(NullTextProperty); }
            set { SetValue(NullTextProperty, value); }
        }
    }
}
