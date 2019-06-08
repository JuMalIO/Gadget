using System.Drawing;

namespace Gadget.Widgets
{
	public interface IWidgetWithHover
    {
        bool IsHoverable { get; set; }

        void Hover(Point applicationLocation, Point mouseLocation, int startFromHeight);
    }
}
