using Gadget.Gadget;
using System.Drawing;

namespace Gadget.Widgets
{
	public interface IWidgetWithHover
    {
        bool IsHoverable { get; set; }

        void Hover(ToolTip toolTipWindow, Point ApplicationLocation, Point MouseLocation, int startFromHeight);
    }
}
