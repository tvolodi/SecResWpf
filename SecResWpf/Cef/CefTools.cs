using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResWpf.Cef
{
    public class CefTools
    {
        public static bool IsInitialized = false;

        public static void Init()
        {
            var settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CachePath = "cache";
            try
            {
                if(IsInitialized == false)
                {
                    CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                    IsInitialized = true;
                }
            } catch (Exception e)
            {
                System.IO.File.AppendAllText("app_log.txt", e.ToString());
                System.IO.File.AppendAllText("app_log.txt", "\n");
                throw e;
            }           
        }

        public static void Down()
        {
            if(IsInitialized == true)
            {
                CefSharp.Cef.Shutdown();
                IsInitialized = false;
            }
            
        }

        public static void LoadYStockPrices(string symbol)
        {
            using (RequestContext requestContext = new RequestContext())
            using (ChromiumWebBrowser browser = new ChromiumWebBrowser("", null, requestContext))
            {

            }
        }

    }
}
