using Gadget.Config;
using Gadget.Properties;
using Gadget.Widgets.Computer;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gadget.Widgets.Graph
{
	public class Graph : IWidget, IWidgetWithClick
	{
        #region public properties IWidget

        public string Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public bool IsVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithClick

        public bool IsClickable { get; set; } = true;
        public ClickType ClickType { get; set; } = ClickType.ScreenOff;
        public string ClickParameter { get; set; } = "";

        #endregion

        private Image _borderTopLeft, _borderTop, _borderTopRight, _borderRight, _borderBottomRight, _borderBottom, _borderBottomLeft, _borderLeft;
		private Image _graph;
		private List<Sensor> _graphSensorList;
		private byte[] _graphPixels;
		private Bitmap _graphBitmap;
		private float[] _graphLastValue;
		private byte[] _graphPixelLastVerticalLine;

		public void Initiate(HardwareType hardwareType, int width, List<IWidget> widgets)
		{
			_graphSensorList = GetWidgets(hardwareType, ref widgets);

			_borderTopLeft = Resources.top_left;
			_borderTop = Resources.top;
			_borderTopRight = Resources.top_right;
			_borderRight = Resources.right;
			_borderBottomRight = Resources.bottom_right;
			_borderBottom = Resources.bottom;
			_borderBottomLeft = Resources.bottom_left;
			_borderLeft = Resources.left;

			_graph = new Bitmap(width - 10 - _borderTopLeft.Width * 2, _borderLeft.Height - _borderTopLeft.Height - _borderBottomRight.Height);
			using (Graphics g = Graphics.FromImage(_graph))
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(40, Color.Black)))
				{
					g.FillRectangle(brush, 0, 0, _graph.Width, _graph.Height);
				}
				using (Brush line = new SolidBrush(Color.FromArgb(51, 255, 255, 255)))
				{
					for (int i = 10; i < _graph.Width; i += 10)
						g.DrawLine(new Pen(line, 1), new Point(i, 0), new Point(i, _graph.Height));
					for (int i = 10; i < _graph.Height; i += 10)
						g.DrawLine(new Pen(line, 1), new Point(0, i), new Point(_graph.Width, i));
				}
				using (Brush border2 = new SolidBrush(Color.FromArgb(127, 0, 0, 0)))
				{
					g.DrawRectangle(new Pen(border2, 1), new Rectangle(0, 0, _graph.Width - 1, _graph.Height - 1));
				}
			}
			_graphBitmap = new Bitmap(_graph.Width - 2, _graph.Height - 2);
			_graphPixels = new byte[_graphBitmap.Width * _graphBitmap.Height * Image.GetPixelFormatSize(_graphBitmap.PixelFormat) / 8];
			_graphLastValue = new float[_graphSensorList.Count];
			_graphPixelLastVerticalLine = new byte[_graphBitmap.Height];
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			int width2 = width - 10 - _borderTopLeft.Width * 2;

			graphics.DrawImageUnscaledAndClipped(_borderTopLeft, new Rectangle(5, height, _borderTopLeft.Width, _borderTopLeft.Height));
			graphics.DrawImageUnscaledAndClipped(_borderTop, new Rectangle(5 + _borderTopLeft.Width, height, width2, _borderTopLeft.Height));
			graphics.DrawImageUnscaledAndClipped(_borderTopRight, new Rectangle(5 + _borderTopLeft.Width + width2, height, _borderTopRight.Width, _borderTopLeft.Height));

			height += _borderTopLeft.Height;

			graphics.DrawImageUnscaledAndClipped(_borderLeft, new Rectangle(5, height, _borderTopLeft.Width, _graph.Height));
			graphics.DrawImageUnscaledAndClipped(_borderRight, new Rectangle(5 + _borderTopLeft.Width + width2, height, _borderTopRight.Width, _graph.Height));

			graphics.DrawImageUnscaledAndClipped(_graph, new Rectangle(5 + _borderTopLeft.Width, height, width2, _graph.Height));
			graphics.DrawImageUnscaledAndClipped(_graphBitmap, new Rectangle(6 + _borderTopLeft.Width, height + 1, width2 - 2, _graph.Height - 2));

			height += _graph.Height;

			graphics.DrawImageUnscaledAndClipped(_borderBottomLeft, new Rectangle(5, height, _borderTopLeft.Width, _borderBottomLeft.Height));
			graphics.DrawImageUnscaledAndClipped(_borderBottom, new Rectangle(5 + _borderTopLeft.Width, height, width2, _borderBottomLeft.Height));
			graphics.DrawImageUnscaledAndClipped(_borderBottomRight, new Rectangle(5 + _borderTopLeft.Width + width2, height, _borderTopRight.Width, _borderBottomLeft.Height));
		}

		public void Update()
		{
			UpdateGraphData(this);
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			Gadget.ProcessManager.Process(ClickType, ClickParameter);
		}

		public int GetHeight()
		{
			return _graph.Height + _borderTopLeft.Height + _borderBottomLeft.Height + 5;
		}

		public bool ShowProperties()
        {
            var propertiesForm = new PropertiesForm(this);
			if (propertiesForm.ShowDialog() == DialogResult.OK)
			{
                //Initiate();

                return true;
            }
			return false;
		}

		private static void UpdateGraphData(Graph graph)
		{
			IntPtr iptr = IntPtr.Zero;
			BitmapData bitmapData = null;

			int width = graph._graphBitmap.Width;
			int height = graph._graphBitmap.Height;
			Rectangle rect = new Rectangle(0, 0, width, height);
			int depth = Image.GetPixelFormatSize(graph._graphBitmap.PixelFormat);

			bitmapData = graph._graphBitmap.LockBits(rect, ImageLockMode.WriteOnly, graph._graphBitmap.PixelFormat);

			int step = depth / 8;
			iptr = bitmapData.Scan0;

			//pixels = new byte[width * height * step];
			//Marshal.Copy(iptr, pixels, 0, pixels.Length);

			int fullWidth = bitmapData.Width * step;
			int fullHeight = bitmapData.Height * step;
			width = (bitmapData.Width - 1) * step;

			for (int y = 0; y < height; y++)
			{
				int b = y * fullWidth;
				for (int x = 0; x < width; x++)
				{
					int i = b + x;
					graph._graphPixels[i] = graph._graphPixels[i + step];
				}
			}

			byte[] newGraphPixelLastVerticalLine = new byte[graph._graphPixelLastVerticalLine.Length];

			for (int k = 0; k < graph._graphSensorList.Count; k++)
			{
				Color color = graph._graphSensorList[k].Color;
                var value = graph._graphSensorList[k].GetSensor().Value.Value;
                double percentage100 = value / 100.0;
				double lastPercentage100 = graph._graphLastValue[k] / 100.0;
				double halfPercentage100 = (lastPercentage100 - percentage100) / 2.0;
				int heightFrom = (int)((double)height - (double)height * percentage100);
				int heightTo = (int)((double)height - (double)height * (percentage100 + halfPercentage100));
				if (heightFrom > heightTo)
				{
					int i = heightFrom;
					heightFrom = heightTo;
					heightTo = i;
				}

				int heightFrom2 = (int)((double)height - (double)height * lastPercentage100);
				int heightTo2 = (int)((double)height - (double)height * (lastPercentage100 - halfPercentage100));
				if (heightFrom2 < heightTo2)
				{
					int i = heightFrom2;
					heightFrom2 = heightTo2;
					heightTo2 = i;
				}

				for (int y = 0; y < height; y++)
				{
					int i = y * fullWidth + width;
					if (y >= heightFrom && y <= heightTo)
					{
						graph._graphPixels[i] = color.B;
						graph._graphPixels[i + 1] = color.G;
						graph._graphPixels[i + 2] = color.R;
						graph._graphPixels[i + 3] = color.A;
						newGraphPixelLastVerticalLine[y] = (byte)(k + 1);
					}
					else
					{
						if (k == 0)
						{
							graph._graphPixels[i] = 0;
							graph._graphPixels[i + 1] = 0;
							graph._graphPixels[i + 2] = 0;
							graph._graphPixels[i + 3] = 0;
							newGraphPixelLastVerticalLine[y] = 0;
						}

						if (y > heightTo2 && y < heightFrom2)
						{
							if ((int)graph._graphPixelLastVerticalLine[y] <= k)
							{
								i -= step;
								//if (k != 0 || graphPixels[i + 3] == 0)
								//{
								graph._graphPixels[i] = color.B;
								graph._graphPixels[i + 1] = color.G;
								graph._graphPixels[i + 2] = color.R;
								graph._graphPixels[i + 3] = color.A;
								//}
							}
						}

					}
				}

				graph._graphLastValue[k] = value;
			}

			graph._graphPixelLastVerticalLine = newGraphPixelLastVerticalLine;

			Marshal.Copy(graph._graphPixels, 0, iptr, graph._graphPixels.Length);

			graph._graphBitmap.UnlockBits(bitmapData);
		}

		private static List<Sensor> GetWidgets(HardwareType hardwareType, ref List<IWidget> widgetList)
		{
			List<Sensor> widgets = new List<Sensor>();
			foreach (var widget in widgetList)
			{
				if (widget is Sensor)
				{
					ISensor iSensor = ((Sensor)widget).GetSensor();
					if (iSensor.SensorType == SensorType.Load)
					{
                        if (hardwareType == HardwareType.CPU || hardwareType == HardwareType.RAM)
                        {
                            if ((iSensor.Hardware.HardwareType == HardwareType.CPU || iSensor.Hardware.HardwareType == HardwareType.RAM) && !iSensor.Name.Contains("Total"))
                            {
                                widgets.Add(((Sensor)widget));
                            }
                        }
                        else if (iSensor.Hardware.HardwareType == hardwareType)
						{
							widgets.Add(((Sensor)widget));
						}
					}
				}
			}
			return widgets;
		}

        public static string GetName(HardwareType hardwareType)
        {
            if (hardwareType == HardwareType.CPU || hardwareType == HardwareType.RAM)
                return "CPU & RAM Graph";
            if (hardwareType == HardwareType.GpuAti || hardwareType == HardwareType.GpuNvidia)
                return "GPU Graph";
            return null;
        }
	}
}
