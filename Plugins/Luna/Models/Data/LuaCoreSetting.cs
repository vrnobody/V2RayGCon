namespace Luna.Models.Data
{
    public class LuaCoreSetting
    {

        public double index { get; set; }
        public bool isHidden { get; set; }
        public string name { get; set; }
        public string script { get; set; }

        public bool isLoadClr { get; set; }
        public bool isAutorun { get; set; }

        public LuaCoreSetting()
        {
            name = string.Empty;
            script = string.Empty;
            isAutorun = false;
            index = double.MaxValue;
            isHidden = true;
            isLoadClr = false;
        }
    }
}
