using System.Drawing;

namespace Gadget.Widgets.RSS
{
    public sealed class RSSData
    {
        public string Link { get; set; }
        public Image Picture { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Date { get; set; }
        public bool IsNew { get; set; }
    }
}
