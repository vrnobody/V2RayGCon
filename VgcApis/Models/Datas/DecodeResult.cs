namespace VgcApis.Models.Datas
{
    public class DecodeResult
    {
        public string name;
        public string config;

        public DecodeResult(string name, string config)
        {
            this.config = config;
            this.name = name;
        }

        public DecodeResult()
            : this(string.Empty, string.Empty) { }
    }
}
