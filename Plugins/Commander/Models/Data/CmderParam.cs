namespace Commander.Models.Data
{
    public class CmderParam : VgcApis.Interfaces.IHasIndex
    {
        public string name = "";
        public string exe = "";
        public string wrkDir = "";
        public double index = 1;

        public string args = "";
        public string envVars = "";
        public string stdInEncoding = "cp936";
        public string stdOutEncoding = "cp936";

        public bool hideWindow = true;
        public bool writeToStdIn = false;
        public bool useShell = false;

        public string stdInContent = "";

        public CmderParam() { }

        #region public methods
        public void Copy(CmderParam src)
        {
            this.name = src.name;
            this.exe = src.exe;
            this.wrkDir = src.wrkDir;
            this.index = src.index;

            this.args = src.args;
            this.envVars = src.envVars;
            this.stdInEncoding = src.stdInEncoding;
            this.stdOutEncoding = src.stdOutEncoding;

            this.hideWindow = src.hideWindow;
            this.writeToStdIn = src.writeToStdIn;
            this.useShell = src.useShell;

            this.stdInContent = src.stdInContent;
        }
        #endregion

        #region IHasIndex
        public double GetIndex() => index;

        public void SetIndex(double value) => this.index = value;
        #endregion
    }
}
