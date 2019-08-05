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

        public static async void LoadYahooStockPrices()
        {
            List<string> stockSymbols = new List<string>();



            // IEnumerable<IEnumerable<string>> listOfLists = SplitList(stockSymbols, 10);

            // split symbol list on chanks

            //foreach (IEnumerable<string> stockSymbolsChunk in listOfLists)
            //{
                
            //}
        }

        private static async Task ProcessYahooStockSymbols(IWebBrowser webBrowser, IEnumerable<string> stockSymbolsChunk)
        {
            foreach(string stockSymbol in stockSymbolsChunk)
            {
                string url = $"https://query1.finance.yahoo.com/v7/finance/download/{stockSymbol}?period1=1532576069&period2=1564112069&interval=1d&events=history&crumb=rY/Buy3dFOs";
                string webPageSource = await LoadPageAsync(webBrowser, url);
            }
            
        }

        private static IEnumerable<IEnumerable<T>> SplitList<T>(IEnumerable<T> source, int listQnt)
        {
            int listSize = source.Count();
            double chunkSizeDbl = listSize / listQnt;
            int chunkSize = (int)Math.Round(chunkSizeDbl, 0);

            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }

        public static async Task<string> LoadPageAsync(IWebBrowser browser, string url)
        {

            string resultHtml = string.Empty;

            if (string.IsNullOrEmpty(url)) return resultHtml;

            browser.FrameLoadEnd += WebBrowserFrameLoadEnded;

            //TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            //EventHandler<LoadingStateChangedEventArgs> handler = null;

            //handler = async (sender, args) =>
            //{
            //    if (!args.IsLoading)
            //    {
            //        browser.LoadingStateChanged -= handler;
            //        tcs.TrySetResult(true);
            //        resultHtml = await browser.GetMainFrame().GetSourceAsync();                    
            //    }
            //};

            //browser.LoadingStateChanged += handler;

            try
            {
                browser.Load(url);
                while(!browser.IsLoading)
                {
                    await Task.Delay(300);
                }
                resultHtml = await browser.GetMainFrame().GetSourceAsync();
                browser.
            } catch (Exception e)
            {
                System.IO.File.WriteAllText("c:\\temp\\SecResWPF.txt", e.ToString());
            }
            

            return resultHtml;
            
        }

        private static async void WebBrowserFrameLoadEnded(object sender, FrameLoadEndEventArgs e)
        {
            if(e.Frame.IsMain)
            {
                string pageSource = await (sender as IWebBrowser).GetMainFrame().GetSourceAsync();
            }
        }

        public static async Task<bool> DownloadPageAsync(IWebBrowser browser, string url)
        {

            bool resultSuccess = false;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<LoadingStateChangedEventArgs> handler = null;

            handler = async (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                    
                }
            };

            browser.LoadingStateChanged += handler;

            browser.DownloadHandler = new CefDownloadHandler();

            browser.Load(url);

            return resultSuccess;

        }

    }
}
