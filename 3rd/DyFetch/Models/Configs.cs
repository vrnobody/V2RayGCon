using System;
using System.Collections.Generic;
using NDesk.Options;

namespace DyFetch.Models
{
    internal class Configs
    {
        public bool headless = false;
        public string pipeIn = null;
        public string pipeOut = null;
        public bool help = false;
        public bool ignoreCertError = false;
        public string proxy = null;
        public List<string> csses = new List<string>();
        public int timeout = -1;
        public string url = null;
        public int wait = -1;
        public string driverDir = null;
        public string userDataDir = null;
        public string file = null;
        public bool useChrome = false;

        readonly OptionSet opts = null;

        readonly string version,
            date;

        public Configs(string[] args)
            : this()
        {
            Parse(args);
        }

        public Configs()
        {
            version = "0.0.6";
            date = "2024-10-17";

            opts = new OptionSet()
            {
                { "pipein=", "anonymous input-pipe handle", v => pipeIn = v },
                { "pipeout=", "anonymous output-pipe handle", v => pipeOut = v },
                { "c|css=", "wait until one of the css selectors match", v => csses.Add(v) },
                { "f|file=", "save HTML to file", v => file = v },
                { "i|ignore", "ignore certificate errors", v => ignoreCertError = v != null },
                { "p|proxy=", "HTTP proxy in host:port format", v => proxy = v },
                { "s|headless", "headless mode", v => headless = v != null },
                { "t|timeout=", "wait timeout in milliseconds", (int v) => timeout = v },
                { "u|url=", "the URL to download", v => url = v },
                { "w|wait=", "wait until match in milliseconds", (int v) => wait = v },
                { "chrome", "use chrome (default is firefox)", v => useChrome = v != null },
                { "driver=", "broswer driver dir", v => driverDir = v },
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
            Console.WriteLine($"DyFetch v{version} {date}");
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine(
                "DyFetch.exe -proxy=\"127.0.0.1:8080\" -t 30000 -u \"https://www.bing.com\""
            );
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
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
