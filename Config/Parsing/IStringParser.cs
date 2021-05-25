using System;

namespace SALT.Config.Parsing
{
    public interface IStringParser
    {
        Type ParsedType { get; }

        object ParseObject(string str);

        string EncodeObject(object obj);

        string GetUsageString();
    }
}
