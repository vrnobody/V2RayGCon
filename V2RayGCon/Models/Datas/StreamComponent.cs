using System.Collections.Generic;

namespace V2RayGCon.Models.Datas
{
    public class StreamComponent
    {
        public bool dropDownStyle;

        public string name;
        public string network;
        public List<string> paths;
        public Dictionary<string, string> options;

        public StreamComponent()
        {
            dropDownStyle = false;
            name = string.Empty;
            network = string.Empty;
            paths = new List<string>();
            options = new Dictionary<string, string>();
        }
    }
}
