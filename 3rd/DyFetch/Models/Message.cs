using System.Collections.Generic;

namespace DyFetch.Models
{
    public class Message
    {
        public string url = string.Empty;
        public int timeout = -1;
        public int wait = -1;
        public List<string> csses = new List<string>();
    }
}
