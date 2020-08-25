using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class StreamComponent
    {
        public bool dropDownStyle;

        public string name;
        public string network;
        public string optionPath;
        public Dictionary<string, string> options;

        public string option2Name;
        public string option2Path;
        public string option3Name;
        public string option3Path;

        public StreamComponent()
        {
            dropDownStyle = false;
            name = string.Empty;
            network = string.Empty;
            optionPath = string.Empty;
            option2Name = string.Empty;
            option2Path = string.Empty;
            option3Name = string.Empty;
            option3Path = string.Empty;
            options = new Dictionary<string, string>();
        }
    }
}
