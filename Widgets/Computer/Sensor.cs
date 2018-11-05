using Gadget.Config;
using OpenHardwareMonitor.Hardware;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Widgets.Computer
{
	public class Sensor : IWidget, IWidgetWithText, IWidgetWithClick
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
        private ISensor _sensor;

		public void Initiate(ISensor sensor)
		{
			_sensor = sensor;
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			graphics.DrawString(Name, _font, _brush, 5, height);
			var formatted = GetFormated(_sensor);
			var textWidth = (int)graphics.MeasureString(formatted, _font).Width;
			graphics.DrawString(formatted, _font, _brush, width - textWidth - 5, height);
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
                Initiate(_sensor);

                return true;
            }
			return false;
		}

        public ISensor GetSensor()
        {
            return _sensor;
        }

        public static Color GetColor(int index)
        {
            var colors = new Color[] { Color.LightBlue, Color.Cyan, Color.Chartreuse, Color.Yellow, Color.Orange, Color.Pink, Color.HotPink, Color.OrangeRed, Color.MediumPurple, Color.DeepSkyBlue };
            return colors[index % colors.Length];
        }

        private static string GetFormated(ISensor sensor)
		{
			string formatted;
            if (sensor.Value.HasValue)
            {
                var format = "";
                switch (sensor.SensorType)
                {
                    case SensorType.Voltage:
                        format = "{0:F3} V";
                        break;
                    case SensorType.Clock:
                        format = "{0:F0} MHz";
                        break;
                    case SensorType.Temperature:
                        format = "{0:F1} °C";
                        break;
                    case SensorType.Fan:
                        format = "{0:F0} RPM";
                        break;
                    case SensorType.Flow:
                        format = "{0:F0} L/h";
                        break;
                    case SensorType.Power:
                        format = "{0:F1} W";
                        break;
                    case SensorType.Data:
                        format = "{0:F1} GB";
                        break;
                    case SensorType.Factor:
                        format = "{0:F3}";
                        break;
                    case SensorType.Load:
                        format = "{0:F0} %";
                        break;
                }
                formatted = string.Format(format, sensor.Value);
            }
            else
            {
                formatted = "-";
            }

			return formatted;
		}
	}
}
