using System;

namespace SALT.Config.Attributes
{
    /// <summary>
    /// Callback attribute for fields in a <see cref="ConfigFile"/>. Runs when the field has changed.
    /// </summary>
    public class ConfigCallbackAttribute : Attribute
    {
        /// <summary>
        /// The method inside <see cref="ConfigFile"/> to run when the field is changed.
        /// </summary>
        internal string methodName;

        /// <summary>
        /// Registers a method inside <see cref="ConfigFile"/> to run when the field is changed.
        /// </summary>
        /// <param name="methodName">The name of the method inside <see cref="ConfigFile"/></param>
        public ConfigCallbackAttribute(string methodName) => this.methodName = methodName;
    }
}
