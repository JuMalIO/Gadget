using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace Gadget.Utilities
{
    public class Web
    {
        public static string GetHtml(string url)
        {
            using (var webClient = new WebClient())
            {
                var uriBuilder = new UriBuilder(url);
                return webClient.DownloadString(uriBuilder.Uri);
            }
        }

        public static Image GetImage(string url)
        {
            using (var client = new WebClient())
            {
                var uriBuilder = new UriBuilder(url);
                var bytes = client.DownloadData(uriBuilder.Uri);
                using (var memoryStream = new MemoryStream(bytes))
                {
                    return Image.FromStream(memoryStream);
                }
            }
        }
    }
}
