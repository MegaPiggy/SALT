namespace SALT.Config.Parsing
{
    public class DelegateStringParser<T> : StringParser<T>
    {
        private EncodeGenericDelegate encoder;
        private ParseGenericDelegate parser;

        public DelegateStringParser(
          EncodeGenericDelegate encoder,
          ParseGenericDelegate parser)
        {
            this.encoder = encoder;
            this.parser = parser;
        }

        public override string Encode(T obj) => this.encoder(obj);

        public override T Parse(string str) => this.parser(str);

        public delegate string EncodeGenericDelegate(T obj);

        public delegate T ParseGenericDelegate(string str);
    }
}
