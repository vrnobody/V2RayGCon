namespace Commander.Models.Data
{
    public class CmderParam
    {
        public string name = "";
        public string exe = "";
        public string wrkDir = "";

        public string args = "";
        public string envVars = "";
        public string stdInEncoding = "cp936";
        public string stdOutEncoding = "cp936";

        public bool hideWindow = true;
        public bool writeToStdIn = false;
        public bool useShell = false;

        public string stdInContent = "";
    }
}
