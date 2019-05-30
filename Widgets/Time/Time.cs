using Gadget.Config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Widgets.Time
{
	public class Time : IWidget, IWidgetWithText, IWidgetWithClick, IWidgetWithHover
    {
        #region public properties IWidget

        public string Id { get; set; } = "Time";
        public string Name { get; set; } = "Time";
        public int Position { get; set; } = -2;
        public bool IsVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithText

        public Color Color { get; set; } = Color.White;
        public string FontName { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 10;

        #endregion

        #region public properties IWidgetWithClick

        public bool IsClickable { get; set; } = true;
        public ClickType ClickType { get; set; } = ClickType.Disabled;
        public string ClickParameter { get; set; } = "";

        #endregion

        #region public properties IWidgetWithHover

        public bool IsHoverable { get; set; } = true;

        #endregion

        #region public properties

        public int Hour { get; set; } = 0;
        public int Minute { get; set; } = 30;
        public AlarmType AlarmType { get; set; } = AlarmType.Alarm;
        public bool AlarmRepeat { get; set; } = false;
        public TimeType TimeType { get; set; } = TimeType.AfterThatTime;
        public bool IsAlarmEnabled { get; set; } = false;

        #endregion

        private DateTime _alarmDateTime;
		private Font _font;
		private Brush _brush;
		private Image[] _time;

		public Time()
		{
			if (AlarmRepeat && !IsAlarmEnabled)
				IsAlarmEnabled = true;
		}

		public void Initiate()
		{
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);

			_time = new Image[11];
			for (int i = 0; i < _time.Length; i++)
			{
				_time[i] = Image.FromFile($"Resources\\Time\\{i.ToString("00")}.png");
			}

            _alarmDateTime = GetAlarmDateTime(TimeType, Hour, Minute);
        }

		public void Draw(Graphics graphics, int width, int height)
		{
			if (IsAlarmEnabled)
			{
				graphics.DrawString(AlarmType + " in:", _font, _brush, 5, height);
				string date = (_alarmDateTime.Subtract(DateTime.Now)).ToString("hh':'mm':'ss");
				graphics.DrawString(date, _font, _brush, 5, height + _font.Height);
			}

			var dateTime = DateTime.Now;
			int timeNumberWidth = _time[0].Width;
			int timeNumberHeight = _time[0].Height;
			int timeDotsWidth = _time[10].Width;
			int timeSpaceWidth = 2;
			int posX = ((width - ((timeNumberWidth + timeSpaceWidth) * 4 + timeDotsWidth)) / 2);
			graphics.DrawImageUnscaledAndClipped(_time[dateTime.Hour / 10], new Rectangle(posX, height, timeNumberWidth, timeNumberHeight));
			posX += timeNumberWidth + timeSpaceWidth;
			graphics.DrawImageUnscaledAndClipped(_time[dateTime.Hour % 10], new Rectangle(posX, height, timeNumberWidth, timeNumberHeight));
			posX += timeNumberWidth + timeSpaceWidth;
			graphics.DrawImageUnscaledAndClipped(_time[10], new Rectangle(posX, height, timeDotsWidth, timeNumberHeight));
			posX += timeDotsWidth + timeSpaceWidth;
			graphics.DrawImageUnscaledAndClipped(_time[dateTime.Minute / 10], new Rectangle(posX, height, timeNumberWidth, timeNumberHeight));
			posX += timeNumberWidth + timeSpaceWidth;
			graphics.DrawImageUnscaledAndClipped(_time[dateTime.Minute % 10], new Rectangle(posX, height, timeNumberWidth, timeNumberHeight));
		}

		public void Update()
		{
			if (IsAlarmEnabled)
			{
				var dateTime = DateTime.Now;
				if (dateTime >= _alarmDateTime)
				{
					IsAlarmEnabled = false;
					ActionForm actionForm = new ActionForm(AlarmType);
					actionForm.ShowDialog();
					IsAlarmEnabled = AlarmRepeat;
				}
			}
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			var alarmForm = new AlarmForm(Hour, Minute, AlarmType, TimeType, AlarmRepeat, IsAlarmEnabled);
			if (alarmForm.ShowDialog() == DialogResult.OK)
			{
				Hour = alarmForm.Hour;
				Minute = alarmForm.Minute;
				AlarmType = alarmForm.AlarmType;
				TimeType = alarmForm.TimeType;
				AlarmRepeat = alarmForm.AlarmRepeat;
				IsAlarmEnabled = alarmForm.Enable;

                Initiate();
            }
		}

		public void Hover(Gadget.ToolTip toolTipWindow, Point ApplicationLocation, Point MouseLocation, int startFromHeight)
		{
			toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
			{
				toolTipWindow.Hide();
				Click(default(Point), 0);
			};
			toolTipWindow.Show(new Point(ApplicationLocation.X + MouseLocation.X, ApplicationLocation.Y + MouseLocation.Y), Date.CalendarForm.GetCalendarBitmap());
		}

		public int GetHeight()
		{
			return _time[0].Height + 5;
		}

		public bool ShowProperties()
        {
            var propertiesForm = new PropertiesForm(this);
			if (propertiesForm.ShowDialog() == DialogResult.OK)
            {
                Initiate();

                return true;
            }
			return false;
		}

        private static DateTime GetAlarmDateTime(TimeType timeType, int hour, int minute)
        {
            if (timeType == TimeType.OnThatTime)
            {
                var dateTimeNow = DateTime.Now;
                var dateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, hour, minute, 0);
                if (dateTimeNow > dateTime)
                {
                    dateTime = dateTime.AddDays(1);
                }
                return dateTime;
            }
            else
            {
                var dateTime = DateTime.Now.AddHours(hour).AddMinutes(minute);
                return dateTime;
            }
        }
	}
}
