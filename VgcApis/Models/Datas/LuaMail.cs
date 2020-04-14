namespace VgcApis.Models.Datas
{
    public class LuaMail : Interfaces.Lua.ILuaMail
    {
        public string from { get; set; }
        public string header { get; set; }
        public string body { get; set; }
        public string footer { get; set; }

        public LuaMail() { }

        public string GetAddress() => from;
        public string GetHeader() => header;
        public string GetBody() => body;
        public string GetFooter() => footer;
    }
}
