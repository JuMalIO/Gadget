using Gadget.Config;
using Gadget.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Gadget.Widgets.Rss
{
	public class Rss : IWidget, IWidgetWithText, IWidgetWithIcon, IWidgetWithClick, IWidgetWithHover, IWidgetWithInternet
    {
        #region public properties IWidget

        public string Id { get; set; } = "RSS";
        public string Name { get; set; } = "RSS";
        public int Position { get; set; } = 999;
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

        public Color NewColor { get; set; } = Color.Red;
        public int MaxTitles { get; set; } = 5;
        public List<string> RssLinks { get; set; } = new List<string>() { "http://feeds.feedburner.com/pocketnow?format=xml" };

        #endregion

        private List<RssData> _rssDataList;
        private int _updateInternetCount;
		private Font _font;
		private Brush _brush;
		private Brush _newBrush;
		private Image _image;

		public void Initiate()
		{
			_rssDataList = new List<RssData>();
			_font = new Font(FontName, FontSize, FontStyle.Regular);
			_brush = new SolidBrush(Color);
			_newBrush = new SolidBrush(NewColor);
			_image = Resources.rss;
        }

		public void UpdateInternet()
		{
			_updateInternetCount++;
			if (UpdateInternetInterval < _updateInternetCount || _rssDataList.Count == 0)
			{
				GetRssFeed(RssLinks, MaxTitles, ref _rssDataList);
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
                height = height + _font.Height + 5;
            }

			int rssWidth = width - 10;
			for (int i = 0; i < _rssDataList.Count; i++)
			{
				if (i >= MaxTitles)
					break;
				int textWidth = (int)graphics.MeasureString(_rssDataList[i].Title, _font).Width;
				Brush brush;
				if (_rssDataList[i].IsNew)
					brush = _newBrush;
				else
					brush = _brush;
				if (textWidth > rssWidth)
				{
					graphics.DrawString(_rssDataList[i].Title, _font, brush, new RectangleF(5, height, rssWidth - 5, _font.Height));
					graphics.DrawString("...", _font, brush, rssWidth - 5, height);
				}
				else
					graphics.DrawString(_rssDataList[i].Title, _font, brush, 5, height);
				height = height + _font.Height;
			}
			height = height + 5;
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
				int x = (MouseLocation.Y - startFromHeight) / _font.Height;
				if (_rssDataList.Count > x && x < MaxTitles && x >= 0)
				{
					_rssDataList[x].IsNew = false;
					System.Diagnostics.Process.Start(_rssDataList[x].Link);
				}
			}
		}

		public void Hover(Point ApplicationLocation, Point MouseLocation, int startFromHeight)
		{
			if (IsIconVisible)
			{
				startFromHeight += _font.Height + 5;
			}
			if (MouseLocation.Y >= startFromHeight)
			{
				int x = (MouseLocation.Y - startFromHeight) / _font.Height;
				if (_rssDataList.Count > x && x < MaxTitles && x >= 0)
				{
					_rssDataList[x].IsNew = false;
                    var toolTipWindow = new Gadget.ToolTip();
                    toolTipWindow.MouseClick += delegate(object obj, MouseEventArgs a)
					{
						toolTipWindow.Hide();
						System.Diagnostics.Process.Start(_rssDataList[x].Link);
					};
					toolTipWindow.Show(new Point(ApplicationLocation.X + MouseLocation.X, ApplicationLocation.Y + MouseLocation.Y), _rssDataList[x].Title, _rssDataList[x].Summary + "\n" + _rssDataList[x].Date, new Font(FontName, FontSize + 2, FontStyle.Regular), _font, _brush, _rssDataList[x].Picture);
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
			if (_rssDataList.Count > MaxTitles)
				height += _font.Height * MaxTitles;
			else
				height += _font.Height * _rssDataList.Count;
			return height;
		}

		public bool ShowProperties()
		{
            var rssUserControl = new RssUserControl(NewColor, MaxTitles, RssLinks);
			var propertiesForm = new PropertiesForm(this, new [] { rssUserControl });
			if (propertiesForm.ShowDialog() == DialogResult.OK)
			{
                NewColor = rssUserControl.Color;
                MaxTitles = rssUserControl.MaxTitles;
                RssLinks = rssUserControl.RssLinks;

				Initiate();
                UpdateInternet();

                return true;
            }
			return false;
		}

		public static void GetRssFeed(List<string> rssLinks, int max, ref List<RssData> rssDataList)
		{
			try
			{
				foreach (var url in rssLinks)
				{
					XmlReader reader = XmlReader.Create(url);
					SyndicationFeed feed = SyndicationFeed.Load(reader);
					reader.Close();
					foreach (SyndicationItem syndicationItem in feed.Items)
					{
						if (!rssDataList.Any(x => x.Link == syndicationItem.Id))
						{
							RssData rssData = new RssData();
							List<Uri> uriList = null;
							if (syndicationItem.Content != null)
							{
								uriList = FetchLinksFromSource(((TextSyndicationContent)syndicationItem.Content).Text);
							}
							else
							{
								string content = "";
								foreach (SyndicationElementExtension ext in syndicationItem.ElementExtensions)
								{
									if (ext.GetObject<XElement>().Name.LocalName == "encoded")
										content += ext.GetObject<XElement>().Value;
								}
								uriList = FetchLinksFromSource(content);
							}

							if (uriList != null && uriList.Count > 0)
								rssData.Picture = ResizeImage(GetImage(uriList[0]), 255, 255);
							rssData.Link = syndicationItem.Id;
							rssData.Title = HttpUtility.HtmlDecode(syndicationItem.Title.Text);
							rssData.Summary = new Regex("<.*?>", RegexOptions.Compiled).Replace(HttpUtility.HtmlDecode(syndicationItem.Summary.Text), string.Empty);
							rssData.Date = syndicationItem.PublishDate.ToString("yyyy.MM.dd HH:mm");
							rssData.IsNew = true;
							rssDataList.Add(rssData);
						}
					}
				}
			}
			catch
			{
			}
			rssDataList.Sort((item1, item2) => item2.Date.CompareTo(item1.Date));
			if (rssDataList.Count > max)
				rssDataList.RemoveRange(max, rssDataList.Count - max);
		}

		public static List<Uri> FetchLinksFromSource(string htmlSource)
		{
			List<Uri> links = new List<Uri>();
			string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
			MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			foreach (Match m in matchesImgSrc)
			{
				string href = m.Groups[1].Value;
				links.Add(new Uri(href));
			}
			return links;
		}

		public static Image GetImage(Uri uri)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
				HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream stream = httpWebReponse.GetResponseStream();
				return Image.FromStream(stream);
			}
			catch
			{
				return null;
			}
		}

		public static Image ResizeImage(Image imgToResize, int x, int y)
		{
			if (imgToResize != null)
			{
				if (imgToResize.Width > x || imgToResize.Height > y)
				{
					if (imgToResize.Width > imgToResize.Height)
						y = (int)((double)x * (double)((double)imgToResize.Height / (double)imgToResize.Width));
					else
						x = (int)((double)y * (double)((double)imgToResize.Width / (double)imgToResize.Height));
					return (Image)(new Bitmap(imgToResize, new Size(x, y)));
				}
				else
					return imgToResize;
			}
			else
				return null;
		}
	}
}
