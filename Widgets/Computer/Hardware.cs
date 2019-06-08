using Gadget.Config;
using Gadget.Properties;
using OpenHardwareMonitor.Hardware;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Widgets.Computer
{
	public class Hardware : IWidget, IWidgetWithText, IWidgetWithClick
	{
        #region public properties IWidget

        public string Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public bool IsVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithText

        public Color Color { get; set; } = Color.White;
        public string FontName { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 10;

        #endregion

        #region public properties IWidgetWithClick

        public bool IsClickable { get; set; } = true;
        public ClickType ClickType { get; set; } = ClickType.Process;
        public string ClickParameter { get; set; } = "taskmgr";

        #endregion

        private Font _font;
		private Brush _brush;
		private IHardware _hardware;
		private Image _icon;

		public void Initiate(IHardware hardware)
		{
			_hardware = hardware;
			_icon = GetHardwareIcon(hardware.HardwareType);
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			var iconSize = _icon.Width;
			graphics.DrawImageUnscaledAndClipped(_icon, new Rectangle(5, height + (int)((_font.Height - iconSize) / 2.0), iconSize, iconSize));
			iconSize += 5;
			graphics.DrawString(Name, _font, _brush, 5 + iconSize, height);
		}

		public void Update()
		{
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			Gadget.ProcessManager.Process(ClickType, ClickParameter);
		}

		public int GetHeight()
		{
			return _font.Height + 5;
		}

		public bool ShowProperties()
        {
            var propertiesForm = new PropertiesForm(this);
			if (propertiesForm.ShowDialog() == DialogResult.OK)
            {
				Initiate(_hardware);

                return true;
            }
			return false;
		}

		private static Image GetHardwareIcon(HardwareType hardwareType)
		{
			switch (hardwareType)
			{
				case HardwareType.CPU:
                    return Resources.cpu;
				case HardwareType.GpuNvidia:
                    return Resources.nvidia;
				case HardwareType.GpuAti:
                    return Resources.ati;
				case HardwareType.HDD:
                    return Resources.hdd;
				case HardwareType.Heatmaster:
                    return Resources.bigng;
				case HardwareType.Mainboard:
                    return Resources.mainboard;
				case HardwareType.SuperIO:
                    return Resources.chip;
				case HardwareType.TBalancer:
                    return Resources.bigng;
				case HardwareType.RAM:
                    return Resources.ram;
				default:
                    return new Bitmap(1, 1);
			}
		}
	}
}
