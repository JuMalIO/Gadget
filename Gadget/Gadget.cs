using Gadget.Utilities;
using Gadget.Widgets;
using OpenHardwareMonitor.GUI;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gadget.Gadget
{
	public class Gadget : NativeWindow, IDisposable
    {
        private Computer _computer = new Computer();
        private Config.Config _config = Config.Config.Load();
        private List<IWidget> _widgets = new List<IWidget>();

        private Timer _gadgetTimer = new Timer();
        private Timer _internetTimer = new Timer();
		private UpdateVisitor _updateVisitor = new UpdateVisitor();
		private NotificationIcon _notificationIcon;
		private BackgroundWorker _backgroundWorker = new BackgroundWorker();

		private Bitmap _background;

        private IntPtr LParam;
		private NativeMethods.DWM_BLURBEHIND bb = default(NativeMethods.DWM_BLURBEHIND);

		private IntPtr _handleBitmapDC;
        private Graphics _graphics;
        private Timer timer = new Timer();
        private ToolTip toolTipWindow = new ToolTip();

		public Gadget()
        {
            Type commandType = typeof(Form).Assembly.GetType("System.Windows.Forms.Command");
			MethodInfo commandDispatch = commandType.GetMethod("DispatchID", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { typeof(int) }, null);
			CreateHandle(CreateParams);

			// move window to the bottom
			NativeMethods.MoveWindowToBottom(Handle);

			// prevent window from fading to a glass sheet when peek is invoked
			NativeMethods.PreventFadingToGlass(Handle);

			_handleBitmapDC = NativeMethods.GetBuffer(Size);
			_graphics = NativeMethods.GetGraphics(_handleBitmapDC, TextRenderingHint);

			Initiate();
            UpdateBackgroundImage();
            Redraw();
        }

		private void Initiate()
		{
            _computer.Open();

            _computer.MainboardEnabled = true;
            _computer.CPUEnabled = true;
            _computer.FanControllerEnabled = true;
            _computer.GPUEnabled = true;
            _computer.HDDEnabled = true;
            _computer.RAMEnabled = true;

            _widgets = _config.GetWidgets(_computer);

            _notificationIcon = new NotificationIcon(this);

            //?
            timer.Interval = 300;
			timer.Tick += delegate(object sender, EventArgs e)
			{
				MouseHover(this, new MouseEventArgs(MouseButtons.Left, 2, NativeMethods.Macros.GET_X_LPARAM(LParam) - Location.X, NativeMethods.Macros.GET_Y_LPARAM(LParam) - Location.Y, 0));
				timer.Stop();
			};


			_backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
			{
				foreach (var widget in _widgets)
				{
					if (widget.IsVisible && widget is IWidgetWithInternet)
						((IWidgetWithInternet)widget).UpdateInternet();
				}
			};
			_backgroundWorker.RunWorkerAsync();

			_internetTimer.Interval = 60 * 1000;
			_internetTimer.Tick += delegate(object sender, EventArgs e)
			{
				if (!_backgroundWorker.IsBusy)
				{
					_backgroundWorker.RunWorkerAsync();
				}
			};
			_internetTimer.Start();

			_gadgetTimer.Interval = UpdateInterval * 1000;
			_gadgetTimer.Tick += delegate(object sender, EventArgs e)
			{
				Update();
				Redraw();
			};
			_gadgetTimer.Start();
            
            NativeMethods.BlurBehindWindow(Handle, _graphics, Size, bb, Blur);

			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_SHOWWINDOW);
			if (!AlwaysOnTop)
				ShowDesktop.Instance.ShowDesktopChanged += ShowDesktopChanged;
		}

        private void MouseHover(object sender, MouseEventArgs args)
		{
			int startFromHeight = 3;
			foreach (var widget in _widgets)
			{
				if (widget.IsVisible)
				{
					int height = widget.GetHeight();
					if (args.Y > startFromHeight && args.Y < startFromHeight + height)
					{
						if (widget is IWidgetWithHover && ((IWidgetWithHover)widget).IsHoverable)
                            ((IWidgetWithHover)widget).Hover(toolTipWindow, Location, args.Location, startFromHeight);
						break;
					}
					startFromHeight += height;
				}
			}
			Redraw();
		}

		private void MouseClick(object obj, MouseEventArgs args)
		{
			if (!LockPositionAndSize)
				return;

			int startFromHeight = 3;
			foreach (var widget in _widgets)
			{
				if (widget.IsVisible)
				{
					int height = widget.GetHeight();
					if (args.Y > startFromHeight && args.Y < startFromHeight + height)
					{
						if (widget is IWidgetWithClick && ((IWidgetWithClick)widget).IsClickable)
                            ((IWidgetWithClick)widget).Click(args.Location, startFromHeight);
						break;
					}
					startFromHeight += height;
				}
			}
			Redraw();
		}

        public List<IWidget> GetWidgets()
        {
            return _widgets;
        }

        public void ResetConfig()
        {
            _config = new Config.Config();
            _widgets = _config.GetWidgets(_computer);
        }

        public void SaveConfig()
        {
            Config.Config.Save(_config);
        }

		public void UpdateBackgroundImage()
		{
			_background = new Bitmap(Size.Width, Size.Height);
			using (Graphics g = Graphics.FromImage(_background))
			{
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(BackgroundOpacity, BackgroundColor)))
				{
					g.FillRectangle(brush, 0, 0, _background.Width, _background.Height);
				}
				using (Brush border = new SolidBrush(Color.FromArgb(127, 255, 255, 255)))
				{
					if (BackgroundBorder[0])
						g.DrawLine(new Pen(border, 1), new Point(0, 1), new Point(_background.Width, 1));
					if (BackgroundBorder[1])
						g.DrawLine(new Pen(border, 1), new Point(_background.Width - 2, 0), new Point(_background.Width - 2, _background.Height));
					if (BackgroundBorder[2])
						g.DrawLine(new Pen(border, 1), new Point(0, _background.Height - 2), new Point(_background.Width, _background.Height - 2));
					if (BackgroundBorder[3])
						g.DrawLine(new Pen(border, 1), new Point(1, 0), new Point(1, _background.Height));
				}
				using (Brush border2 = new SolidBrush(Color.FromArgb(127, 0, 0, 0)))
				{
					if (BackgroundBorder[0])
						g.DrawLine(new Pen(border2, 1), new Point(0, 0), new Point(_background.Width, 0));
					if (BackgroundBorder[1])
						g.DrawLine(new Pen(border2, 1), new Point(_background.Width - 1, 0), new Point(_background.Width - 1, _background.Height));
					if (BackgroundBorder[2])
						g.DrawLine(new Pen(border2, 1), new Point(0, _background.Height - 1), new Point(_background.Width, _background.Height - 1));
					if (BackgroundBorder[3])
						g.DrawLine(new Pen(border2, 1), new Point(0, 0), new Point(0, _background.Height));
				}
			}
		}

		private void SizeChanged(object sender, EventArgs e)
		{
			UpdateBackgroundImage();
            DisposeDraw();
			_handleBitmapDC = NativeMethods.GetBuffer(Size);
			_graphics = NativeMethods.GetGraphics(_handleBitmapDC, TextRenderingHint);
			Redraw();

            SaveConfig();
        }

		private void ShowDesktopChanged(bool showDesktop)
		{
			if (showDesktop)
			{
				NativeMethods.MoveWindowToTopMost(Handle);
			}
			else
			{
				NativeMethods.MoveWindowToBottom(Handle);
			}
		}

		private void Update()
		{
			_computer.Accept(_updateVisitor);

			foreach (var widget in _widgets)
			{
				if (widget.IsVisible)
					widget.Update();
			}
		}

		public void Redraw()
		{
			int width = Size.Width;
			int hidth = Size.Height;

			_graphics.Clear(Color.Transparent);
			_graphics.DrawImage(_background, new Rectangle(0, 0, width, hidth));

			if (BackgroundBorder[1] && !BackgroundBorder[3])
				width -= 2;
			if (!BackgroundBorder[1] && BackgroundBorder[3])
				width += 2;
			int height = 5;
			foreach (var widget in _widgets)
			{
                if (widget.IsVisible)
                {
                    widget.Draw(_graphics, width, height);
                    height += widget.GetHeight();
                }
			}

			/*GlassText.DrawText(graphics, "Hello Vista 2013", "Segoe UI", 14, new SolidBrush(Color.White), new Point(5, startFromHeight));
			startFromHeight += 15;*/

			//Rectangle rectangle = new Rectangle(5, 805, 180, 20);
			//GlassText.FillBlackRegion(graphics, rectangle);
			//GlassText.DrawTextOnGlass(handleBitmapDC, "Hello Vista 2013", new Font(new FontFamily("Segoe UI"), 14), Color.White, rectangle, 10);

			Point pointSource = Point.Empty;
			NativeMethods.BlendFunction blend = NativeMethods.GetBlendFunction(Opacity);

			NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, IntPtr.Zero, Size, _handleBitmapDC, ref pointSource, 0, ref blend, NativeMethods.ULW_ALPHA);

			// make sure the window is at the right location
			NativeMethods.SetWindowPos(Handle, IntPtr.Zero, Location.X, Location.Y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSENDCHANGING);
		}

		protected override void WndProc(ref Message message)
		{
			if (LockPositionAndSize)
			{
				switch (message.Msg)
				{
					case NativeMethods.WM_NCRBUTTONDOWN:
						{
							if (!AlwaysOnTop)
								NativeMethods.MoveWindowToBottom(Handle);
							message.Result = IntPtr.Zero;
						} break;
					case NativeMethods.WM_NCRBUTTONUP:
						{
							if (!AlwaysOnTop)
								NativeMethods.MoveWindowToBottom(Handle);
							message.Result = IntPtr.Zero;
						} break;
					case NativeMethods.WM_NCMOUSEMOVE:
						{
							NativeMethods.TrackNcMouseLeave(this);
						} break;
					case NativeMethods.WM_NCHITTEST:
						{
							LParam = message.LParam;
							timer.Stop();
							timer.Start();
							message.Result = (IntPtr)HitResult.Caption;
						} break;
					case NativeMethods.WM_NCMOUSELEAVE:
						{
							//MouseLeave
							timer.Stop();
							message.Result = IntPtr.Zero;
						} break;
					case NativeMethods.WM_NCLBUTTONDOWN:
						{
							//MouseClick
							timer.Stop();
							MouseClick(this, new MouseEventArgs(MouseButtons.Left, 2,
							NativeMethods.Macros.GET_X_LPARAM(message.LParam) - Location.X,
							NativeMethods.Macros.GET_Y_LPARAM(message.LParam) - Location.Y, 0));
							message.Result = IntPtr.Zero;
						} break;
					case NativeMethods.WM_NCLBUTTONUP:
						{
							timer.Stop();
							message.Result = IntPtr.Zero;
						} break;
					default:
						{
							base.WndProc(ref message);
						} break;
				}
			}
			else
			{
				switch (message.Msg)
				{
					case NativeMethods.WM_NCHITTEST:
						{
							//HitTest
							Point p = new Point(NativeMethods.Macros.GET_X_LPARAM(message.LParam) - Location.X, NativeMethods.Macros.GET_Y_LPARAM(message.LParam) - Location.Y);
							HitTestEventArgs e = new HitTestEventArgs(p, HitResult.Caption);
							HitTestEventArgs.HitTest(this, e, LockPositionAndSize, Size);
							message.Result = (IntPtr)e.HitResult;
						}
                        break;
					case NativeMethods.WM_WINDOWPOSCHANGING:
						{
							NativeMethods.WindowPos wp = (NativeMethods.WindowPos)Marshal.PtrToStructure(
							message.LParam, typeof(NativeMethods.WindowPos));

							if (!LockPositionAndSize)
							{
								// prevent the window from leaving the screen
								if ((wp.flags & NativeMethods.SWP_NOMOVE) == 0)
								{
									Rectangle rect = Screen.GetWorkingArea(
									new Rectangle(wp.x, wp.y, wp.cx, wp.cy));
									const int margin = 16;
									wp.x = Math.Max(wp.x, rect.Left - wp.cx + margin);
									wp.x = Math.Min(wp.x, rect.Right - margin);
									wp.y = Math.Max(wp.y, rect.Top - wp.cy + margin);
									wp.y = Math.Min(wp.y, rect.Bottom - margin);
								}

								// update location and fire event
								if ((wp.flags & NativeMethods.SWP_NOMOVE) == 0)
								{
									if (Location.X != wp.x || Location.Y != wp.y)
									{
										if (wp.x < 0)
											wp.x = 0;
										else if (wp.x > Screen.PrimaryScreen.Bounds.Width - wp.cx)
											wp.x = Screen.PrimaryScreen.Bounds.Width - wp.cx;
										Location = new Point(wp.x, wp.y);

                                        SaveConfig();
                                    }
								}

								// update size and fire event
								if ((wp.flags & NativeMethods.SWP_NOSIZE) == 0)
								{
									if (Size.Width != wp.cx || Size.Height != wp.cy)
									{
										Size = new Size(wp.cx, wp.cy);
										//SizeChanged
										SizeChanged(this, EventArgs.Empty);

                                        NativeMethods.BlurBehindWindow(Handle, _graphics, Size, bb, Blur);
									}
								}

								// update the size of the layered window
								if ((wp.flags & NativeMethods.SWP_NOSIZE) == 0)
								{
									NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero,
									    IntPtr.Zero, Size, IntPtr.Zero, IntPtr.Zero, 0,
									    IntPtr.Zero, 0);
								}

								// update the position of the layered window
								if ((wp.flags & NativeMethods.SWP_NOMOVE) == 0)
								{
									NativeMethods.SetWindowPos(Handle, IntPtr.Zero,
									    Location.X, Location.Y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE |
									    NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSENDCHANGING);
								}
							}

							// do not forward any move or size messages
							wp.flags |= NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE;

							// suppress any frame changed events
							wp.flags &= ~NativeMethods.SWP_FRAMECHANGED;

							Marshal.StructureToPtr(wp, message.LParam, false);
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
		}

        #region public properties

        public bool[] BackgroundBorder
        {
            get
            {
                return _config.BackgroundBorder;
            }
            set
            {
                _config.BackgroundBorder = value;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return _config.BackgroundColor;
            }
            set
            {
                _config.BackgroundColor = value;
            }
        }

        public byte BackgroundOpacity
        {
            get
            {
                return _config.BackgroundOpacity;
            }
            set
            {
                _config.BackgroundOpacity = value;
            }
        }

        public bool Blur
        {
            get
            {
                return _config.Blur;
            }
            set
            {
                if (_config.Blur != value)
                {
                    _config.Blur = value;
                    NativeMethods.BlurBehindWindow(Handle, _graphics, Size, bb, value);
                }
            }
        }

        public int UpdateInterval
        {
            get
            {
                return _config.UpdateInterval;
            }
            set
            {
                if (_config.UpdateInterval != value)
                {
                    _config.UpdateInterval = value;

                }
            }
        }

        public int TextRenderingHint
        {
            get
            {
                return _config.TextRenderingHint;
            }
            set
            {
                if (_config.TextRenderingHint != value)
                {
                    _config.TextRenderingHint = value;
                    _graphics.TextRenderingHint = (TextRenderingHint)value;
                }
            }
        }

        public byte Opacity
		{
			get
			{
				return _config.Opacity;
			}
			set
			{
				if (_config.Opacity != value)
				{
                    _config.Opacity = value;
					NativeMethods.BlendFunction blend = NativeMethods.GetBlendFunction(value);
					NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, ref blend, NativeMethods.ULW_ALPHA);
				}
			}
		}

		public bool LockPositionAndSize
		{
			get
			{
				return _config.LockPositionAndSize;
			}
			set
			{
                _config.LockPositionAndSize = value;
			}
		}

		public bool AlwaysOnTop
		{
			get
			{
				return _config.AlwaysOnTop;
			}
			set
			{
				if (_config.AlwaysOnTop != value)
				{
                    _config.AlwaysOnTop = value;
					if (value)
					{
						ShowDesktop.Instance.ShowDesktopChanged -= ShowDesktopChanged;
						NativeMethods.MoveWindowToTopMost(Handle);
					}
					else
					{
						NativeMethods.MoveWindowToBottom(Handle);
						ShowDesktop.Instance.ShowDesktopChanged += ShowDesktopChanged;
					}
				}
			}
		}

		public Size Size
		{
			get
			{
				return _config.Size;
			}
			set
			{
				if (_config.Size != value)
				{
                    _config.Size = value;
					NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, IntPtr.Zero, ref value, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);
					SizeChanged(this, EventArgs.Empty);
				}
			}
		}

		public Point Location
		{
			get
			{
				return _config.Location;
			}
			set
			{
				if (_config.Location != value)
				{
                    _config.Location = value;
					NativeMethods.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSENDCHANGING);
				}
			}
		}

        #endregion

        protected virtual CreateParams CreateParams
		{
			get
			{
				return NativeMethods.GetCreateParams(Location);
			}
		}

        public virtual void DisposeDraw()
        {
            _graphics.Dispose();
            NativeMethods.DeleteDC(_handleBitmapDC);
        }

        public virtual void Dispose()
		{
            DisposeDraw();

            _computer.Close();
        }
	}
}
