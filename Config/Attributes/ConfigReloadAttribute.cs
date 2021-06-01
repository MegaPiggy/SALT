using System;

namespace SALT.Config.Attributes
{
    public class ConfigReloadAttribute : Attribute
    {
        internal ReloadMode Mode;

        public ConfigReloadAttribute(ReloadMode mode) => this.Mode = mode;
    }
}
