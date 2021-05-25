using System;
using System.Collections.Generic;
using SALT.Extensions;

namespace SALT.Config.Parsing
{
    public static class ParserRegistry
    {
        private static readonly Dictionary<Type, IStringParser> parsers = new Dictionary<Type, IStringParser>()
    {
      {
        typeof (int),
        (IStringParser) new DelegateStringParser<int>((DelegateStringParser<int>.EncodeGenericDelegate<int>) (x => x.ToString()), new DelegateStringParser<int>.ParseGenericDelegate<int>(int.Parse))
      },
      {
        typeof (bool),
        (IStringParser) new DelegateStringParser<bool>((DelegateStringParser<bool>.EncodeGenericDelegate<bool>) (x => x.ToString()), new DelegateStringParser<bool>.ParseGenericDelegate<bool>(bool.Parse))
      },
      {
        typeof (float),
        (IStringParser) new DelegateStringParser<float>((DelegateStringParser<float>.EncodeGenericDelegate<float>) (x => x.ToString()), new DelegateStringParser<float>.ParseGenericDelegate<float>(float.Parse))
      },
      {
        typeof (long),
        (IStringParser) new DelegateStringParser<long>((DelegateStringParser<long>.EncodeGenericDelegate<long>) (x => x.ToString()), new DelegateStringParser<long>.ParseGenericDelegate<long>(long.Parse))
      },
      {
        typeof (double),
        (IStringParser) new DelegateStringParser<double>((DelegateStringParser<double>.EncodeGenericDelegate<double>) (x => x.ToString()), new DelegateStringParser<double>.ParseGenericDelegate<double>(double.Parse))
      },
      {
        typeof (uint),
        (IStringParser) new DelegateStringParser<uint>((DelegateStringParser<uint>.EncodeGenericDelegate<uint>) (x => x.ToString()), new DelegateStringParser<uint>.ParseGenericDelegate<uint>(uint.Parse))
      },
      {
        typeof (byte),
        (IStringParser) new DelegateStringParser<byte>((DelegateStringParser<byte>.EncodeGenericDelegate<byte>) (x => x.ToString()), new DelegateStringParser<byte>.ParseGenericDelegate<byte>(byte.Parse))
      },
      {
        typeof (sbyte),
        (IStringParser) new DelegateStringParser<sbyte>((DelegateStringParser<sbyte>.EncodeGenericDelegate<sbyte>) (x => x.ToString()), new DelegateStringParser<sbyte>.ParseGenericDelegate<sbyte>(sbyte.Parse))
      },
      {
        typeof (short),
        (IStringParser) new DelegateStringParser<short>((DelegateStringParser<short>.EncodeGenericDelegate<short>) (x => x.ToString()), new DelegateStringParser<short>.ParseGenericDelegate<short>(short.Parse))
      },
      {
        typeof (ushort),
        (IStringParser) new DelegateStringParser<ushort>((DelegateStringParser<ushort>.EncodeGenericDelegate<ushort>) (x => x.ToString()), new DelegateStringParser<ushort>.ParseGenericDelegate<ushort>(ushort.Parse))
      },
      {
        typeof (ulong),
        (IStringParser) new DelegateStringParser<ulong>((DelegateStringParser<ulong>.EncodeGenericDelegate<ulong>) (x => x.ToString()), new DelegateStringParser<ulong>.ParseGenericDelegate<ulong>(ulong.Parse))
      },
      {
        typeof (string),
        (IStringParser) new DelegateStringParser<string>((DelegateStringParser<string>.EncodeGenericDelegate<string>) (x => x.ToQuotedString()), (DelegateStringParser<string>.ParseGenericDelegate<string>) (x => x.FromQuotedString()))
      }
    };

        public static IStringParser GetParser(Type type) => type.IsArray ? (IStringParser)new ArrayParser(type) : (type.IsEnum ? ParserRegistry.GetEnumParser(type) : ParserRegistry.parsers[type]);

        public static bool TryGetParser(Type type, out IStringParser parser)
        {
            try
            {
                parser = ParserRegistry.GetParser(type);
                return true;
            }
            catch
            {
                parser = (IStringParser)null;
                return false;
            }
        }

        private static IStringParser GetEnumParser(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Type is not an enum!");
            IStringParser stringParser;
            if (!ParserRegistry.parsers.TryGetValue(enumType, out stringParser))
            {
                stringParser = (IStringParser)new ParserRegistry.EnumStringParser(enumType);
                ParserRegistry.parsers[enumType] = stringParser;
            }
            return stringParser;
        }

        public static void RegisterParser(IStringParser parser) => ParserRegistry.parsers[parser.ParsedType] = parser;

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
