using System;

namespace SALT.Config.Parsing
{
    public abstract class StringParser<T> : IStringParser
    {
        public Type ParsedType => typeof(T);

        public abstract string Encode(T obj);

        public abstract T Parse(string str);

        public string EncodeObject(object obj) => this.Encode((T)obj);

        public object ParseObject(string str) => (object)this.Parse(str);

        public string GetUsageString() => this.ParsedType.Name;
    }
}
