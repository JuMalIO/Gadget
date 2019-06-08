using System.Drawing;

namespace Gadget.Widgets.Rss
{
    public sealed class RssData
    {
        public string Link { get; set; }
        public Image Picture { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Date { get; set; }
        public bool IsNew { get; set; }
    }
}
