using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResWpf.Cef
{
    public class CefTools
    {
        public static ConcurrentDictionary<string, string> q = new ConcurrentDictionary<string, string>();

        public async static void Init()
        {
            var settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CachePath = "cache";
            try
            {
                if(CefSharp.Cef.IsInitialized == false)
                {
                    
                    CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
                    
                    while(!CefSharp.Cef.IsInitialized)
                    {
                        await Task.Delay(100);
                    }
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
            if(CefSharp.Cef.IsInitialized == true)
            {
                CefSharp.Cef.Shutdown();
            }
        }
    }
}
