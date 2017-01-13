using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModelCreator
{
    class Part
    {
        public List<Vector2> Vertices = new List<Vector2>();
        public List<Vector2> UVs = new List<Vector2>();
        public List<Vector2> DrawJoints = new List<Vector2>();
        public List<int> Joints = new List<int>();
        public List<float> Angles = new List<float>();
        public List<float> Lengths = new List<float>();
        public Vector2 Relative;
        public Boolean Collide;

        public Polygon getPolygon(Vector2 offset)
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 3;
            foreach (Vector2 vector in Vertices)
            {
                polygon.Points.Add(new System.Windows.Point(vector.X * 50 + offset.X, vector.Y * 50 + offset.Y));
            }
            return polygon;
        }

        public void setAnglesJointsLengths(int numberOfSides, float[] angles, float[] lengths, List<int> joints)
        {
            Joints = joints;
            UVs = Enumerable.Repeat(new Vector2(), angles.Length).ToList();
            Vertices.Clear();
            Vertices.Add(new Vector2());
            float lastAngle = 0;
            Vector2 lastVector2 = new Vector2();
            for (int i = 0; i < numberOfSides; i++)
            {
                Vector2 directionalVector = new Vector2((float)Math.Cos((lastAngle + angles[i]) / 180 * Math.PI),
                                        (float)Math.Sin((lastAngle + angles[i]) / 180 * Math.PI));

                float directionalVectorLength = (float)Math.Sqrt(directionalVector.X * directionalVector.X + directionalVector.Y * directionalVector.Y);
                float ratio = lengths[i] / directionalVectorLength;

                lastVector2 = new Vector2(directionalVector.X * ratio + lastVector2.X, directionalVector.Y * ratio + lastVector2.Y);

                if (i == numberOfSides - 1 && Math.Round(lastVector2.X) == 0 && Math.Round(lastVector2.Y) == 0)
                {

                }
                else
                {
                    Vertices.Add(lastVector2);
                }

                lastAngle += angles[i];
            }

            DrawJoints.Clear();

            joints.Add(0);

            for (int i = 0; i < Vertices.Count && Vertices.Count == joints.Count; i++)
            {
                Vector2 local = Vertices[i];
                Vector2 nextLocal = new Vector2();
                if (i + 2 > Vertices.Count)
                    nextLocal = Vertices[0];
                else
                    nextLocal = Vertices[i + 1];
                Vector2 dif = nextLocal - local;
                Vector2 onePeace = new Vector2(dif.X / (joints[i] + 1), dif.Y / (joints[i] + 1));
                for (int jointNumber = 0; jointNumber < joints[i]; jointNumber++)
                {
                    DrawJoints.Add(new Vector2(onePeace.X * (jointNumber + 1) + local.X, onePeace.Y * (jointNumber + 1) + local.Y));
                }
            }
        }

        public void setFromJSON(Vector2[] coords, Vector2[] uvs, int[] joints)
        {
            Vertices.Clear();
            UVs.Clear();
            Joints.Clear();


            UVs.AddRange(uvs);
            Joints.AddRange(joints);


            for (int i = 0; i < coords.Length; i++)
            {
                Vector2 vec1 = coords.noOutOfBounds(i) - coords.noOutOfBounds(i + 1);

                Lengths.Add((float)Math.Sqrt(Math.Pow(vec1.X, 2) + Math.Pow(vec1.Y, 2)));

                Vector2 vec2 = coords.noOutOfBounds(i + 2) - coords.noOutOfBounds(i + 1);

                Angles.Add(VectorUtil.AngleBetweenVector2(vec1, vec2));
            }
        }
    }
}
