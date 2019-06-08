using Gadget.Config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Widgets.Date
{
    public class Date : IWidget, IWidgetWithText, IWidgetWithClick, IWidgetWithHover
	{
        #region public properties IWidget

        public string Id { get; set; } = "Date";
        public string Name { get; set; } = "Date";
        public int Position { get; set; } = -3;
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

        private Font _font;
		private Brush _brush;

        public void Initiate()
		{
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			var dateTime = DateTime.Now;
			graphics.DrawString(dateTime.DayOfWeek.ToString(), _font, _brush, 5, height);
			var date = dateTime.ToString("yyyy.MM.dd");
			var textWidth = (int)graphics.MeasureString(date, _font).Width;
			graphics.DrawString(date, _font, _brush, width - textWidth - 5, height);
		}

		public void Update()
		{
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			CalendarForm calendarForm = new CalendarForm();
			calendarForm.ShowDialog();
		}

		public void Hover(Point ApplicationLocation, Point MouseLocation, int startFromHeight)
		{
            var toolTipWindow = new Gadget.ToolTip();
            toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
			{
				toolTipWindow.Hide();
				Click(default(Point), 0);
			};
			toolTipWindow.Show(new Point(ApplicationLocation.X + MouseLocation.X, ApplicationLocation.Y + MouseLocation.Y), CalendarForm.GetCalendarBitmap());
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
                Initiate();

                return true;
            }
			return false;
		}
	}
}
