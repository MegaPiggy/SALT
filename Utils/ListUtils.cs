using System;
using System.Collections.Generic;
using System.Linq;

namespace SALT.Utils
{
    public static class ListUtils
    {
        public static List<int> CreateEmptyIntList(int endat, int startfrom = 1)
        {
            List<int> list = new List<int>();
            for (int index = startfrom; index <= endat; ++index)
                list.Add(0);
            return list;
        }
        public static List<float> CreateEmptyFloatList(int endat, int startfrom = 1)
        {
            List<float> list = new List<float>();
            for (int index = startfrom; index <= endat; ++index)
                list.Add(0f);
            return list;
        }
    }
}
