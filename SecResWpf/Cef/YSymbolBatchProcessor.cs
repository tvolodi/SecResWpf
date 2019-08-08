using AngleSharp;
using AngleSharp.Dom;
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResWpf.Cef
{
    public class YSymbolBatchProcessor
    {
        public static int TimeEnd { get; } = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        private List<ChromiumWebBrowser> browserRegistry;

        private ConcurrentDictionary<ChromiumWebBrowser, bool> uninitializedBrowsers;

        private ConcurrentDictionary<ChromiumWebBrowser, bool> uninitializedCookies;

        private Dictionary<IWebBrowser, List<string>> batchesByBrowsers;

        private int batchQnt = 2;

        private string initialSymbol = string.Empty;

        public Dictionary<IWebBrowser, string> CrumbsDict { get; set; }

        // private List<string> symbolsBatch;

        // private List<string> remainingSymbols;

        public YSymbolBatchProcessor(List<string> symbolsBatch)
        {

            CrumbsDict = new Dictionary<IWebBrowser, string>();

            // Wait while browsers will ititialize
            uninitializedBrowsers = new ConcurrentDictionary<ChromiumWebBrowser, bool>();
            uninitializedCookies = new ConcurrentDictionary<ChromiumWebBrowser, bool>();

            batchesByBrowsers = new Dictionary<IWebBrowser, List<string>>();

            browserRegistry = new List<ChromiumWebBrowser>();
            for (int cnt = 0; cnt < batchQnt; cnt++)
            {
                ChromiumWebBrowser webBrowser = new ChromiumWebBrowser("", null, new RequestContext());
                webBrowser.BrowserInitialized += BrowserInitializedHandler;
                uninitializedBrowsers[webBrowser] = false;
                uninitializedCookies[webBrowser] = false;
                browserRegistry.Add(webBrowser);
            }
        }

        public async Task ProcessSymbolsBatch(List<string> symbolsBatch)
        {
            // Wait while browsers are inintializing
            while (uninitializedBrowsers.Count(p => p.Value == false) > 0)
            {
                await Task.Delay(300);
            }

            var batchList = splitList<string>(symbolsBatch, batchQnt);

            for(int cnt = 0; cnt<browserRegistry.Count; cnt++)
            {
                batchesByBrowsers[browserRegistry[cnt]] = batchList[cnt];
                string symbol = batchList[cnt][0];
                string currUrl = $"https://finance.yahoo.com/quote/{symbol}/history?p={symbol}";

                browserRegistry[cnt].FrameLoadEnd += BrowserFrameLoadEnd;

                browserRegistry[cnt].Load(currUrl);
            }

            while(batchesByBrowsers.Count(kv => kv.Value.Count > 0) > 0)
            {
                await Task.Delay(100);
            }

            string resultStr = "Complete";
        }

        private async void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if(e.Frame.IsMain)
            {
                var browser = (sender as IWebBrowser);
                var mainFrame = browser.GetMainFrame();

                try
                {
                    string htmlSource = await mainFrame.GetSourceAsync();

                    string initialSymbol = batchesByBrowsers[(sender as IWebBrowser)][0];

                    // Get url for history prices

                    var context = BrowsingContext.New(Configuration.Default);

                    var htmlDocument = await context.OpenAsync(req => req.Content(htmlSource));

                    var elements = htmlDocument.All.Where(m => m.LocalName == "a"
                                                                && m.GetAttribute("download") == $"{initialSymbol}.csv").ToList();
                    var downloadElement = elements[0];
                    string downloadUrl = downloadElement.GetAttribute("href");
                    string[] urlElements = downloadUrl.Split('&');
                    string crumbElement = urlElements[urlElements.Length - 1];

                    CrumbsDict[sender as ChromiumWebBrowser] = crumbElement;


                    Int32 timeEnd = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    string symbol = batchesByBrowsers[(sender as ChromiumWebBrowser)][0];

                    // Build download url
                    string url = $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1=0&period2={timeEnd}&interval=1d&events=history&{crumbElement}";
                    DownloadPageAsync(url, symbol, (sender as ChromiumWebBrowser), this);

                    // initCookiesObservable.RegisterInitFinish(crumbElement);
                }
                catch (Exception e1)
                {
                    File.WriteAllText("c:\\temp\\pageLoad.txt", e1.ToString());
                    throw e1;
                }
                
            }
        }

        private void BrowserInitializedHandler(object sender, EventArgs e)
        {
            uninitializedBrowsers[(sender as ChromiumWebBrowser)] = true;
            (sender as ChromiumWebBrowser).BrowserInitialized -= BrowserInitializedHandler;
        }

        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            //if (e.IsLoading == false)
            //{                
            //    (sender as IWebBrowser).LoadingStateChanged -= BrowserLoadingStateChanged;
            //}
        }

        public static bool DownloadPageAsync(string url, string symbol, IWebBrowser browser, YSymbolBatchProcessor ySymbolBatchProcessor)
        {

            bool resultSuccess = false;

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<LoadingStateChangedEventArgs> handler = null;

            handler = (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            browser.LoadingStateChanged += handler;

            browser.DownloadHandler = new CefDownloadHandler(symbol, ySymbolBatchProcessor.batchesByBrowsers[browser], ySymbolBatchProcessor.CrumbsDict, ySymbolBatchProcessor);

            browser.Load(url);

            return resultSuccess;

        }

        public static List<List<T>> splitList<T>(List<T> inList, int nSize = 10)
        {

            var list = new List<List<T>>();

            //for (int i = 0; i < inList.Count; i += nSize)
            //{
            //    list.Add(inList.GetRange(i, Math.Min(nSize, inList.Count - i)));
            //}

            int width = (int) inList.Count / nSize + 1;

            for (int i = 0; i < nSize; i++)
            {
                List<T> subList;
                subList = inList.Skip(i * width).Take(width).ToList();
                list.Add(subList);
            }

            return list;
        }
    }
}
