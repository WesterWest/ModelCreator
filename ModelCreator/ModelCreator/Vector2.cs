using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    class Vector2
    {
        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }

        public Vector2 ()
        {

        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator - (Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }
    }
}
