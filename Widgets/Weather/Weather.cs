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
					graphics.DrawString("Sun Rise/Set " + _weatherData.CurrentSunRise + "/" + _weatherData.CurrentSunSet, _font, _brush, 5, height);
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
				title.Add(_weatherData.DayForcast[1].Item1);
				image.Add(GetImage(_weatherData.DayForcast[1].Item2, _weatherData.CurrentSunRiseDateTime, _weatherData.CurrentSunSetDateTime));
				text.Add(_weatherData.DayForcast[1].Item3 + " " + _weatherData.DayForcast[1].Item4 + CELCIUS);
				if (_weatherData.DayForcast.Count > 2)
				{
					title.Add(_weatherData.DayForcast[2].Item1);
					image.Add(GetImage(_weatherData.DayForcast[2].Item2, _weatherData.CurrentSunRiseDateTime, _weatherData.CurrentSunSetDateTime));
					text.Add(_weatherData.DayForcast[2].Item3 + " " + _weatherData.DayForcast[2].Item4 + CELCIUS);
				}
				if (_weatherData.DayForcast.Count > 3)
				{
					title.Add(_weatherData.DayForcast[3].Item1);
					image.Add(GetImage(_weatherData.DayForcast[2].Item2, _weatherData.CurrentSunRiseDateTime, _weatherData.CurrentSunSetDateTime));
					text.Add(_weatherData.DayForcast[3].Item3 + " " + _weatherData.DayForcast[3].Item4 + CELCIUS);
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
            var propertiesForm = new PropertiesForm(this, new UserControl[] { weatherUserControl });
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
			if (dateTime.TimeOfDay > _weatherData.CurrentSunRiseDateTime.TimeOfDay && dateTime.TimeOfDay < _weatherData.CurrentSunSetDateTime.TimeOfDay)
				isDayNow = true;
			if (_weatherData.IsDayNow != isDayNow)
			{
				if (_weatherData.DayForcast.Count > 0)
				{
					_weatherData.TodayWeatherImage = GetImage(_weatherData.DayForcast[0].Item2, _weatherData.CurrentSunRiseDateTime, _weatherData.CurrentSunSetDateTime);
					_weatherData.IsDayNow = isDayNow;
				}
				else
					_weatherData.TodayWeatherImage = GetImage(null, _weatherData.CurrentSunRiseDateTime, _weatherData.CurrentSunSetDateTime);
			}
		}

		private static Image GetImage(string name, DateTime currentSunRiseDateTime, DateTime currentSunSetDateTime)
		{
			var dateTime = DateTime.Now;
			string end;
            if (!(dateTime.TimeOfDay > currentSunRiseDateTime.TimeOfDay && dateTime.TimeOfDay < currentSunSetDateTime.TimeOfDay))
                end = "2";
            else
                end = "";
            if (File.Exists("Resources\\Weather\\w" + name + end + ".png"))
				return Image.FromFile("Resources\\Weather\\w" + name + end + ".png");
			else
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
					weatherData.CurrentTemperature = GetTemperature(strArray[0] + strArray[1]);
					weatherData.CurrentWeather = strArray[2];
				}
				else if (strArray.Length == 4)
				{
					weatherData.CurrentTemperature = GetTemperature(strArray[0] + strArray[1]);
					weatherData.CurrentWind = strArray[2];
					weatherData.CurrentWeather = strArray[3];
				}

				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentTemperatureFeelsLike = GetTemperature(html.Substring(0, index));

				text = "Barometer:  <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentBarometer = html.Substring(0, index).Trim();

				text = "Dewpoint:   <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentTemperatureDewpoint = GetTemperature(html.Substring(0, index));

				text = "Humidity:   <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentHumidity = html.Substring(0, index).Trim();

				text = "Visibility: <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentVisibility = html.Substring(0, index).Trim();

				text = "Sun rise: <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentSunRise = html.Substring(0, index).Trim();
                DateTime currentSunRiseDateTime;
                DateTime.TryParseExact(weatherData.CurrentSunRise, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out currentSunRiseDateTime);
                weatherData.CurrentSunRiseDateTime = currentSunRiseDateTime;

                text = "Sun set:  <strong>";
				index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);
				index = html.IndexOf("<");
				weatherData.CurrentSunSet = html.Substring(0, index).Trim();
                DateTime currentSunSetDateTime;
                DateTime.TryParseExact(weatherData.CurrentSunSet, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out currentSunSetDateTime);
                weatherData.CurrentSunSetDateTime = currentSunSetDateTime;

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
				if (weatherData.DayForcast.Count > 0 && File.Exists("Resources\\Weather\\w" + weatherData.DayForcast[0].Item2 + ".png"))
					weatherData.TodayWeatherImage = Image.FromFile("Resources\\Weather\\w" + weatherData.DayForcast[0].Item2 + ".png");
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
