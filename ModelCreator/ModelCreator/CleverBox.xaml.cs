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

namespace ModelCreator
{
    /// <summary>
    /// Interaction logic for CleverBox.xaml
    /// </summary>
    public partial class CleverBox : UserControl
    {
        public event Action<TextBox> ContentChanged;

        public CleverBox()
        {
            InitializeComponent();

        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ContentChanged?.Invoke(textBox);
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ContentChanged?.Invoke(textBox);
        }

        public String Text
        {
            get {return textBox.Text;}
            set
            {
                textBox.Text = value;
                ContentChanged?.Invoke(textBox);
            }
        }
    }
}
