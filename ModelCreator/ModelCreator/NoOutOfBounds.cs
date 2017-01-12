using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreator
{
    static class NoOutOfBounds
    {
        public static T noOutOfBounds<T>(this T[] input, int index)
        {
            index %= input.Length - 1;

            return input[index];
        }
    }
}
