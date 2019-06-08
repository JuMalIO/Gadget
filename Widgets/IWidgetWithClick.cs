using System.Drawing;

namespace Gadget.Widgets
{
	public interface IWidgetWithClick
    {
        bool IsClickable { get; set; }
        ClickType ClickType { get; set; }
        string ClickParameter { get; set; }

        void Click(Point mouseLocation, int startFromHeight);
    }
}
