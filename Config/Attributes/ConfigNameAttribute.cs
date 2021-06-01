using System;

namespace SALT.Config.Attributes
{
    public class ConfigNameAttribute : Attribute
    {
        internal string Name;

        public ConfigNameAttribute(string name) => this.Name = name;
    }
}
