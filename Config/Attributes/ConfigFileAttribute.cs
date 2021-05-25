using System;

namespace SALT.Config.Attributes
{
    /// <summary>
    /// An attribute to make a class into a config file.
    /// </summary>
    public class ConfigFileAttribute : Attribute
    {
        /// <summary>
        /// Name of <see cref="ConfigFile"/>
        /// </summary>
        internal string FileName;
        /// <summary>
        /// Default section of <see cref="ConfigFile"/>
        /// </summary>
        internal string DefaultSection;

        /// <summary>
        /// Turns a class into a <see cref="ConfigFile"/>
        /// </summary>
        /// <param name="name">The name of the <see cref="ConfigFile"/></param>
        /// <param name="defaultsection">Name of the default <see cref="ConfigSection"/></param>
        public ConfigFileAttribute(string name, string defaultsection = "CONFIG")
        {
            this.FileName = name;
            this.DefaultSection = defaultsection;
        }
    }
}
