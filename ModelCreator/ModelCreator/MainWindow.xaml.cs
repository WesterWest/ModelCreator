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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Part> parts = new List<Part>();
        private int partIndex = 0;
        public MainWindow()
        {
            parts.Add(new Part());
            InitializeComponent();
            sides_textBox.ContentChanged += UpdateSides;
        }


        
        private void magicButton_Click(object sender, RoutedEventArgs e)
        {


        }

        private void UpdateSides(TextBox txBx)
        {
            int sidesParsed;
            if (int.TryParse(txBx.Text, out sidesParsed) && sidesParsed > 2)
            {
                parts[partIndex].Sides = sidesParsed;
            }
            else
            {
                parts[partIndex].Sides = 3;
                txBx.Text = "3";
            }

            
            for(int i = 0; i < Math.Abs(sidesParsed - corners_stackPanel.Children.Count); i++)
            {
                if (sidesParsed - corners_stackPanel.Children.Count > 0)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    stackPanel.Width = corners_stackPanel.Width;
                    stackPanel.Orientation = Orientation.Horizontal;

                    CleverBox angle_textBox = new CleverBox();
                    CleverBox length_textBox = new CleverBox();
                    CleverBox joints_textBox = new CleverBox();                    

                    angle_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    length_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    joints_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;

                    angle_textBox.TransformToAncestor(stackPanel);// = (corners_stackPanel.Width / 3);
                    length_textBox.TransformToAncestor(stackPanel);//length_textBox.Size = new corners_stackPanel.Width / 3);
                    joints_textBox.TransformToAncestor(stackPanel);//joints_textBox.Width = (corners_stackPanel.Width / 3);


                    stackPanel.Children.Add(angle_textBox);
                    stackPanel.Children.Add(length_textBox);
                    stackPanel.Children.Add(joints_textBox);

                    corners_stackPanel.Children.Add(stackPanel);
                }
                else
                {

                }
            }
        }

        private void sides_textBox_ContentChanged(string obj)
        {

        }
    }
}
