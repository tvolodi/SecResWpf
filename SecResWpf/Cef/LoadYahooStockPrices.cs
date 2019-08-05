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
    public class LoadYahooStockPrices
    {

        public static ConcurrentDictionary<string, bool> symbolPageLoadedStatuses = new ConcurrentDictionary<string, bool>();

        public static async Task LoadAsync()
        {

            string[] testSymbolArr = new string[] { "IBM", "AAPL", "F" };

            // Load some finance.yahoo.com page to emulate user activity
            using (RequestContext requestContext = new RequestContext())
            using (ChromiumWebBrowser browser = new ChromiumWebBrowser("", null, requestContext))
            {
                browser.LoadingStateChanged += BrowserLoadingStateChanged;

                while(!browser.IsBrowserInitialized)
                {
                    await Task.Delay(100);
                }

                string pageSource = await CefTools.LoadPageAsync(browser, "https://finance.yahoo.com/quote/GOOG/history?p=GOOG");

                foreach (string stockSymbol in testSymbolArr)
                {
                    string url = $"https://query1.finance.yahoo.com/v7/finance/download/{stockSymbol}?period1=1532833569&period2=1564369569&interval=1d&events=history&crumb=rY/Buy3dFOs";
                    await CefTools.DownloadPageAsync(browser, url);

                }
            }            
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if(e.IsLoading == false)
            {
                
            }


        }
    }
}
