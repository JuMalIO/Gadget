using Gadget.Config;
using Gadget.Extensions;
using Gadget.Properties;
using Gadget.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Widgets.Weather
{
	public class Weather : IWidget, IWidgetWithText, IWidgetWithClick, IWidgetWithHover, IWidgetWithInternet
	{
        #region public properties IWidget

        public string Id { get; set; } = "Weather";
        public string Name { get; set; } = "Weather";
        public int Position { get; set; } = -1;
        public bool IsVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithText

        public Color Color { get; set; } = Color.White;
        public string FontName { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 10;

        #endregion

        #region public properties IWidgetWithClick

        public bool IsClickable { get; set; } = true;
        public ClickType ClickType { get; set; } = ClickType.Internet;
        public string ClickParameter { get; set; } = "http://www.foreca.com/Lithuania/Vilnius?tenday";

        #endregion

        #region public properties IWidgetWithHover

        public bool IsHoverable { get; set; } = true;

        #endregion

        #region public properties IWidgetWithInternet

        public int UpdateInternetInterval { get; set; } = 60;

        #endregion

        #region public properties

        public string Url { get; set; } = "http://www.foreca.com/Lithuania/Vilnius";

        #endregion

        #region static readonly constants

        private static readonly string Celcius = "°C";

        private static readonly Dictionary<string, Image[]> WeatherImages = new Dictionary<string, Image[]>
        {
            {
                "Clear",
                new []
                {
                    Resources.clear_day,
                    Resources.clear_night
                }
            },
            {
                "Cloudy and light snow",
                new []
                {
                    Resources.cloudy_and_light_snow_day,
                    Resources.cloudy_and_light_snow_night
                }
            },
            {
                "Cloudy and showers",
                new []
                {
                    Resources.cloudy_and_showers_day,
                    Resources.cloudy_and_showers_night
                }
            },
            {
                "Cloudy",
                new []
                {
                    Resources.cloudy_day,
                    Resources.cloudy_night
                }
            },
            {
                "Cloudy, thunderstorms with rain",
                new []
                {
                    Resources.cloudy_thunderstorms_with_rain_day,
                    Resources.cloudy_thunderstorms_with_rain_night
                }
            },
            {
                "Mostly clear",
                new []
                {
                    Resources.mostly_clear_day,
                    Resources.mostly_clear_night
                }
            },
            {
                "Mostly cloudy",
                new []
                {
                    Resources.mostly_cloudy_day,
                    Resources.mostly_cloudy_night
                }
            },
            {
                "Overcast and light rain",
                new []
                {
                    Resources.overcast_and_light_rain_day,
                    Resources.overcast_and_light_rain_night
                }
            },
            {
                "Overcast and light snow",
                new []
                {
                    Resources.overcast_and_light_snow_day,
                    Resources.overcast_and_light_snow_night
                }
            },
            {
                "Overcast and light wet snow",
                new []
                {
                    Resources.overcast_and_light_wet_snow_day,
                    Resources.overcast_and_light_wet_snow_night
                }
            },
            {
                "Overcast and rain",
                new []
                {
                    Resources.overcast_and_rain_day,
                    Resources.overcast_and_rain_night
                }
            },
            {
                "Overcast and showers",
                new []
                {
                    Resources.overcast_and_showers_day,
                    Resources.overcast_and_showers_night
                }
            },
            {
                "Overcast",
                new []
                {
                    Resources.overcast_day,
                    Resources.overcast_night
                }
            },
            {
                "Partly cloudy and light rain",
                new []
                {
                    Resources.partly_cloudy_and_light_rain_day,
                    Resources.partly_cloudy_and_light_rain_night
                }
            },
            {
                "Partly cloudy and showers",
                new []
                {
                    Resources.partly_cloudy_and_showers_day,
                    Resources.partly_cloudy_and_showers_night
                }
            },
            {
                "Partly cloudy",
                new []
                {
                    Resources.partly_cloudy_day,
                    Resources.partly_cloudy_night
                }
            }
        };

        #endregion

        private int _updateInternetCount;
        private Font _font;
		private Brush _brush;
		private WeatherData _weatherData;
        private Image _weatherImage;
        private bool _isDayTime;

        public void Initiate()
		{
            _weatherImage = new Bitmap(140, 90);
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
        }

		public void UpdateInternet()
		{
			_updateInternetCount++;
			if (UpdateInternetInterval < _updateInternetCount || _weatherData == null || _weatherData.TemperatureData.Count == 0)
			{
				_weatherData = GetWeather(Url);
                _weatherImage = GetImage(_weatherData?.TemperatureData.FirstOrDefault()?.Weather, IsDayTime(_weatherData));
				_updateInternetCount = 0;
			}
            
            if (_isDayTime != IsDayTime(_weatherData))
            {
                _isDayTime = !_isDayTime;
                _weatherImage = GetImage(_weatherData?.TemperatureData.FirstOrDefault()?.Weather, _isDayTime);
            }
        }

		public void Draw(Graphics graphics, int width, int height)
		{
			if (_weatherData != null && _weatherData.TemperatureData.Count > 0)
			{
				graphics.DrawString(_weatherData.TemperatureData[0].Weather, _font, _brush, 5, height);
				string date = $"{_weatherData.TemperatureData[0].TemperatureLow} {_weatherData.TemperatureData[0].TemperatureHigh} {Celcius}";
				int textWidth = (int)graphics.MeasureString(date, _font).Width;
				graphics.DrawString(date, _font, _brush, width - textWidth - 5, height);
				height += _font.Height;
				int posX = (width - _weatherImage.Width) / 2;
				graphics.DrawImageUnscaledAndClipped(_weatherImage, new Rectangle(posX, height, _weatherImage.Width, _weatherImage.Height));
				height += _weatherImage.Height;
				graphics.DrawString($"Sun Rise/Set {_weatherData.SunRise:HH:mm}/{_weatherData.SunSet:HH:mm}", _font, _brush, 5, height);
			}
			else
			{
				graphics.DrawString("No weather update.", _font, _brush, 5, height);
				height += _font.Height + _weatherImage.Height;
			}
			height += _font.Height + 5;
		}

		public void Update()
		{
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			Gadget.ProcessManager.Process(ClickType, ClickParameter);
		}

		public void Hover(Point applicationLocation, Point mouseLocation, int startFromHeight)
		{
			if (_weatherData != null)
			{
                var toolTipWindow = new Gadget.ToolTip();
                toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
				{
					toolTipWindow.Hide();
					Gadget.ProcessManager.Process(ClickType, ClickParameter);
				};
				toolTipWindow.Show(new Point(applicationLocation.X + mouseLocation.X, applicationLocation.Y + mouseLocation.Y), _weatherData.ForecastImage);
			}
		}

		public int GetHeight()
		{
            return _weatherImage.Height + _font.Height * 2 + 5;
		}

		public bool ShowProperties()
        {
            var weatherUserControl = new WeatherUserControl(Url);
            var propertiesForm = new PropertiesForm(this, new [] { weatherUserControl });
			if (propertiesForm.ShowDialog() == DialogResult.OK)
			{
                Url = weatherUserControl.Url;

                Initiate();
                UpdateInternet();

                return true;
            }
			return false;
		}

        private static Image GetImage(string name, bool isDayTime = true)
        {
            if (WeatherImages.ContainsKey(name))
            {
                return WeatherImages[name][isDayTime ? 0 : 1];
            }
            return new Bitmap(140, 90);
		}

        private static bool IsDayTime(WeatherData weatherData)
        {
            var now = DateTime.Now;
            return now.TimeOfDay > weatherData?.SunRise.TimeOfDay && now.TimeOfDay < weatherData?.SunSet.TimeOfDay;
        }

		private static WeatherData GetWeather(string url)
		{
			try
			{
                var html = Web.GetHtml(url);

				html = html.CutAfterText("Current conditions");

				html = html.CutAfterText("Feels Like: <strong>");
				var feelsLike = html.CutBeforeText("<").Replace("&deg;", "");

                html = html.CutAfterText("Barometer:  <strong>");
				var barometer = html.CutBeforeText("<").Trim();

                html = html.CutAfterText("Dewpoint:   <strong>");
				var dewpoint = html.CutBeforeText("<").Replace("&deg;", "");

                html = html.CutAfterText("Humidity:   <strong>");
				var humidity = html.CutBeforeText("<").Trim();

                html = html.CutAfterText("Visibility: <strong>");
				var visibility = html.CutBeforeText("<").Trim();

                html = html.CutAfterText("Sun rise: <strong>");
				var sunRise = html.CutBeforeText("<").Trim().ParseTime();

				html = html.CutAfterText("Sun set:  <strong>");
				var sunSet = html.CutBeforeText("<").Trim().ParseTime();

                html = html.CutAfterText("3 day outlook");

                var temperatureData = new List<TemperatureData>();
                for (var i = 0; i < 3; i++)
                {
                    html = html.CutAfterText("title=\"");
                    var weather = html.CutBeforeText("\"").Trim();

                    html = html.CutAfterText("<strong>");
                    var day = html.CutBeforeText("<").Trim();

                    html = html.CutAfterText("Hi: <strong>");
                    var temperatureHigh = html.CutBeforeText("<").Replace("&deg;", "");

                    html = html.CutAfterText("Lo: <strong>");
                    var temperatureLow = html.CutBeforeText("<").Replace("&deg;", "");

                    temperatureData.Add(new TemperatureData
                    {
                        Day = day,
                        Weather = weather,
                        TemperatureHigh = temperatureHigh,
                        TemperatureLow = temperatureLow
                    });
                }

                html = html.CutAfterText("Detailed 5 day forecast");

                html = html.CutAfterText("<img src=\"");
                var forecastLink = url.CutAfterText("//").CutBeforeText("/") + html.CutBeforeText("\"");
                var forecastImage = Web.GetImage(forecastLink);

                return new WeatherData
                {
                    FeelsLike = feelsLike,
                    Barometer = barometer,
                    Dewpoint = dewpoint,
                    Humidity = humidity,
                    Visibility = visibility,
                    SunRise = sunRise,
                    SunSet = sunSet,
                    TemperatureData = temperatureData,
                    ForecastImage = forecastImage
                };
			}
			catch
            {
                return null;
            }
		}
	}
}
