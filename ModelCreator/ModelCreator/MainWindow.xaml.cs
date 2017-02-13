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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ModelCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Part> parts = new ObservableCollection<Part>();
        private int partIndex = 0;
        public MainWindow()
        {
            InitializeComponent();
            partList_listView.ItemsSource = parts;
            partList_listView.SelectionChanged += (temp, temp2) =>
            {
                if (partList_listView.SelectedItem != null) changeSelected((Part)partList_listView.SelectedItem);
            };

            addPart(new Part("Part 0"));
            sides_textBox.ContentChanged += (textBox) =>
            {
                int i;
                if (Int32.TryParse(textBox.Text, out i))
                {
                    rerenderTextBoxesOfPartSides(i);
                    loadDataFromGUItoSelectedPart();
                    rerenderParts();
                }
            };

            rellativeX_DoubleUpDown.DefaultValue = 0;
            rellativeY_DoubleUpDown.DefaultValue = 0;

            rellativeX_DoubleUpDown.ValueChanged += (temp, temp2) => { loadDataFromGUItoSelectedPart(); };
            rellativeX_DoubleUpDown.ValueChanged += (temp, temp2) => { loadDataFromGUItoSelectedPart(); };
            collide_checkBox.Click += (temp, temp2) => { loadDataFromGUItoSelectedPart(); };
        }


        private void loadDataFromGUItoSelectedPart()
        {
            List<float> angles = new List<float>();
            List<int> joints = new List<int>();
            List<float> lengths = new List<float>();

            Debug.WriteLine(corners_stackPanel.Children.Count);

            foreach (UIElement element in corners_stackPanel.Children)
            {
                Grid grid = (Grid)element;
                DoubleUpDown angle = (DoubleUpDown)grid.Children[0];
                DoubleUpDown length = (DoubleUpDown)grid.Children[1];
                IntegerUpDown joint = (IntegerUpDown)grid.Children[2];
                angles.Add((float)angle.Value);
                joints.Add((int)joint.Value);
                lengths.Add((float)length.Value);

                Debug.WriteLine(angle.Value + " " + joint.Value + " " + length.Value);
            }

            parts[partIndex].Relative = new Vector2((float)rellativeX_DoubleUpDown.Value, (float)rellativeY_DoubleUpDown.Value);

            parts[partIndex].Collide = collide_checkBox.IsChecked.Value;

            parts[partIndex].setAnglesJointsLengths(angles.Count, angles, lengths, joints);
        }

        private void rerenderTextBoxesOfPartSides(int sides)
        {
            sides_textBox.setTextEventBypass(sides.ToString());

            Boolean addMode = sides - corners_stackPanel.Children.Count > 0;

            int timesRun = Math.Abs(sides - corners_stackPanel.Children.Count);

            for (int i = 0; i < timesRun; i++)
            {
                if (addMode)
                {
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());

                    DoubleUpDown angle_textBox = new DoubleUpDown();

                    DoubleUpDown length_textBox = new DoubleUpDown();

                    IntegerUpDown joints_textBox = new IntegerUpDown();

                    angle_textBox.DefaultValue = 0;
                    length_textBox.DefaultValue = 0;
                    joints_textBox.DefaultValue = 0;

                    angle_textBox.Value = 30;
                    length_textBox.Value = 10;
                    joints_textBox.Value = 0;

                    angle_textBox.ValueChanged += (temp, args) =>
                    {
                        loadDataFromGUItoSelectedPart();
                        rerenderParts();
                    };
                    length_textBox.ValueChanged += (temp, args) =>
                    {
                        loadDataFromGUItoSelectedPart();
                        rerenderParts();
                    };
                    joints_textBox.ValueChanged += (temp, args) =>
                    {
                        loadDataFromGUItoSelectedPart();
                        rerenderParts();
                    };

                    angle_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    length_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                    joints_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;

                    Grid.SetColumn(angle_textBox, 0);
                    Grid.SetColumn(length_textBox, 1);
                    Grid.SetColumn(joints_textBox, 2);

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

        private void loadDataFromSelectedPartToGUI()
        {
            corners_stackPanel.Children.Clear();

            sides_textBox.setTextEventBypass(parts[partIndex].GetNumberOfSides().ToString());

            List<float> angles = parts[partIndex].Angles;
            List<int> joints = parts[partIndex].Joints;
            List<float> lengths = parts[partIndex].Lengths;

            rellativeX_DoubleUpDown.Value = parts[partIndex].Relative.X;
            rellativeY_DoubleUpDown.Value = parts[partIndex].Relative.Y;
            collide_checkBox.IsChecked = parts[partIndex].Collide;

            for (int i = 0; i < parts[partIndex].GetNumberOfSides(); i++)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                DoubleUpDown angle_textBox = new DoubleUpDown();

                DoubleUpDown length_textBox = new DoubleUpDown();

                IntegerUpDown joints_textBox = new IntegerUpDown();

                angle_textBox.DefaultValue = 0;
                length_textBox.DefaultValue = 0;
                joints_textBox.DefaultValue = 0;

                angle_textBox.Value = angles[i];
                length_textBox.Value = lengths[i];
                joints_textBox.Value = joints[i];

                angle_textBox.ValueChanged += (temp, args) =>
                {
                    loadDataFromGUItoSelectedPart();
                    rerenderParts();
                };
                length_textBox.ValueChanged += (temp, args) =>
                {
                    loadDataFromGUItoSelectedPart();
                    rerenderParts();
                };
                joints_textBox.ValueChanged += (temp, args) =>
                {
                    loadDataFromGUItoSelectedPart();
                    rerenderParts();
                };

                angle_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                length_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                joints_textBox.HorizontalAlignment = HorizontalAlignment.Stretch;

                Grid.SetColumn(angle_textBox, 0);
                Grid.SetColumn(length_textBox, 1);
                Grid.SetColumn(joints_textBox, 2);

                grid.Children.Add(angle_textBox);
                grid.Children.Add(length_textBox);
                grid.Children.Add(joints_textBox);

                corners_stackPanel.Children.Add(grid);
            }
        }

        private void rerenderParts()
        {
            canvas.Children.Clear();
            foreach (Part part in parts)
            {
                Polygon polygon = part.getPolygonForRender(new Point(canvas.ActualWidth / 2 + part.Relative.X, canvas.ActualHeight / 2 + part.Relative.Y));

                if (part == parts[partIndex])
                    polygon.Stroke = Brushes.Coral;
                else
                    polygon.Stroke = Brushes.Black;

                canvas.Children.Add(polygon);

                foreach (Point local in part.getJointPositionsForRender(new Point(canvas.ActualWidth / 2 + part.Relative.X, canvas.ActualHeight / 2 + part.Relative.Y)))
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Stroke = Brushes.Red;
                    ellipse.StrokeThickness = 2;
                    ellipse.Width = 5;
                    ellipse.Height = 5;
                    ellipse.Fill = Brushes.Red;
                    ellipse.Margin = new Thickness(local.X, local.Y, 0, 0);
                    canvas.Children.Add(ellipse);
                }
            }
        }

        private void addPart(Part part)
        {
            parts.Add(part);
            rerenderParts();
        }

        private void removePart(Part part)
        {
            parts.Remove(part);
            rerenderParts();
        }

        private void changeSelected(Part part)
        {
            partIndex = parts.IndexOf(part);
            loadDataFromSelectedPartToGUI();
            rerenderParts();
        }




        private void fileSelector_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"F:\OneDrive\Documents\GitHub\Kana\Assets\VanillaModule\models";
            dialog.Filter = "model JSON file (*.json)|*.JSON";
            if (dialog.ShowDialog().Value)
            {
                path_textBox.Text = dialog.FileName;
                loadButton_Click(null, null);
            }
        }

        private void addPart_button_Click(object sender, RoutedEventArgs e)
        {
            addPart(new Part("Part " + parts.Count));
        }

        private void RemoveSelectedPart_Click(object sender, RoutedEventArgs e)
        {
            removePart(parts[partIndex]);
        }

        private void DuplicateSelectedPart_Click(object sender, RoutedEventArgs e)
        {
            addPart(parts[partIndex].Clone());
        }

        private void magicButton_Click(object sender, RoutedEventArgs e)
        {
            JObject jObject = new JObject();
            jObject.Add("name", name_textBox.Text);
            JArray partsArray = new JArray();
            foreach (Part part in parts)
            {
                JObject JPart = new JObject();
                JArray JVertices = JArraySerializer.SerializeVector2(part.Vertices);
                JArray JJoints = JArraySerializer.SerializeIntegeer(part.Joints);
                JArray JUVs = JArraySerializer.SerializeVector2(part.SUVs);

                JPart.Add("vertices", JVertices);
                JPart.Add("uvs", JUVs);
                JPart.Add("joints", JJoints);
                partsArray.Add(JPart);
            }

            jObject.Add("parts", partsArray);


            using (StreamWriter sw = new StreamWriter(path_textBox.Text))
            {
                sw.Write(JsonConvert.SerializeObject(jObject, Formatting.Indented));
            }

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Part part in parts.ToList())
            {
                removePart(part);
            }

            JObject jObject = JObject.Parse(File.ReadAllText(path_textBox.Text));
            name_textBox.Text = JSONUtil.ReadProperty<string>(jObject, "name");
            JArray jParts = JSONUtil.ReadArray(jObject, "parts");
            foreach (JToken partToken in jParts)
            {
                JObject partObject = (JObject)partToken;

                JArray jVertices = JSONUtil.ReadArray(partObject, "vertices");
                Vector2[] vertices = new Vector2[jVertices.Count];
                int vertexIndex = 0;
                foreach (JToken jVertex in jVertices)
                {
                    JArray coords = (JArray)jVertex;
                    vertices[vertexIndex] = new Vector2(
                        Newtonsoft.Json.Linq.Extensions.Value<float>(coords[0]),
                        Newtonsoft.Json.Linq.Extensions.Value<float>(coords[1]));
                    vertexIndex++;
                }

                JArray juvs = JSONUtil.ReadArray(partObject, "uvs");
                Vector2[] uvs = new Vector2[juvs.Count];
                int uvIndex = 0;
                foreach (JToken juv in juvs)
                {
                    JArray coords = (JArray)juv;
                    uvs[uvIndex] = new Vector2(Newtonsoft.Json.Linq.Extensions.Value<float>(coords[0]),
                        Newtonsoft.Json.Linq.Extensions.Value<float>(coords[1]));
                    uvIndex++;
                }

                JArray jJoints = JSONUtil.ReadArray(partObject, "joints");
                int[] joints = new int[jJoints.Count];
                int jointIndex = 0;
                foreach (JToken jJoint in jJoints)
                {
                    joints[jointIndex] = Newtonsoft.Json.Linq.Extensions.Value<int>(jJoint);
                    jointIndex++;
                }

                String partName = JSONUtil.ReadWithDefaultValue(partObject, "name", "Part " + parts.Count);

                Part part = new Part(partName);
                part.setFromJSON(vertices, uvs, joints);

                addPart(part);

                sides_textBox.Text = jVertices.Count.ToString();
            }

            changeSelected(parts.First());
        }
    }
}
