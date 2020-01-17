using System.Collections.Generic;

namespace V2RayGCon.Model.Data
{
    public class StreamComponent
    {
        public bool dropDownStyle;

        public string name;
        public string network;
        public string optionPath;
        public Dictionary<string, string> options;

        public StreamComponent()
        {
            dropDownStyle = false;
            name = string.Empty;
            network = string.Empty;
            optionPath = string.Empty;
            options = new Dictionary<string, string>();
        }
    }
}
