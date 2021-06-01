using System;

namespace SALT.Config.Attributes
{
    public class ConfigSectionAttribute : Attribute
    {
        internal string SectionName;

        internal ConfigSectionAttribute()
        {
        }

        public ConfigSectionAttribute(string sectionname) => this.SectionName = sectionname;
    }
}
