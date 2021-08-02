using System.Collections.Generic;

namespace Pacman.Models.Data
{
    public class Package
    {
        public List<Bean> beans;
        public string uid { get; set; }
        public string name { get; set; }

        public int strategy { get; set; }

        public string interval { get; set; }

        public string url { get; set; }

        public Package()
        {
            beans = new List<Bean>();
            name = string.Empty;
            uid = string.Empty;
            strategy = 0;
            interval = string.Empty;
            url = string.Empty;
        }
    }
}
