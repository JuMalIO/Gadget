using Gadget.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
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

        private int _updateInternetCount;
        private Font _font;
		private Brush _brush;
		private WeatherData _weatherData;

		public const string CELCIUS = " °C";

		public void Initiate()
		{
			_weatherData = new WeatherData();
			_weatherData.TodayWeatherImage = new Bitmap(140, 90);
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
        }

		public void UpdateInternet()
		{
			UpdateImage();

			_updateInternetCount++;
			if (UpdateInternetInterval < _updateInternetCount || _weatherData.DayForcast.Count == 0)
			{
				_weatherData = GetWeather(Url);
				_updateInternetCount = 0;
			}
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			if (_weatherData != null)
			{
				if (_weatherData.DayForcast.Count > 0)
				{
					graphics.DrawString(_weatherData.DayForcast[0].Item2, _font, _brush, 5, height);
					string date = _weatherData.DayForcast[0].Item3 + " " + _weatherData.DayForcast[0].Item4 + CELCIUS;
					int textWidth = (int)graphics.MeasureString(date, _font).Width;
					graphics.DrawString(date, _font, _brush, width - textWidth - 5, height);
					height = height + _font.Height;
					int posX = (width - _weatherData.TodayWeatherImage.Width) / 2;
					graphics.DrawImageUnscaledAndClipped(_weatherData.TodayWeatherImage, new Rectangle(posX, height, _weatherData.TodayWeatherImage.Width, _weatherData.TodayWeatherImage.Height));
					height = height + _weatherData.TodayWeatherImage.Height;
					graphics.DrawString($"Sun Rise/Set {_weatherData.SunRise}/{_weatherData.SunSet}", _font, _brush, 5, height);
				}
				else
				{
					graphics.DrawString("No weather update.", _font, _brush, 5, height);
					height = height + _font.Height;
					if (_weatherData.TodayWeatherImage != null)
						height = height + _weatherData.TodayWeatherImage.Height;
				}
				height = height + _font.Height + 5;
			}
		}

		public void Update()
		{
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			Gadget.ProcessManager.Process(ClickType, ClickParameter);
		}

		public void Hover(Gadget.ToolTip toolTipWindow, Point ApplicationLocation, Point MouseLocation, int startFromHeight)
		{
			if (_weatherData != null && _weatherData.DayForcast.Count > 1)
			{
				var title = new List<string>();
				var text = new List<string>();
				var image = new List<Image>();

                for (var i = 1; i <= 3; i++)
                {
                    if (_weatherData.DayForcast.Count <= i)
                    {
                        break;
                    }

                    title.Add(_weatherData.DayForcast[i].Item1);
                    image.Add(GetImage(_weatherData.DayForcast[i].Item2));
                    text.Add(_weatherData.DayForcast[i].Item3 + " " + _weatherData.DayForcast[i].Item4 + CELCIUS);
                }

				toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
				{
					toolTipWindow.Hide();
					Gadget.ProcessManager.Process(ClickType, ClickParameter);
				};
				toolTipWindow.Show(new Point(ApplicationLocation.X + MouseLocation.X, ApplicationLocation.Y + MouseLocation.Y), title, text, _font, _brush, image);
			}
		}

		public int GetHeight()
		{
			if (_weatherData != null)
				return _weatherData.TodayWeatherImage.Height + _font.Height * 2 + 5;
			else
				return _font.Height * 2 + 5;
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

		private void UpdateImage()
		{
			var dateTime = DateTime.Now;
			bool isDayNow = false;
			if (dateTime.TimeOfDay > _weatherData.SunRiseDateTime.TimeOfDay && dateTime.TimeOfDay < _weatherData.SunSetDateTime.TimeOfDay)
				isDayNow = true;
			if (_weatherData.IsDayNow != isDayNow)
			{
				if (_weatherData.DayForcast.Count > 0)
				{
					_weatherData.TodayWeatherImage = GetImage(_weatherData.DayForcast[0].Item2, _weatherData.SunRiseDateTime, _weatherData.SunSetDateTime);
					_weatherData.IsDayNow = isDayNow;
				}
				else
					_weatherData.TodayWeatherImage = GetImage(null, _weatherData.SunRiseDateTime, _weatherData.SunSetDateTime);
			}
		}

        private static Image GetImage(string name, DateTime sunRise, DateTime sunSet)
        {
            var now = DateTime.Now;
            var dayOrNight = now.TimeOfDay > sunRise.TimeOfDay && now.TimeOfDay < sunSet.TimeOfDay
                ? ""
                : "2";

            return GetImage(name + dayOrNight);
        }

        private static Image GetImage(string name)
		{
            var file = $"Resources\\Weather\\{name.ToLower()}.png";

            if (File.Exists(file))
            {
                return Image.FromFile(file);
            }

            return new Bitmap(140, 90);
		}

		private static WeatherData GetWeather(string url)
		{
			var weatherData = new WeatherData();

			try
			{
				var myRequest = (HttpWebRequest)WebRequest.Create(url);
				var myResponse = myRequest.GetResponse();
				var sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(1252));
				string html = sr.ReadToEnd();
				sr.Close();
				myResponse.Close();

				string text = "Current conditions";
				int index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);

				text = "Feels Like: <strong>";
				index = html.IndexOf(text);

				string data = html.Substring(0, index);
				string[] strArray = GetTextArray(data);
				if (strArray.Length == 3)
				{
					weatherData.Temperature = GetTemperature(strArray[0] + strArray[1]);
					weatherData.Weather = strArray[2];
				}
				else if (strArray.Length == 4)
				{
					weatherData.Temperature = GetTemperature(strArray[0] + strArray[1]);
					weatherData.Wind = strArray[2];
					weatherData.Weather = strArray[3];
				}

				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.TemperatureFeelsLike = GetTemperature(html.Substring(0, index));

				text = "Barometer:  <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.Barometer = html.Substring(0, index).Trim();

				text = "Dewpoint:   <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.TemperatureDewpoint = GetTemperature(html.Substring(0, index));

				text = "Humidity:   <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.Humidity = html.Substring(0, index).Trim();

				text = "Visibility: <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.Visibility = html.Substring(0, index).Trim();

				text = "Sun rise: <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.SunRise = html.Substring(0, index).Trim();
                DateTime currentSunRiseDateTime;
                DateTime.TryParseExact(weatherData.SunRise, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out currentSunRiseDateTime);
                weatherData.SunRiseDateTime = currentSunRiseDateTime;

                text = "Sun set:  <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.SunSet = html.Substring(0, index).Trim();
                DateTime currentSunSetDateTime;
                DateTime.TryParseExact(weatherData.SunSet, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out currentSunSetDateTime);
                weatherData.SunSetDateTime = currentSunSetDateTime;

                myRequest = (HttpWebRequest)WebRequest.Create(url + "?tenday");
				myResponse = myRequest.GetResponse();
				sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(1252));
				html = sr.ReadToEnd();
				sr.Close();
				myResponse.Close();

				text = "<h4>10 day forecast</h4>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				
				for (int i = 0; i < 10; i++)
				{
					text = "<span class=\"h5\">";
					index = html.IndexOf(text);
					html = html.Substring(index + text.Length, html.Length - index - text.Length);
					index = html.IndexOf("<");
					string day = html.Substring(0, index).Trim();

					text = "title=\"";
					index = html.IndexOf(text);
					html = html.Substring(index + text.Length, html.Length - index - text.Length);
					index = html.IndexOf("\"");
					string weather = html.Substring(0, index).Trim();

					text = "Hi: <strong>";
					index = html.IndexOf(text);
					html = html.Substring(index + text.Length, html.Length - index - text.Length);
					index = html.IndexOf("<");
					string high = GetTemperature(html.Substring(0, index));

					text = "Lo: <strong>";
					index = html.IndexOf(text);
					html = html.Substring(index + text.Length, html.Length - index - text.Length);
					index = html.IndexOf("<");
					string low = GetTemperature(html.Substring(0, index));

					weatherData.DayForcast.Add(new Tuple<string, string, string, string>(day, weather, low, high));
				}

				weatherData.IsDayNow = true;
				if (weatherData.DayForcast.Count > 0 && File.Exists("Resources\\Weather\\" + weatherData.DayForcast[0].Item2 + ".png"))
					weatherData.TodayWeatherImage = Image.FromFile("Resources\\Weather\\" + weatherData.DayForcast[0].Item2 + ".png");
				else
					weatherData.TodayWeatherImage = new Bitmap(140, 90);
			}
			catch
			{
			}

			return weatherData;
		}

		private static string[] GetTextArray(string data)
		{
			var result = new List<string>();
			int index;
			while ((index = data.IndexOf(">")) >= 0)
			{
				data = data.Substring(index + 1, data.Length - index - 1);
				index = data.IndexOf("<");
				if (index >= 0)
				{
					string str = data.Substring(0, index).Trim();
					if (str != "")
						result.Add(str);
					data = data.Substring(index + 1, data.Length - index - 1);
				}
			}
			return result.ToArray();
		}

		private static string GetTemperature(string str)
		{
			int lastValid = -1;
			int number = 0;

			int.TryParse(str, out number);

			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsDigit(str[i]) || str[i] == '-' || str[i] == '+')
					lastValid = i;
				else
					break;
			}

			if (lastValid >= 0)
				int.TryParse(str.Substring(0, lastValid + 1), out number);

			if (number > 0)
				return "+" + number;
			else
				return number.ToString();
		}
	}
}
