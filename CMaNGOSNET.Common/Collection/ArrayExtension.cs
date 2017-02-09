using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Collection
{
    public static class ArrayExtension
    {
        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            T[] subArray = new T[length];

            Array.Copy(array, startIndex, subArray, 0, length);

            return subArray;
        }

        public static T[] SubArray<T>(this T[] array, int length)
        {
            return array.SubArray<T>(0, length);
        }
    }
}
