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
using Xceed.Wpf.Toolkit;

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
                UpdatePolygon();
            }
            else
            {
                txBx.Text = "3";
                UpdatePolygon();
            }

            Boolean addMode = sidesParsed - corners_stackPanel.Children.Count > 0;

            int timesRun = Math.Abs(sidesParsed - corners_stackPanel.Children.Count);

            for (int i = 0; i < timesRun; i++)
            {
                if (addMode)
                {
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());

                    DoubleUpDown angle_textBox = new DoubleUpDown();
                    angle_textBox.DefaultValue = 0;
                    DoubleUpDown length_textBox = new DoubleUpDown();
                    length_textBox.DefaultValue = 0;
                    IntegerUpDown joints_textBox = new IntegerUpDown();
                    joints_textBox.DefaultValue = 0;

                    angle_textBox.ValueChanged += (penis, args) => 
                    {
                        UpdatePolygon();
                    };
                    length_textBox.ValueChanged += (kana, args) =>
                    {
                        UpdatePolygon();
                    };
                    joints_textBox.ValueChanged += (john_cena, args) =>
                    {
                        UpdatePolygon();
                    };

                    angle_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    length_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    joints_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;

                    Grid.SetColumn(angle_textBox, 0);
                    Grid.SetColumn(length_textBox, 1);
                    Grid.SetColumn(joints_textBox, 2);

                    angle_textBox.Value = 30;
                    length_textBox.Value = 50;
                    joints_textBox.Value = 0;

                    grid.Children.Add(angle_textBox);
                    grid.Children.Add(length_textBox);
                    grid.Children.Add(joints_textBox);

                    corners_stackPanel.Children.Add(grid);
                }
                else
                {
                    corners_stackPanel.Children.RemoveAt(corners_stackPanel.Children.Count - 1);
                }
            }
        }

        private void UpdatePolygon()
        {
            List<float> angles = new List<float>();
            List<int> joints = new List<int>();
            List<float> lengths = new List<float>();

            foreach (UIElement element in corners_stackPanel.Children)
            {
                Grid grid = (Grid)element;
                DoubleUpDown angle = (DoubleUpDown)grid.Children[0];
                DoubleUpDown length = (DoubleUpDown)grid.Children[1];
                IntegerUpDown joint = (IntegerUpDown)grid.Children[2];
                angles.Add((float)angle.Value);
                joints.Add((int)joint.Value);
                lengths.Add((float)length.Value);
            }

            parts[partIndex].setAnglesJointsLengths(angles.Count, angles.ToArray(), lengths.ToArray(), joints);


            if (corners_stackPanel.Children.Count > 0 && parts[partIndex].Vertices.Count > 2)
            {
                Grid gridLast = (Grid)corners_stackPanel.Children[corners_stackPanel.Children.Count - 1];

                DoubleUpDown angleLast = (DoubleUpDown)gridLast.Children[0];
                DoubleUpDown lengthLast = (DoubleUpDown)gridLast.Children[1];
                IntegerUpDown jointLast = (IntegerUpDown)gridLast.Children[2];

                Vector2 firstVector = parts[partIndex].Vertices[1];
                Vector2 secondVector = parts[partIndex].Vertices.Last();
            }
                        
            canvas.Children.Clear();
            canvas.Children.Add(parts[partIndex].getPolygon(new Vector2((float)canvas.ActualWidth / 2, (float)canvas.ActualHeight / 2)));
            foreach(Vector2 local in parts[partIndex].DrawJoints)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Stroke = Brushes.Red;
                ellipse.StrokeThickness = 2;
                ellipse.Width = 5;
                ellipse.Height = 5;
                ellipse.Fill = Brushes.Red;
                ellipse.Margin = new Thickness(local.X + ((float)canvas.ActualWidth / 2), local.Y + ((float)canvas.ActualHeight / 2), 0, 0);
                canvas.Children.Add(ellipse);
            }
            
        } 
    }
}
