using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Gadget.Widgets.Date
{
	public partial class CalendarForm : Form
	{
		private int _year;

		public CalendarForm()
		{
			InitializeComponent();

            var date = DateTime.Now;
            Text = "Today: " + date.ToString("yyyy.MM.dd");
            _year = date.Year;

			UpdateCalendar();
		}

        private static MonthCalendar GetMonthCalendar()
        {
            var monthCalendar = new MonthCalendar();
            monthCalendar.CalendarDimensions = new Size(6, 1);
            monthCalendar.Enabled = false;
            monthCalendar.Location = new Point(0, 37);
            monthCalendar.MaxSelectionCount = 366;
            monthCalendar.ShowToday = false;
            monthCalendar.ShowWeekNumbers = true;
            monthCalendar.Visible = false;
            return monthCalendar;
        }

        private static Image GetFullCalendarImage(int year)
		{
            var monthCalendar = GetMonthCalendar();
            var monthCalendarWidth = monthCalendar.Size.Width;
			var monthCalendarHeight = monthCalendar.Size.Height;
			var rectangle1 = new Rectangle(0, 0, monthCalendarWidth, monthCalendarHeight);
			var rectangle2 = new Rectangle(monthCalendarWidth / 6 + 18, 15, (monthCalendarWidth / 6 - 9) * 4, monthCalendarHeight - 38);

			monthCalendar.MinDate = new DateTime(year - 1, 12, 1);
			monthCalendar.MaxDate = new DateTime(year, 5, 31, 23, 59, 59);
			monthCalendar.SetDate(monthCalendar.MinDate);
			var bitmap1 = new Bitmap(monthCalendarWidth, monthCalendarHeight);
			monthCalendar.DrawToBitmap(bitmap1, rectangle1);
			var bitmapCrop1 = (Bitmap)CropImage(bitmap1, rectangle2);

			monthCalendar.MinDate = new DateTime(year, 4, 1);
			monthCalendar.MaxDate = new DateTime(year, 9, 30, 23, 59, 59);
			monthCalendar.SetDate(monthCalendar.MinDate);
            var bitmap2 = new Bitmap(monthCalendarWidth, monthCalendarHeight);
			monthCalendar.DrawToBitmap(bitmap2, rectangle1);
			var bitmapCrop2 = (Bitmap)CropImage(bitmap2, rectangle2);

			monthCalendar.MinDate = new DateTime(year, 8, 1);
			monthCalendar.MaxDate = new DateTime(year + 1, 1, 31, 23, 59, 59);
			monthCalendar.SetDate(monthCalendar.MinDate);
			var bitmap3 = new Bitmap(monthCalendarWidth, monthCalendarHeight);
			monthCalendar.DrawToBitmap(bitmap3, rectangle1);
			var bitmapCrop3 = (Bitmap)CropImage(bitmap3, rectangle2);

			var bitmap = new Bitmap(bitmapCrop1.Width, bitmapCrop1.Height + bitmapCrop2.Height + bitmapCrop3.Height);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(bitmapCrop1, new Point(0, 0));
				graphics.DrawImage(bitmapCrop2, new Point(0, bitmapCrop1.Height));
				graphics.DrawImage(bitmapCrop3, new Point(0, bitmapCrop1.Height + bitmapCrop2.Height));
            }

            bitmap1.Dispose();
            bitmap2.Dispose();
            bitmap3.Dispose();
            bitmapCrop1.Dispose();
            bitmapCrop2.Dispose();
            bitmapCrop3.Dispose();
            monthCalendar.Dispose();

            return bitmap;
		}

		private static Image GetMonthCalendarImage(int year)
		{
			var nowDateTime = DateTime.Now;
			var startDateTime = nowDateTime.AddMonths(-1);
			var endDateTime = nowDateTime.AddMonths(4);
            var monthCalendar = GetMonthCalendar();
            var monthCalendarWidth = monthCalendar.Size.Width;
			var monthCalendarHeight = monthCalendar.Size.Height;
			var rectangle1 = new Rectangle(0, 0, monthCalendarWidth, monthCalendarHeight);
			var rectangle2 = new Rectangle(monthCalendarWidth / 6 + 18, 17, monthCalendarWidth / 6 - 9, monthCalendarHeight - 38);
			monthCalendar.MinDate = new DateTime(startDateTime.Year, startDateTime.Month, 1);
			monthCalendar.MaxDate = new DateTime(endDateTime.Year, endDateTime.Month, DateTime.DaysInMonth(endDateTime.Year, endDateTime.Month), 23, 59, 59);
			monthCalendar.SetDate(monthCalendar.MinDate);
			var bitmap = new Bitmap(monthCalendarWidth, monthCalendarHeight);
			monthCalendar.DrawToBitmap(bitmap, rectangle1);
            var bitmapCrop = CropImage(bitmap, rectangle2);

            bitmap.Dispose();
            monthCalendar.Dispose();

            return bitmapCrop;
        }

        private static Image CropImage(Bitmap img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }

        private static Bitmap Transform(Bitmap source)
        {
            var newBitmap = new Bitmap(source.Width, source.Height);
            using (var graphics = Graphics.FromImage(newBitmap))
            {
                // create the negative color matrix
                var colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] {-1, 0, 0, 0, 0},
                    new float[] {0, -1, 0, 0, 0},
                    new float[] {0, 0, -1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 0, 1}
                });
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                var rectangle = new Rectangle(0, 0, source.Width, source.Height);
                graphics.DrawImage(source, rectangle, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;
        }

        public static Bitmap GetCalendarBitmap(int? year = null)
        {
            using (var bitmap = GetMonthCalendarImage(year ?? DateTime.Now.Year))
            {
                var bitmapTransform = Transform((Bitmap)bitmap);
                bitmapTransform.MakeTransparent(Color.Black);
                return bitmapTransform;
            }  
        }

        private void UpdateCalendar()
        {
            lblYear.Text = _year.ToString();
            pbCalendar.Image = GetFullCalendarImage(_year);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _year++;
            UpdateCalendar();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            _year--;
            UpdateCalendar();
        }

        private void lblYear_Click(object sender, EventArgs e)
        {
            _year = DateTime.Now.Year;
            UpdateCalendar();
        }

        private void pbCalendar_MouseClick(object sender, MouseEventArgs e)
        {
            _year += e.X > (pbCalendar.Size.Width / 2) ? 1 : -1;
            UpdateCalendar();
        }

        private void CalendarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
