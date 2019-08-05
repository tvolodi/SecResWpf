using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResDataLoaderPlatf
{
    class Program
    {
        static void Main(string[] args)
        {
            string importDataType = args[0];
            switch (importDataType)
            {
                case "YahooStockPrices":
                    ImportYahooStockPrices();
                    break;

                default:
                    break;
            }
        }

        private static void ImportYahooStockPrices()
        {

            var settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CachePath = "cache";
            try
            {
                if (IsInitialized == false)
                {
                    CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                    IsInitialized = true;
                }
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText("app_log.txt", e.ToString());
                System.IO.File.AppendAllText("app_log.txt", "\n");
                throw e;
            }
        }
    }
}
