using System;

namespace SALT.Config.Attributes
{
    public class ConfigSectionAttribute : Attribute
    {
        internal string SectionName;

        public ConfigSectionAttribute()
        {
        }

        public ConfigSectionAttribute(string sectionname) => this.SectionName = sectionname;
    }
}
