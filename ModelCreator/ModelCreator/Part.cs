using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModelCreator
{
    class Part
    {
        public const float GUI_SCALE_FACTOR = 10;
        private List<Vector2> vertices = new List<Vector2>();
        private List<Vector2> UVs = new List<Vector2>();
        private List<Vector2> jointsPosition = new List<Vector2>();
        private List<int> joints = new List<int>();
        private List<float> angles = new List<float>();
        private List<float> lengths = new List<float>();
        public Vector2 Relative { get; set; }
        public Boolean Collide { get; set; }
        public String PartName { get; set; }

        public List<int> Joints { get { return joints.ToList(); } }
        public List<float> Angles { get { return angles.ToList(); } }
        public List<float> Lengths { get { return lengths.ToList(); } }

        public List<Vector2> Vertices { get { return vertices.ToList(); } }
        public List<Vector2> SUVs { get { return UVs.ToList(); } }

        public Part(String name)
        {
            PartName = name;
        }

        public Polygon getPolygonForRender(Point offset)
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 3;
            foreach (Vector2 vector in vertices)
            {
                polygon.Points.Add(new System.Windows.Point(vector.X * GUI_SCALE_FACTOR + offset.X, vector.Y * GUI_SCALE_FACTOR + offset.Y));
            }
            return polygon;
        }

        public List<Point> getJointPositionsForRender(Point offset)
        {
            return jointsPosition.Select(vector => new Point(vector.X * GUI_SCALE_FACTOR + offset.X, vector.Y * GUI_SCALE_FACTOR + offset.Y)).ToList();
        }

        public void setAnglesJointsLengths(int numberOfSides, List<float> pAngles, List<float> pLengths, List<int> pJoints)
        {
            this.angles = pAngles;
            this.lengths = pLengths;
            this.joints = pJoints;

            vertices.Clear();

            if (numberOfSides > 0)
                vertices.Add(new Vector2());

            float lastAngle = 0;
            Vector2 lastVector2 = new Vector2();

            for (int i = 0; i < numberOfSides; i++)
            {
                Vector2 directionalVector = new Vector2((float)Math.Cos((lastAngle + angles[i]) / 180 * Math.PI),
                                        (float)Math.Sin((lastAngle + angles[i]) / 180 * Math.PI));

                float ratio = lengths[i] / directionalVector.GetLength();

                lastVector2 = new Vector2(directionalVector.X * ratio + lastVector2.X, directionalVector.Y * ratio + lastVector2.Y);

                if (i == numberOfSides - 1 && (Math.Round(lastVector2.X) == 0 || Math.Round(lastVector2.Y) == 0))
                {

                }
                else if (i == numberOfSides - 1 && !(Math.Round(lastVector2.X) == 0 || Math.Round(lastVector2.Y) == 0))
                {
                    vertices.Add(lastVector2);
                    angles.Add(Vector2.AngleBetween(lastVector2, vertices.First()));
                    lengths.Add(lastVector2.GetLength());
                    joints.Add(0);
                }
                else
                {
                    vertices.Add(lastVector2);
                }


                lastAngle += angles[i];
            }

            this.UVs = Enumerable.Repeat(new Vector2(), GetNumberOfSides()).ToList();

            verifyListsIntegrity();

            calculateJointsPosition();
        }

        public void setFromJSON(Vector2[] coords, Vector2[] uvs, int[] joints)
        {
            vertices.Clear();
            UVs.Clear();
            this.joints.Clear();


            UVs.AddRange(uvs);
            this.joints.AddRange(joints);


            for (int i = 0; i < coords.Length; i++)
            {
                Vector2 vec1 = coords.noOutOfBounds(i) - coords.noOutOfBounds(i + 1);

                lengths.Add((float)Math.Sqrt(Math.Pow(vec1.X, 2) + Math.Pow(vec1.Y, 2)));

                Vector2 vec2 = coords.noOutOfBounds(i + 2) - coords.noOutOfBounds(i + 1);

                angles.Add(Vector2.AngleBetween(vec1, vec2));
            }

            vertices = coords.ToList();

            verifyListsIntegrity();

            calculateJointsPosition();
        }

        public override string ToString()
        {
            return PartName;
        }

        public Part Clone()
        {
            return new Part(PartName + " (copy)")
            {
                vertices = vertices.ToList(),
                UVs = UVs.ToList(),
                joints = joints.ToList(),
                angles = angles.ToList(),
                lengths = lengths.ToList(),
                Relative = Relative,
                Collide = Collide,
                jointsPosition = jointsPosition.ToList()
            };
        }

        public int GetNumberOfSides()
        {
            return vertices.Count;
        }

        private void verifyListsIntegrity()
        {
            int sides = GetNumberOfSides();

            if (sides != angles.Count || sides != UVs.Count || sides != joints.Count || sides != lengths.Count)
            {
                throw new InconsistentListsCountException();
            }
        }

        private void calculateJointsPosition()
        {
            jointsPosition.Clear();
            for (int i = 0; i < vertices.Count && vertices.Count == joints.Count; i++)
            {
                Vector2 local = vertices[i];
                Vector2 nextLocal = new Vector2();
                if (i + 2 > vertices.Count)
                    nextLocal = vertices[0];
                else
                    nextLocal = vertices[i + 1];
                Vector2 dif = nextLocal - local;
                Vector2 onePeace = new Vector2(dif.X / (joints[i] + 1), dif.Y / (joints[i] + 1));
                for (int jointNumber = 0; jointNumber < joints[i]; jointNumber++)
                {
                    jointsPosition.Add(new Vector2(onePeace.X * (jointNumber + 1) + local.X, onePeace.Y * (jointNumber + 1) + local.Y));
                }
            }
        }
    }
}
