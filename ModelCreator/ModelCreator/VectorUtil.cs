using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    static class VectorUtil
    {
        public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            float angle = (float)Math.Abs(Math.Atan2(vec1.Y, vec1.X) * 180 / Math.PI -
                                Math.Atan2(vec2.Y, vec2.X) * 180 / Math.PI);
            return angle;
        }
    }
}
