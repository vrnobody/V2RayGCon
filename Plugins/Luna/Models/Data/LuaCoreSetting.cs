namespace Luna.Models.Data
{
    public class LuaCoreSetting
    {
        public string name { get; set; }
        public string script { get; set; }
        public bool isAutorun { get; set; }

        public LuaCoreSetting()
        {
            name = string.Empty;
            script = string.Empty;
            isAutorun = false;
        }
    }
}
