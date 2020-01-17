using System.Collections.Generic;

namespace Pacman.Models.Data
{
    public class Package
    {
        public List<Bean> beans;
        public string uid { get; set; }
        public string name { get; set; }

        public Package()
        {
            beans = new List<Bean>();
            name = string.Empty;
            uid = string.Empty;
        }
    }
}
