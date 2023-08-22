using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DyFetch
{
    class Program
    {
        static void Main(string[] args)
        {
            var configs = new Models.Configs(args);
            if (configs.help)
            {
                configs.ShowHelp();
                Environment.Exit(0);
            }

            using (var fetcher = new Comps.Fetcher(configs))
            {
                var pin = configs.pipeIn;
                var pout = configs.pipeOut;
                if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(pout))
                {
                    var html = fetcher.Fetch(configs.url, configs.csses, configs.timeout, configs.wait);
                    Console.WriteLine(html);
                }
                else
                {
                    using (var plumber = new Comps.Plumber(pin, pout, fetcher))
                    {
                        plumber.Work();
                    }
                }
            }
            Console.WriteLine("Goodbye!");
            Environment.Exit(0);
        }
    }
}
