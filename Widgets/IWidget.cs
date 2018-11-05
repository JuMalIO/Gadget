using System.Drawing;

namespace Gadget.Widgets
{
	public interface IWidget
	{
        string Id { get; set; }
        string Name { get; set; }
        int Position { get; set; }
        bool IsVisible { get; set; }
        
        void Draw(Graphics graphics, int width, int height);
        void Update();
        int GetHeight();
        bool ShowProperties();
    }
}
