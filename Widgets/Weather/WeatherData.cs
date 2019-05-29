using System;
using System.Collections.Generic;
using System.Drawing;

namespace Gadget.Widgets.Weather
{
    public sealed class WeatherData
    {
        public string Temperature { get; set; }
        public string Wind { get; set; }
        public string Weather { get; set; }
        public string TemperatureFeelsLike { get; set; }
        public string Barometer { get; set; }
        public string TemperatureDewpoint { get; set; }
        public string Humidity { get; set; }
        public string Visibility { get; set; }
        public string SunRise { get; set; }
        public string SunSet { get; set; }
        public DateTime SunRiseDateTime { get; set; }
        public DateTime SunSetDateTime { get; set; }
        public bool IsDayNow { get; set; }
        public Image TodayWeatherImage { get; set; } = new Bitmap(140, 90);
        public List<Tuple<string, string, string, string>> DayForcast { get; set; } = new List<Tuple<string, string, string, string>>();
    }
}
