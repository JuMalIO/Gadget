using Gadget.Properties;
using Gadget.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Gadget.Gadget
{
	public class ToolTip : NativeWindow, IDisposable
	{
		private bool _visible = false;
		private byte _opacity = 255;
		private Point _location = new Point(0, 0);
		private Size _size = new Size(255, 255);
		private MethodInfo _commandDispatch;
		private IntPtr _handleBitmapDC;
		private Graphics _graphics;
		private Image _background, _borderTopLeft, _borderTop, _borderTopRight, _borderRight, _borderBottomRight, _borderBottom, _borderBottomLeft, _borderLeft;
		private string _title;
		private string _text;
		private Font _font;
		private Font _titleFont;
		private Brush _brush;
		private Image _image;
		private List<string> _titleList;
		private List<string> _textList;
		private List<Image> _imageList;

		public ToolTip()
		{
			Type commandType = typeof(Form).Assembly.GetType("System.Windows.Forms.Command");
			_commandDispatch = commandType.GetMethod("DispatchID", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(int) }, null);
			CreateHandle(CreateParams);
			NativeMethods.PreventFadingToGlass(Handle);

			_borderTopLeft = Resources.top_left;
			_borderTop = Resources.top;
			_borderTopRight = Resources.top_right;
			_borderRight = Resources.right;
			_borderBottomRight = Resources.bottom_right;
			_borderBottom = Resources.bottom;
			_borderBottomLeft = Resources.bottom_left;
			_borderLeft = Resources.left;
		}

		protected virtual CreateParams CreateParams
		{
			get
			{
				return NativeMethods.GetCreateParams(_location);
			}
		}

		private void Show()
		{
			_handleBitmapDC = NativeMethods.GetBuffer(_size);
			_graphics = NativeMethods.GetGraphics(_handleBitmapDC, 0);
			_graphics.Clear(Color.Transparent);

			NativeMethods.MoveWindowToTopMost(Handle);

			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, _location.X, _location.Y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSENDCHANGING);

			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | (NativeMethods.SWP_SHOWWINDOW));

			_visible = true;
			Redraw();
		}

		public void Show(Point location, Image image)
		{
			_imageList = null;

			_image = image;

			Size newSize = new Size(image.Width + 10, image.Height + 10);
			UpdateBackground(newSize.Width, newSize.Height);
			CheckLocationToFitOnScreen(ref location, ref newSize);

			Show();
		}

		public void Show(Point location, List<string> title, List<string> text, Font font, Brush brush, List<Image> image)
		{
			_image = null;

			int width = 10;
			for (int i = 0; i < image.Count; i++)
				width += image[i].Width;
			int height = 5 + font.Height + 5 + image[0].Height + 5 + font.Height + 5;
			
			_font = font;
			_brush = brush;
			_imageList = image;
			_titleList = title;
			_textList = text;

			Size newSize = new Size(width, height);
			UpdateBackground(newSize.Width, newSize.Height);
			CheckLocationToFitOnScreen(ref location, ref newSize);

			Show();
		}

		public void Show(Point location, string title, string text, Font titleFont, Font font, Brush brush, Image image)
		{
			_imageList = null;

			using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
			{
				SizeF titleSize = graphics.MeasureString(title, titleFont);
				int textWidth = 300;
				if (titleSize.Width > textWidth)
					textWidth = (int)titleSize.Width;
				SizeF textSize = graphics.MeasureString(text, font, textWidth);
				int height = (int)(titleSize.Height + textSize.Height) + 15;

				int defaultWidth = textWidth + 10;
				if (image != null)
				{
					_image = ResizeImage(image, height - 10);
					defaultWidth += _image.Width + 5;
				}
				else
					_image = null;

				Size newSize = new Size(defaultWidth, height);
				UpdateBackground(newSize.Width, newSize.Height);
				CheckLocationToFitOnScreen(ref location, ref newSize);
			}

			_titleFont = titleFont;
			_font = font;
			_brush = brush;
			_title = title;
			_text = text;

			Show();
		}

		private void UpdateBackground(int width, int height)
		{
			_background = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(_background))
			{
				using (SolidBrush brush2 = new SolidBrush(Color.FromArgb(180, Color.Black)))
				{
					g.FillRectangle(brush2, 0, 0, _background.Width, _background.Height);
				}
			}
		}

		private void CheckLocationToFitOnScreen(ref Point location, ref Size size)
		{
			location.X = location.X - size.Width - _borderTopLeft.Width;
			location.Y = location.Y - size.Height - _borderTopLeft.Height;
			_size = new Size(size.Width + _borderTopLeft.Width + _borderBottomRight.Width, size.Height + _borderTopLeft.Height + _borderBottomRight.Height);

			if (location.X < 0)
			{
				if (Screen.PrimaryScreen.Bounds.Width > location.X + size.Width)
				{
					location.X = location.X + size.Width;
				}
			}
			if (location.Y < 0)
			{
				if (Screen.PrimaryScreen.Bounds.Height > location.Y + size.Height)
				{
					location.Y = location.Y + size.Height;
				}
			}

			_location = location;
		}

		public void Hide()
		{
			_visible = false;
			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | (NativeMethods.SWP_HIDEWINDOW));
			Dispose();

			if (MouseClick != null)
			{
				foreach (MouseEventHandler eh in MouseClick.GetInvocationList())
				{
					MouseClick -= eh;
				}
			}
		}

		protected override void WndProc(ref Message message)
		{
			switch (message.Msg)
			{
				case NativeMethods.WM_NCMOUSEMOVE:
					{
						NativeMethods.TrackNcMouseLeave(this);
						message.Result = IntPtr.Zero;
					}
					break;
				case NativeMethods.WM_NCHITTEST:
					{
						message.Result = (IntPtr)HitResult.Caption;
					}
					break;
                case NativeMethods.WM_KILLFOCUS:
                case NativeMethods.WM_NCMOUSELEAVE:
					{
						Hide();
						message.Result = IntPtr.Zero;
					}
					break;
				case NativeMethods.WM_NCLBUTTONDOWN:
					{
						message.Result = IntPtr.Zero;
					}
					break;
				case NativeMethods.WM_NCLBUTTONUP:
					{
						if (MouseClick != null)
							MouseClick(this, new MouseEventArgs(MouseButtons.Left, 2, NativeMethods.Macros.GET_X_LPARAM(message.LParam) - _location.X, NativeMethods.Macros.GET_Y_LPARAM(message.LParam) - _location.Y, 0));
						message.Result = IntPtr.Zero;
					}
					break;
				default:
					{
						base.WndProc(ref message);
					}
					break;
			}
		}

		public void Redraw()
		{
			if (!_visible)
				return;

			int width = _size.Width;
			int hidth = _size.Height;
			_graphics.Clear(Color.Transparent);
			_graphics.DrawImageUnscaledAndClipped(_borderTopLeft, new Rectangle(0, 0, _borderTopLeft.Width, _borderTopLeft.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderTop, new Rectangle(_borderTopLeft.Width, 0, _background.Width, _borderTop.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderTopRight, new Rectangle(_borderTopLeft.Width + _background.Width, 0, _borderTopRight.Width, _borderTopRight.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderLeft, new Rectangle(0, _borderTopLeft.Height, _borderLeft.Width, _background.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderBottomLeft, new Rectangle(0, _borderTopLeft.Height + _background.Height, _borderBottomLeft.Width, _borderBottomLeft.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderBottom, new Rectangle(_borderTopLeft.Width, _borderTopLeft.Height + _background.Height, _background.Width, _borderBottom.Height));

			_graphics.DrawImageUnscaledAndClipped(_borderRight, new Rectangle(_borderTopLeft.Width + _background.Width, _borderTopLeft.Height, _borderRight.Width, _background.Height));
			_graphics.DrawImageUnscaledAndClipped(_borderBottomRight, new Rectangle(_borderTopLeft.Width + _background.Width, _borderTopLeft.Height + _background.Height, _borderBottomRight.Width, _borderBottomRight.Height));
			_graphics.DrawImageUnscaledAndClipped(_background, new Rectangle(_borderTopLeft.Width, _borderTopLeft.Height, _background.Width, _background.Height));

			if (_imageList != null && _titleList != null && _textList != null && _imageList.Count == _titleList.Count && _imageList.Count == _textList.Count)
			{
				StringFormat sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;
				int imageWidth = 0;
				for (int i = 0; i < _imageList.Count; i++)
				{
					_graphics.DrawString(_titleList[i], _font, _brush, new Rectangle(imageWidth + _borderTopLeft.Width + 5, _borderTopLeft.Height + 5, _imageList[i].Width, _font.Height), sf);
					_graphics.DrawString(_textList[i], _font, _brush, new Rectangle(imageWidth + _borderTopLeft.Width + 5, _borderTopLeft.Height + 5 + _font.Height + 5 + _imageList[i].Height + 5, _imageList[i].Width, _font.Height), sf);
					_graphics.DrawImageUnscaledAndClipped(_imageList[i], new Rectangle(imageWidth + _borderTopLeft.Width + 5, _borderTopLeft.Height + 5 + _font.Height + 5, _imageList[i].Width, _imageList[i].Height));
					imageWidth += _imageList[i].Width + 5;
				}
			}
			else
			{
				int imageWidth = 0;
				if (_image != null)
				{
					imageWidth = _image.Width + 5;
					_graphics.DrawImageUnscaledAndClipped(_image, new Rectangle(_borderTopLeft.Width + 5, _borderTopLeft.Height + 5, _image.Width, _image.Height));
				}

				if (_title != null)
					_graphics.DrawString(_title, _titleFont, _brush, imageWidth + _borderTopLeft.Width + 5, _borderTopLeft.Height + 5);

				if (_text != null)
				{
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Near;
					sf.LineAlignment = StringAlignment.Center;
					int x = imageWidth + _borderTopLeft.Width + 5;
					int y = _borderTopLeft.Height + 5;
					if (_title != null)
						y += 5 + _font.Height;
					_graphics.DrawString(_text, _font, _brush, new Rectangle(x, y, (_background.Width - x), (_background.Height - y)), sf);
				}
			}

			Point pointSource = Point.Empty;
			NativeMethods.BlendFunction blend = NativeMethods.GetBlendFunction(_opacity);
			NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, IntPtr.Zero, ref _size, _handleBitmapDC, ref pointSource, 0, ref blend, NativeMethods.ULW_ALPHA);
			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, _location.X, _location.Y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSENDCHANGING);
		}

		public static Image ResizeImage(Image imgToResize, int y)
		{
			int x = (int)((double)y * (double)((double)imgToResize.Width / (double)imgToResize.Height));
			return new Bitmap(imgToResize, new Size(x, y));
		}

		public event MouseEventHandler MouseClick;

		public virtual void Dispose()
		{
			_graphics.Dispose();
			NativeMethods.DeleteDC(_handleBitmapDC);
		}
	}
}
