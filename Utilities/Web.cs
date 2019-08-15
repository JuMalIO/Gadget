using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Gadget.Utilities
{
    public static class Web
    {
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0";

        public static string GetHtml(string url)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", UserAgent);
                webClient.Encoding = System.Text.Encoding.UTF8;

                var uriBuilder = new UriBuilder(url);
                return webClient.DownloadString(uriBuilder.Uri);
            }
        }

        public static Image GetImage(string url)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", UserAgent);
                webClient.Encoding = System.Text.Encoding.UTF8;

                var uriBuilder = new UriBuilder(url);
                var bytes = webClient.DownloadData(uriBuilder.Uri);
                using (var memoryStream = new MemoryStream(bytes))
                {
                    return Image.FromStream(memoryStream);
                }
            }
        }
    }
}
