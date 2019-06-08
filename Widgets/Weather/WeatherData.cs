using System;
using System.Collections.Generic;

namespace Gadget.Widgets.Weather
{
    public sealed class TemperatureData
    {
        public string Day { get; set; }
        public string Weather { get; set; }
        public string TemperatureHigh { get; set; }
        public string TemperatureLow { get; set; }
    }

    public sealed class WeatherData
    {
        public string FeelsLike { get; set; }
        public string Barometer { get; set; }
        public string Dewpoint { get; set; }
        public string Humidity { get; set; }
        public string Visibility { get; set; }
        public DateTime SunRise { get; set; }
        public DateTime SunSet { get; set; }
        public List<TemperatureData> TemperatureData { get; set; } = new List<TemperatureData>();
        public string ForecastLink { get; set; }
    }
}
