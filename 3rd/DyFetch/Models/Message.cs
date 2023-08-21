using System;
using System.Collections.Generic;
using System.Linq;

namespace DyFetch.Models
{
    public class Message
    {
        public string url = string.Empty;
        public int timeout = -1;
        public List<string> csses = new List<string>();
    }
}