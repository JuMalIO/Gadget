using Gadget.Config;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace Gadget.Widgets.Currency
{
	public class Currency : IWidget, IWidgetWithText, IWidgetWithIcon, IWidgetWithClick, IWidgetWithHover, IWidgetWithInternet
	{
        #region public properties IWidget

        public string Id { get; set; } = "Currency";
        public string Name { get; set; } = "Currency";
        public int Position { get; set; } = 998;
        public bool IsVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithText

        public Color Color { get; set; } = Color.White;
        public string FontName { get; set; } = "Segoe UI";
        public int FontSize { get; set; } = 10;

        #endregion

        #region public properties IWidgetWithIcon

        public bool IsIconVisible { get; set; } = true;

        #endregion

        #region public properties IWidgetWithClick

        public bool IsClickable { get; set; } = true;
        public ClickType ClickType { get; set; } = ClickType.Disabled;
        public string ClickParameter { get; set; } = "";

        #endregion

        #region public properties IWidgetWithHover

        public bool IsHoverable { get; set; } = true;

        #endregion

        #region public properties IWidgetWithInternet

        public int UpdateInternetInterval { get; set; } = 60;

        #endregion

        #region public properties

        public CurrencyType CurrencyType { get; set; } = CurrencyType.BankBuysCash;
        public bool CurrencyShort { get; set; } = false;
        public List<string> VisibleCurrency { get; set; } = new List<string>() { "EUR", "USD" };

        #endregion
        
        private int _updateInternetCount;
		private Font _font;
		private Brush _brush;
		private Image _image;
		private string _url;
		private List<CurrencyData> _currencyDataList;

		public void Initiate(string url = "https://ib.swedbank.lt/private/home/more/pricesrates/rates?language=LIT")
		{
			_url = url;
			_currencyDataList = new List<CurrencyData>();
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
			_image = Image.FromFile("Resources\\Icons\\currency.png");
		}

		public void UpdateInternet()
		{
			_updateInternetCount++;
			if (UpdateInternetInterval < _updateInternetCount || _currencyDataList.Count == 0)
			{
				_currencyDataList = GetCurrency(_url, VisibleCurrency);
				_updateInternetCount = 0;
			}
		}

		public void Draw(Graphics graphics, int width, int height)
		{
			if (IsIconVisible)
			{
				var imageSize = _image.Width;
				graphics.DrawImageUnscaledAndClipped(_image, new Rectangle(5, height + (int)((_font.Height - imageSize) / 2.0), imageSize, imageSize));
				imageSize += 5;
				graphics.DrawString(Name, _font, _brush, 5 + imageSize, height);
                height += _font.Height + 5;
            }

			for (int i = 0; i < _currencyDataList.Count; i++)
			{
				if (_currencyDataList[i].Visible)
				{
					if (CurrencyShort)
						graphics.DrawString(_currencyDataList[i].CurrencyShort, _font, _brush, 5, height);
					else
						graphics.DrawString(_currencyDataList[i].Currency, _font, _brush, 5, height);
					if (_currencyDataList[i].Money.ContainsKey(CurrencyType))
					{
						string date = _currencyDataList[i].Money[CurrencyType] + " LT";
						int textWidth = (int)graphics.MeasureString(date, _font).Width;
						graphics.DrawString(date, _font, _brush, width - textWidth - 5, height);
					}
					height += _font.Height;
				}
			}
			height += 5;
		}

		public void Update()
		{
		}

		public void Click(Point MouseLocation, int startFromHeight)
		{
			if (IsIconVisible)
			{
				startFromHeight += _font.Height + 5;
			}
			if (MouseLocation.Y >= startFromHeight)
			{
				System.Diagnostics.Process.Start(_url);
			}
		}

		public void Hover(Gadget.ToolTip toolTipWindow, Point ApplicationLocation, Point MouseLocation, int startFromHeight)
		{
			if (IsIconVisible)
			{
				startFromHeight += _font.Height + 5;
			}
			if (MouseLocation.Y >= startFromHeight)
			{
				for (int i = 0; i < _currencyDataList.Count; i++)
				{
					if (_currencyDataList[i].Visible)
					{
						startFromHeight += _font.Height;
						if (MouseLocation.Y < startFromHeight)
						{
							string text = "Bank Buys Cash: " + _currencyDataList[i].Money[CurrencyType.BankBuysCash] + " LT\nBank Buys Transfer: " + _currencyDataList[i].Money[CurrencyType.BankBuysTransfer] + " LT\nBank Sells Cash: " + _currencyDataList[i].Money[CurrencyType.BankSellsCash] + " LT\nBank Sells Transfer: " + _currencyDataList[i].Money[CurrencyType.BankSellsTransfer] + " LT\nBank Of Lithuania: " + _currencyDataList[i].Money[CurrencyType.BankOfLithuania] + " LT";
							toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
							{
								toolTipWindow.Hide();
								System.Diagnostics.Process.Start(_url);
							};
							toolTipWindow.Show(new Point(ApplicationLocation.X + MouseLocation.X, ApplicationLocation.Y + MouseLocation.Y), _currencyDataList[i].Currency, text, new Font(FontName, FontSize + 2, FontStyle.Regular), _font, _brush, null);
							break;
						}
					}
				}
			}
		}

		public int GetHeight()
		{
			int height = 5;
			if (IsIconVisible)
			{
				height += _font.Height + 5;
			}
			for (int i = 0; i < _currencyDataList.Count; i++)
			{
				if (_currencyDataList[i].Visible)
					height += _font.Height;
			}
			return height;
		}

		public bool ShowProperties()
		{
            var currencyUserControl = new CurrencyUserControl(_currencyDataList, VisibleCurrency, CurrencyShort, (int)CurrencyType);
			var propertiesForm = new PropertiesForm(this, new [] { currencyUserControl });
			if (propertiesForm.ShowDialog() == DialogResult.OK)
			{
				VisibleCurrency = currencyUserControl.VisibleCurrency;
				CurrencyType = (CurrencyType)currencyUserControl.Type;
				CurrencyShort = currencyUserControl.CurrencyShort;

				Initiate();
                UpdateInternet();

                return true;
            }
			return false;
		}

		public static List<CurrencyData> GetCurrency(string url, List<string> visibleCurrency)
		{
			List<CurrencyData> currencyDataList = new List<CurrencyData>();
			try
			{
				HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
				WebResponse myResponse = myRequest.GetResponse();
				System.IO.StreamReader sr = new System.IO.StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.GetEncoding(1252));
				string html = sr.ReadToEnd();
				sr.Close();
				myResponse.Close();

				string text = "Valiuta</th>";
				int index = html.IndexOf(text);
				html = html.Substring(index + text.Length, html.Length - index - text.Length);

				text = "<tr>";
				while ((index = html.IndexOf(text)) >= 0)
				{
					html = html.Substring(index + text.Length, html.Length - index - text.Length);
					index = html.IndexOf("</tr>");
					if ((index = html.IndexOf("</tr>")) >= 0)
					{
						string data = html.Substring(0, index);
						html = html.Substring(index, html.Length - index);
						string[] strArray = GetTextArray(data);
						if (strArray.Length == 6)
						{
							CurrencyData currencyData = new CurrencyData();
							currencyData.Currency = HttpUtility.HtmlDecode(strArray[0]).Replace((char)0XA0, ' ');
							string[] strArray2 = currencyData.Currency.Split(' ');
							if (strArray2.Length > 0)
								currencyData.CurrencyShort = strArray2[0];
							else
								currencyData.CurrencyShort = currencyData.Currency;
							currencyData.Money[CurrencyType.BankBuysCash] = strArray[1];
							currencyData.Money[CurrencyType.BankBuysTransfer] = strArray[2];
							currencyData.Money[CurrencyType.BankSellsCash] = strArray[3];
							currencyData.Money[CurrencyType.BankSellsTransfer] = strArray[4];
							currencyData.Money[CurrencyType.BankOfLithuania] = strArray[5];
							string str = visibleCurrency.FirstOrDefault(x => x == currencyData.CurrencyShort);
							if (str != null)
								currencyData.Visible = true;
							else
								currencyData.Visible = false;
							currencyDataList.Add(currencyData);
						}
					}
				}
			}
			catch
			{
			}

			return currencyDataList;
		}

		private static string[] GetTextArray(string data)
		{
			List<string> result = new List<string>();
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
	}
}
