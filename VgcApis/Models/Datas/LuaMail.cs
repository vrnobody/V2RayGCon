namespace VgcApis.Models.Datas
{
    public class LuaMail : Interfaces.Lua.ILuaMail
    {
        public string from { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public double code { get; set; }

        public bool state { get; set; }

        public LuaMail() { }


        public bool GetState() => state;
        public string GetAddress() => from;
        public string GetTitle() => title;
        public string GetContent() => content;
        public double GetCode() => code;
    }
}
