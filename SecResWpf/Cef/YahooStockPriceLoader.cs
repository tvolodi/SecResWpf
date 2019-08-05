using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResWpf.Cef
{
    public class YahooStockPriceLoader
    {
        private ChromiumWebBrowser browser;
        private RequestContext requestContext;

        private List<string> symbolList = new List<string>();

        private bool isPageInitialized = false;

        private string symbol;

        public YahooStockPriceLoader(string symbol)
        {
            this.symbol = symbol;
            requestContext = new RequestContext();
            browser = new ChromiumWebBrowser(symbolList[0], null, requestContext);

            browser.LoadingStateChanged += BrowserLoadingStateChanged;
            browser.FrameLoadEnd += BrowserFrameLoadEnd;
        }

        private void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task LoadPrices()
        {
            
        }

        private void DisposeWebBrowser()
        {
            requestContext.Dispose();
            browser.Dispose();
        }


    }
}
