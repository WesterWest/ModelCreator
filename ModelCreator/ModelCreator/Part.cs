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
        public int Sides;
        public int[] Joints;
        public List<Vector2> Vertices = new List<Vector2>();
        public List<Vector2> UVs = new List<Vector2>();
        public Vector2 Relative;
        public Boolean Collide;

        public Polygon getPolygon(Vector2 offset) 
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 3;
            foreach (Vector2 vector in Vertices)
            {
                polygon.Points.Add(new System.Windows.Point(vector.X + offset.X, vector.Y + offset.Y));
            }
            return polygon;
        }

        public void setAnglesJointsLengths(int numberOfSides, float[] angles, float[] lengths, int[] joints)
        {
            Joints = joints;
            Sides = numberOfSides;
            Vertices.Clear();
            Vertices.Add(new Vector2());
            float lastAngle = 0;
            for(int i= 0; i < numberOfSides - 1; i++)
            {
                Vertices.Add(new Vector2((float)Math.Sin((lastAngle + angles[i]) / 180 * Math.PI) * lengths[i],
                                        (float)Math.Cos((lastAngle + angles[i]) / 180 * Math.PI) * lengths[i]));
                lastAngle += angles[i];
            }
            
        }
    }
}
