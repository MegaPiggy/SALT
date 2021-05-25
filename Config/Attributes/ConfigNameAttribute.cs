using System;

namespace SALT.Config.Attributes
{
    public class ConfigNameAttribute : Attribute
    {
        public string Name;

        public ConfigNameAttribute(string name) => this.Name = name;
    }
}
