using System;

namespace SALT.Config.Attributes
{
    public class ConfigReloadAttribute : Attribute
    {
        public ReloadMode Mode;

        public ConfigReloadAttribute(ReloadMode mode) => this.Mode = mode;
    }
}
