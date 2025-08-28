namespace Commander.Models.Data
{
    public class CmderParam
    {
        public string name = "";
        public string exe = "";
        public string wrkDir = "";

        public string args = "";
        public string envVars = "";
        public string stdInEncoding = "";
        public string stdOutEncoding = "";

        public bool hideWindow = true;
        public bool writeToStdIn = false;

        public string stdInContent = "";
    }
}
