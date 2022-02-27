using System;
using System.Collections.Generic;
using SALT.Extensions;

namespace SALT.Config.Parsing
{
    public static class ParserRegistry
    {
        private static readonly Dictionary<Type, IStringParser> parsers = new Dictionary<Type, IStringParser>()
        {
            {typeof(int),new DelegateStringParser<int>(x=>x.ToString(),int.Parse) },
            {typeof(bool),new DelegateStringParser<bool>(x=>x.ToString(),bool.Parse) },
            {typeof(Half),new DelegateStringParser<Half>(x=>x.ToString(),Half.Parse) },
            {typeof(float),new DelegateStringParser<float>(x=>x.ToString(),float.Parse) },
            {typeof(long),new DelegateStringParser<long>(x=>x.ToString(),long.Parse) },
            {typeof(double),new DelegateStringParser<double>(x=>x.ToString(),double.Parse) },
            {typeof(decimal),new DelegateStringParser<decimal>(x=>x.ToString(),decimal.Parse) },
            {typeof(uint),new DelegateStringParser<uint>(x=>x.ToString(),uint.Parse) },
            {typeof(byte),new DelegateStringParser<byte>(x=>x.ToString(),byte.Parse) },
            {typeof(sbyte),new DelegateStringParser<sbyte>(x=>x.ToString(),sbyte.Parse) },
            {typeof(short),new DelegateStringParser<short>(x=>x.ToString(),short.Parse) },
            {typeof(ushort),new DelegateStringParser<ushort>(x=>x.ToString(),ushort.Parse) },
            {typeof(ulong),new DelegateStringParser<ulong>(x=>x.ToString(),ulong.Parse) },
            {typeof(DateTime),new DelegateStringParser<DateTime>(x=>x.ToString(),DateTime.Parse) },
            {typeof(string),new DelegateStringParser<string>(x=>x.ToQuotedString(),x=>x.FromQuotedString()) }
        };

        public static IStringParser GetParser(Type type) => type.IsArray ? new ArrayParser(type) : (type.IsEnum ? GetEnumParser(type) : parsers[type]);

        public static bool TryGetParser(Type type, out IStringParser parser)
        {
            try
            {
                parser = GetParser(type);
                return true;
            }
            catch
            {
                parser = null;
                return false;
            }
        }

        private static IStringParser GetEnumParser(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Type is not an enum!");
            IStringParser stringParser;
            if (!parsers.TryGetValue(enumType, out stringParser))
            {
                stringParser = new EnumStringParser(enumType);
                parsers[enumType] = stringParser;
            }
            return stringParser;
        }

        public static void RegisterParser(IStringParser parser) => parsers[parser.ParsedType] = parser;

        internal class EnumStringParser : IStringParser
        {
            public Type ParsedType { get; }

            public EnumStringParser(Type enumType) => this.ParsedType = enumType;

            public string EncodeObject(object obj) => Enum.GetName(this.ParsedType, obj);

            public string GetUsageString() => this.ParsedType.ToString();

            public object ParseObject(string str) => Enum.Parse(this.ParsedType, str, true);
        }
    }
}
