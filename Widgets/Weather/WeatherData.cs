using System;
using System.Collections.Generic;
using System.Drawing;

namespace Gadget.Widgets.Weather
{
    public sealed class WeatherData
    {
        public string CurrentTemperature { get; set; }
        public string CurrentWind { get; set; }
        public string CurrentWeather { get; set; }
        public string CurrentTemperatureFeelsLike { get; set; }
        public string CurrentBarometer { get; set; }
        public string CurrentTemperatureDewpoint { get; set; }
        public string CurrentHumidity { get; set; }
        public string CurrentVisibility { get; set; }
        public string CurrentSunRise { get; set; }
        public string CurrentSunSet { get; set; }
        public DateTime CurrentSunRiseDateTime { get; set; }
        public DateTime CurrentSunSetDateTime { get; set; }
        public bool IsDayNow { get; set; }
        public Image TodayWeatherImage { get; set; } = new Bitmap(140, 90);
        public List<Tuple<string, string, string, string>> DayForcast { get; set; } = new List<Tuple<string, string, string, string>>();
    }
}
