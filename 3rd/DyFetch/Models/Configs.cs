using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DyFetch.Models
{
    internal class Configs
    {
        public bool headless = false;
        public string pipeIn = null;
        public string pipeOut = null;
        public bool help = false;
        public int port = -1;
        public List<string> csses = new List<string>();
        public int timeout = -1;
        public string url = null;

        readonly OptionSet opts = null;

        public Configs(string[] args) : this()
        {
            Parse(args);
        }

        public Configs()
        {
            opts = new OptionSet()
            {
                { "pipein=", "input anonymous pipe handle", v => pipeIn = v },
                { "pipeout=", "output anonymous pipe handle", v => pipeOut = v },
                { "p|port=", "proxy port number", (int v) => port = v },
                { "u|url=", "the URL to download", v => url = v  },
                { "s|headless", "headless mode", v => headless = v != null },
                { "c|css=", "wait until one of the css selectors match", v => csses.Add(v) },
                { "t|timeout=",  "wait timeout in milliseconds", (int v) => timeout = v },
                { "h|help", "show help", v => help = v != null },
            };
        }

        #region properties

        #endregion

        #region public methods
        public void Parse(string[] args)
        {
            try
            {
                opts.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("DyFetch v0.0.1 2023-08-21");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("DyFetch.exe -p 8080 -t 20000 -u \"https://www.bing.com\"");
            Console.WriteLine("");
            Console.WriteLine("Params:");
            opts?.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");

        }
        #endregion

        #region private methods

        #endregion

        #region protected methods

        #endregion
    }
}