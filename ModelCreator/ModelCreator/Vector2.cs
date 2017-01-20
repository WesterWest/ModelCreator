using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    struct Vector2
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

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }



        public float GetLength()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public static float AngleBetween(Vector2 vec1, Vector2 vec2)
        {
            float angle = (float)Math.Abs(Math.Atan2(vec1.Y, vec1.X) * 180 / Math.PI -
                                Math.Atan2(vec2.Y, vec2.X) * 180 / Math.PI);
            return angle;
        }

        public static Vector2 operator -(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }
    }
}
