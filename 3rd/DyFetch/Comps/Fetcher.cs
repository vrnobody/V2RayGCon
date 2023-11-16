using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DyFetch.Comps
{
    internal class Fetcher : IDisposable
    {
        readonly FirefoxDriver driver;
        private bool disposedValue;

        public Fetcher(Models.Configs configs)
        {
            var options = CreateOptions(configs);
            var dir = configs.driverDir;
            driver = string.IsNullOrEmpty(dir)
                ? new FirefoxDriver(options)
                : new FirefoxDriver(dir, options);
        }

        #region properties

        #endregion

        #region public methods
        public string Fetch(string url, List<string> csses, int timeout, int wait)
        {
            timeout = timeout > 0 ? timeout : Models.Consts.DefaultFetchTimeout;
            try
            {
                var span = TimeSpan.FromMilliseconds(timeout);
                var w = new WebDriverWait(driver, span);
                driver.Navigate().GoToUrl(url);
                if (csses != null && csses.Count > 0)
                {
                    var cts = new CancellationTokenSource(span);
                    WaitForOneOfCsses(csses, w, cts.Token);
                }
                if (wait > 0)
                {
                    Thread.Sleep(wait);
                }
                // driver.GetScreenshot().SaveAsFile("c:/dyfetch-debug.png");
                return driver.PageSource;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }

        #endregion

        #region private methods
        void WaitForOneOfCsses(
            IEnumerable<string> csses,
            WebDriverWait wait,
            CancellationToken token
        )
        {
            var bys = csses.Select(css => By.CssSelector(css)).ToList();
            wait.Until(drv => WaitForOneOfBys(drv, bys), token);
        }

        bool WaitForOneOfBys(IWebDriver drv, IEnumerable<By> bys)
        {
            foreach (var by in bys)
            {
                try
                {
                    var el = drv.FindElement(by);
                    if (el.Displayed)
                    {
                        Console.WriteLine($"Match: {by.Criteria}");
                        return true;
                    }
                }
                catch { }
            }
            Console.WriteLine("No match!");
            return false;
        }

        void AddMemoryOptimizeOptions(FirefoxOptions options)
        {
            //options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-application-cache");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
        }

        FirefoxOptions CreateOptions(Models.Configs configs)
        {
            var options = new FirefoxOptions();

            // AddMemoryOptimizeOptions(options);

            if (!string.IsNullOrEmpty(configs.proxy))
            {
                var proxy = new Proxy
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false,
                    HttpProxy = configs.proxy,
                    SslProxy = configs.proxy
                };
                options.Proxy = proxy;
            }

            if (configs.headless)
            {
                options.AddArgument("--headless");
            }

            if (configs.ignoreCertError)
            {
                options.AddArgument("ignore-certificate-errors");
            }
            return options;
        }
        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        driver?.Close();
                    }
                    catch { }
                    try
                    {
                        driver?.Quit();
                    }
                    catch { }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods

        #endregion
    }
}
