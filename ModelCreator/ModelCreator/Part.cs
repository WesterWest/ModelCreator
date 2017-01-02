using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ModelCreator
{
    class Part
    {
        public int Sides;
        public int[] Joints;
        public Vector2[] Vertices;
        public Vector2[] UVs;
        public Vector2 Relative;
        public Boolean Collide;

        public Polygon getPoly() 
        {
            Polygon polygon = new Polygon();
            foreach (Vector2 vector in Vertices)
            {
                polygon.Points.Add(new System.Windows.Point(vector.X, vector.Y));
            }
            return polygon;
        }
    }
}
