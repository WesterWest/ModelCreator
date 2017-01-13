using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    class JArraySerializer
    {
        public static JArray SerializeVector2(List<Vector2> list)
        {
            JArray origin = new JArray();
            foreach (Vector2 steam in list)
            {
                JArray uPlay = new JArray();
                uPlay.Add(steam.X);
                uPlay.Add(steam.Y);
                origin.Add(uPlay);
            }
            return origin;
        }

        public static JArray SerializeIntegeer(List<int> list)
        {
            JArray origin = new JArray();
            foreach (int steam in list)
            {
                origin.Add(steam);
            }
            return origin;
        }
    }
}
