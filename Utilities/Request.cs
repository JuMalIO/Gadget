using System.Net;

namespace Gadget.Utilities
{
    public class Request
    {
        public static string GetHtml(string url)
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadString(url);
            }
        }
    }
}
