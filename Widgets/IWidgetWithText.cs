using System.Drawing;

namespace Gadget.Widgets
{
    public interface IWidgetWithText
    {
        Color Color { get; set; }
        string FontName { get; set; }
        int FontSize { get; set; }
    }
}
