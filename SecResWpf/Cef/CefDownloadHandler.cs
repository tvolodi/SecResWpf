using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecResWpf.Cef
{
    public class CefDownloadHandler : IDownloadHandler
    {
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        private string symbol = string.Empty;

        private List<string> remainingSymbols;

        private YSymbolBatchProcessor ySymbolBatchProcessor;

        public Dictionary<IWebBrowser, string> CrumbsDict { get; set; }

        public CefDownloadHandler(string symbol, List<string> remainingSymbols, Dictionary<IWebBrowser, string> CrumbsDict, YSymbolBatchProcessor ySymbolBatchProcessor)
        {
            this.symbol = symbol;
            this.remainingSymbols = remainingSymbols;
            this.CrumbsDict = CrumbsDict;
            this.ySymbolBatchProcessor = ySymbolBatchProcessor;
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if(downloadItem.SuggestedFileName == string.Empty)
            {
                downloadItem.SuggestedFileName = $"{symbol}.csv";
            }

            OnBeforeDownloadFired?.Invoke(this, downloadItem);

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(downloadItem.SuggestedFileName, showDialog: false);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            downloadItem.SuggestedFileName = $"{symbol}.csv";

            OnDownloadUpdatedFired?.Invoke(this, downloadItem);

            if(downloadItem.IsComplete == true && downloadItem.IsInProgress == false)
            {
                remainingSymbols.Remove(symbol);

                if(remainingSymbols.Count > 0)
                {
                    symbol = remainingSymbols[0];

                    // Build download url
                    int timeEnd = YSymbolBatchProcessor.TimeEnd;
                    string crumbElement = CrumbsDict[chromiumWebBrowser];
                    string url = $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1=0&period2={timeEnd}&interval=1d&events=history&{crumbElement}";
                    YSymbolBatchProcessor.DownloadPageAsync(url, symbol, chromiumWebBrowser, ySymbolBatchProcessor);
                }
            }
        }
    }
}
