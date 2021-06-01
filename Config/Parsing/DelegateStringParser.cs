namespace SALT.Config.Parsing
{
    public class DelegateStringParser<T> : StringParser<T>
    {
        private DelegateStringParser<T>.EncodeGenericDelegate<T> encoder;
        private DelegateStringParser<T>.ParseGenericDelegate<T> parser;

        public DelegateStringParser(
          DelegateStringParser<T>.EncodeGenericDelegate<T> encoder,
          DelegateStringParser<T>.ParseGenericDelegate<T> parser)
        {
            this.encoder = encoder;
            this.parser = parser;
        }

        public override string Encode(T obj) => this.encoder(obj);

        public override T Parse(string str) => this.parser(str);

        public delegate string EncodeGenericDelegate<in T>(T obj);

        public delegate T ParseGenericDelegate<T>(string str);
    }
}
