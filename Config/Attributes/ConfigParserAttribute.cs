using System;
using SALT.Config.Parsing;

namespace SALT.Config.Attributes
{
    public class ConfigParserAttribute : Attribute
    {
        internal IStringParser Parser;

        public ConfigParserAttribute(IStringParser parser) => this.Parser = parser;
    }
}
