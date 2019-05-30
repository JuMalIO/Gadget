using Gadget.Utilities;
using Gadget.Widgets;
using Gadget.Widgets.Computer;
using Gadget.Widgets.Currency;
using Gadget.Widgets.Date;
using Gadget.Widgets.Graph;
using Gadget.Widgets.RSS;
using Gadget.Widgets.Time;
using Gadget.Widgets.Weather;
using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Config
{
    public class Config
    {
        #region Other

        public int TextRenderingHint { get; set; } = 0;
        public byte BackgroundOpacity { get; set; } = 100;
        public Color BackgroundColor { get; set; } = Color.Black;
        public bool Blur { get; set; } = true;
        public bool[] BackgroundBorder { get; set; } = new bool[] { false, false, false, true };
        public Point Location { get; set; } = new Point(Screen.PrimaryScreen.Bounds.Width - 225, 0);
        public Size Size { get; set; } = new Size(225, Screen.PrimaryScreen.Bounds.Height - 40);
        public bool LockPositionAndSize { get; set; } = true;
        public bool AlwaysOnTop { get; set; } = false;
        public byte Opacity { get; set; } = 255;
        public int UpdateInterval { get; set; } = 1;

        #endregion

        #region Widgets

        public List<Hardware> Hardwares { get; set; } = new List<Hardware>();
        public List<Sensor> Sensors { get; set; } = new List<Sensor>();
        public List<Graph> Graphs { get; set; } = new List<Graph>();
        public Date Date { get; set; } = new Date();
        public Time Time { get; set; } = new Time();
        public Weather Weather { get; set; } = new Weather();
        public Currency Currency { get; set; } = new Currency();
        public RSS RSS { get; set; } = new RSS();

        #endregion

        public const string FILE = "config.ini";

        public static Config Load()
        {
            return Json.DeserializeFile<Config>(FILE);
        }

        public static void Save(Config config)
        {
            Json.SerializeFile(FILE, config);
        }

        public List<IWidget> GetWidgets(Computer computer)
        {
            var widgets = new List<IWidget>();

            int pos = 0;

            foreach (var hardware in computer.Hardware.OrderBy(x => x.HardwareType))
            {
                var id = hardware.Identifier.ToString();
                var widgetHardware = Hardwares.FirstOrDefault(x => x.Id == id);
                if (widgetHardware == null)
                {
                    widgetHardware = new Hardware
                    {
                        Id = id,
                        Name = hardware.Name,
                        Position = pos
                    };
                    Hardwares.Add(widgetHardware);
                }

                pos++;

                widgetHardware.Initiate(hardware);

                widgets.Add(widgetHardware);

                foreach (var sensor in hardware.Sensors)
                {
                    id = sensor.Identifier.ToString();
                    var widgetSensor = Sensors.FirstOrDefault(x => x.Id == id);
                    if (widgetSensor == null)
                    {
                        widgetSensor = new Sensor
                        {
                            Id = id,
                            Name = sensor.Name,
                            Position = pos,
                            Color = Sensor.GetColor(pos)
                        };
                        Sensors.Add(widgetSensor);
                    }

                    pos++;

                    widgetSensor.Initiate(sensor);

                    widgets.Add(widgetSensor);
                }
                
                if (hardware.HardwareType == HardwareType.RAM || hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    var graph = Graph.GetName(hardware.HardwareType);
                    if (graph != null)
                    {
                        var widgetGraph = Graphs.FirstOrDefault(x => x.Id == graph);
                        if (widgetGraph == null)
                        {
                            widgetGraph = new Graph
                            {
                                Id = graph,
                                Name = graph,
                                Position = pos
                            };
                            Graphs.Add(widgetGraph);
                        }

                        pos++;

                        widgetGraph.Initiate(hardware.HardwareType, Size.Width, widgets);

                        widgets.Add(widgetGraph);
                    }
                }
            }

            Date.Initiate();
            widgets.Add(Date);

            Time.Initiate();
            widgets.Add(Time);

            Weather.Initiate();
            widgets.Add(Weather);

            Currency.Initiate();
            widgets.Add(Currency);

            RSS.Initiate();
            widgets.Add(RSS);

            widgets.SortByPosition();

            return widgets;
        }
    }
}
