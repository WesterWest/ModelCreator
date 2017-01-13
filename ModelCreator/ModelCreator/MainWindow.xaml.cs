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
            partList_listView.SelectionChanged += (kana, windowsJeSracka) => changeSelected((Part)partList_listView.SelectedItem);
            addPart(new Part());
            sides_textBox.ContentChanged += UpdateSides;
            /*
            Vector2 vec1 = new Vector2(1, 0);
            Vector2 vec2 = new Vector2(0.2f, 1);
            float solution = VectorUtil.AngleBetweenVector2(vec1, vec2);
            */
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
                JArray JUVs = JArraySerializer.SerializeVector2(part.UVs);

                JPart.Add("vertices", JVertices);
                JPart.Add("uvs", JUVs);
                JPart.Add("joints", JJoints);
                partsArray.Add(JPart);
            }

            jObject.Add("parts", partsArray);

            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(path_textBox.Text))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, jObject);
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

                Part part = new Part();
                part.setFromJSON(vertices, uvs, joints);

                addPart(part);

                sides_textBox.Text = jVertices.Count.ToString();

                int i = 0;
                foreach (UIElement element in corners_stackPanel.Children)
                {
                    Grid grid = (Grid)element;
                    DoubleUpDown angle = (DoubleUpDown)grid.Children[0];
                    DoubleUpDown length = (DoubleUpDown)grid.Children[1];
                    IntegerUpDown joint = (IntegerUpDown)grid.Children[2];
                    angle.Value = part.Angles[i];
                    length.Value = part.Lengths[i];
                    joint.Value = part.Joints[i];
                    i++;
                }


                canvas.Children.Clear();
                canvas.Children.Add(parts[partIndex].getPolygon(new Vector2((float)canvas.ActualWidth / 2, (float)canvas.ActualHeight / 2)));
                foreach (Vector2 local in parts[partIndex].DrawJoints)
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

            rerenderParts();

        }

        private void rerenderParts()
        {
            canvas.Children.Clear();
            foreach (Part part in parts)
            {
                Polygon polygon = part.getPolygon(new Vector2((float)canvas.ActualWidth / 2, (float)canvas.ActualHeight / 2));

                if (part == parts[partIndex])
                    polygon.Stroke = Brushes.Coral;
                else
                    polygon.Stroke = Brushes.Black;

                canvas.Children.Add(polygon);

                foreach (Vector2 local in part.DrawJoints)
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

        private void addPart(Part part)
        {
            parts.Add(part);
        }

        private void removePart(Part part)
        {
            parts.Remove(part);
        }

        private void changeSelected(Part part)
        {
            partIndex = parts.IndexOf(part);
            rerenderParts();
        }

        private void addPart_button_Click(object sender, RoutedEventArgs e)
        {
            addPart(new Part());
        }
    }
}
