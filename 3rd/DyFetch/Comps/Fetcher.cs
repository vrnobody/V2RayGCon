using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace DyFetch.Comps
{
    internal class Fetcher : IDisposable
    {
        readonly WebDriver driver;
        private bool disposedValue;

        public Fetcher(Models.Configs configs)
        {
            driver = CreateDriver(configs);
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
                driver.Manage().Timeouts().PageLoad = span;
                driver.Navigate().GoToUrl(url);
                if (csses != null && csses.Count > 0)
                {
                    var cts = new CancellationTokenSource(span);
                    var token = cts.Token;
                    token.Register(() => cts.Dispose());
                    WaitForOneOfCsses(csses, w, token);
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
        WebDriver CreateDriver(Models.Configs configs)
        {
            var dir = configs.driverDir;
            var isCustomDrv = !string.IsNullOrEmpty(dir);
            WebDriver drv;
            if (configs.useChrome)
            {
                Console.WriteLine("using chrome driver");
                var options = CreateChromeOptions(configs);
                drv = isCustomDrv ? new ChromeDriver(dir, options) : new ChromeDriver(options);
            }
            else
            {
                Console.WriteLine("using firefox driver");
                var options = CreateFirefoxOptions(configs);
                drv = isCustomDrv ? new FirefoxDriver(dir, options) : new FirefoxDriver(options);
            }
            return drv;
        }

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

        ChromeOptions CreateChromeOptions(Models.Configs configs)
        {
            var options = new ChromeOptions();

            //options.AddArgument("--window-size=1920,1080");
            options.AddArgument("start-maximized");
            options.AddArgument("disable-infobars");

            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-application-cache");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            if (!string.IsNullOrEmpty(configs.proxy))
            {
                var proxy = new Proxy
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false,
                    HttpProxy = configs.proxy,
                    SslProxy = configs.proxy,
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

        FirefoxOptions CreateFirefoxOptions(Models.Configs configs)
        {
            var options = new FirefoxOptions();

            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-application-cache");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");

            if (!string.IsNullOrEmpty(configs.proxy))
            {
                var proxy = new Proxy
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false,
                    HttpProxy = configs.proxy,
                    SslProxy = configs.proxy,
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
