using System;
using System.Collections.Generic;

namespace SALT.Config.Parsing
{
    public class ArrayParser : IStringParser
    {
        public Type ParsedType { get; }

        public ArrayParser(Type array) => this.ParsedType = array;

        public string EncodeObject(object obj)
        {
            Array array = (Array)obj ?? Array.CreateInstance(this.ParsedType.GetElementType(), 0);
            IStringParser parser = ParserRegistry.GetParser(this.ParsedType.GetElementType());
            string str = "[";
            for (int index = 0; index < array.Length; ++index)
            {
                str += parser.EncodeObject(array.GetValue(index));
                if (index != array.Length - 1)
                    str += ", ";
            }
            return str + "]";
        }

        public string GetUsageString() => this.ParsedType.ToString();

        public object ParseObject(string str)
        {
            IStringParser parser = ParserRegistry.GetParser(this.ParsedType.GetElementType());
            str = str.Trim(' ', '[', ']');
            List<object> objectList = new List<object>();
            string str1 = str;
            char[] chArray = new char[1] { ',' };
            foreach (string str2 in str1.Split(chArray))
                objectList.Add(parser.ParseObject(str2.Trim()));
            Array instance = Array.CreateInstance(this.ParsedType.GetElementType(), objectList.Count);
            for (int index = 0; index < objectList.Count; ++index)
                instance.SetValue(objectList[index], index);
            return (object)instance;
        }
    }
}
