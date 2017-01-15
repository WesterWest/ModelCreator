using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    class InconsistentListsCountException : Exception
    {
        public override string ToString()
        {
            Random rand = new Random();
            return $"Joints, Lengths, Angles, UVs, Vertices Arrays count is not equal (x666{rand.Next(1000)}69)";
        }

        public override string Message
        {
            get
            {
                return ToString();
            }
        }
    }
}
