using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleman_1
{
    internal class Utils
    {
        public static void switchListOrder<T>(List<T> list)
        {
            // This puts the first to the last index
            // Example:
            // Input:  1, 2, 3, 4
            // Output: 2, 3, 4, 1
            for (int i = 0; i < list.Count - 1; i++)
            {
                T temp = list[i];
                list[i] = list[i + 1];
                list[i + 1] = temp;
            }
        }

    }
}
