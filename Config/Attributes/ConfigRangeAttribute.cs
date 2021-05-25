using System;

namespace SALT.Config.Attributes
{
    public class ConfigRangeAttribute : Attribute
    {
        internal int min;
        internal int max;

        public ConfigRangeAttribute(int min = int.MinValue, int max = int.MaxValue)
        {
            this.min = min;
            this.max = max;
        }
    }
}
