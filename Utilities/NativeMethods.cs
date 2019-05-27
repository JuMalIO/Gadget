using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gadget.Utilities
{
	public static class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct MARGINS
		{
			public int Left;
			public int Right;
			public int Top;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DWM_BLURBEHIND
		{
			public int dwFlags;
			public bool fEnable;
			public IntPtr hRgnBlur;
			public bool fTransitionOnMaximized;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BlendFunction
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct WindowPos
		{
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct BitmapInfo
		{
			public Int32 Size;
			public Int32 Width;
			public Int32 Height;
			public Int16 Planes;
			public Int16 BitCount;
			public Int32 Compression;
			public Int32 SizeImage;
			public Int32 XPelsPerMeter;
			public Int32 YPelsPerMeter;
			public Int32 ClrUsed;
			public Int32 ClrImportant;
			public Int32 Colors;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class TRACKMOUSEEVENT
		{
			public uint cbSize;
			public uint dwFlags;
			public IntPtr hwndTrack;
			public uint dwHoverTime;
		}

		public enum WindowAttribute : int
		{
			DWMWA_NCRENDERING_ENABLED = 1,
			DWMWA_NCRENDERING_POLICY,
			DWMWA_TRANSITIONS_FORCEDISABLED,
			DWMWA_ALLOW_NCPAINT,
			DWMWA_CAPTION_BUTTON_BOUNDS,
			DWMWA_NONCLIENT_RTL_LAYOUT,
			DWMWA_FORCE_ICONIC_REPRESENTATION,
			DWMWA_FLIP3D_POLICY,
			DWMWA_EXTENDED_FRAME_BOUNDS,
			DWMWA_HAS_ICONIC_BITMAP,
			DWMWA_DISALLOW_PEEK,
			DWMWA_EXCLUDED_FROM_PEEK,
			DWMWA_LAST
		}

		private const int BI_RGB = 0;
		private const int DIB_RGB_COLORS = 0;

		public const int TOP_BORDER = 6;
		public const int BOTTOM_BORDER = 7;
		public const int LEFT_BORDER = 6;
		public const int RIGHT_BORDER = 7;

		public const int TME_LEAVE = 0x00000002;
		public const int TME_NONCLIENT = 0x00000010;

		public const int DWM_BB_ENABLE = 0x1;
		public const int DWM_BB_BLURREGION = 0x2;
		public const int DWM_BB_TRANSITIONONMAXIMIZED = 0x4;

		public const int WS_EX_LAYERED = 0x00080000;
		public const int WS_EX_TOOLWINDOW = 0x00000080;

		public const uint SWP_NOSIZE = 0x0001;
		public const uint SWP_NOMOVE = 0x0002;
		public const uint SWP_NOACTIVATE = 0x0010;
		public const uint SWP_FRAMECHANGED = 0x0020;
		public const uint SWP_HIDEWINDOW = 0x0080;
		public const uint SWP_SHOWWINDOW = 0x0040;
		public const uint SWP_NOZORDER = 0x0004;
		public const uint SWP_NOSENDCHANGING = 0x0400;

		public const int ULW_COLORKEY = 0x00000001;
		public const int ULW_ALPHA = 0x00000002;
		public const int ULW_OPAQUE = 0x00000004;

		public const byte AC_SRC_OVER = 0x00;
		public const byte AC_SRC_ALPHA = 0x01;

		public const int WM_NCHITTEST = 0x0084;
		public const int WM_NCLBUTTONDBLCLK = 0x00A3;
		public const int WM_NCLBUTTONDOWN = 0x00A1;
		public const int WM_NCLBUTTONUP = 0x00A2;
		public const int WM_NCRBUTTONDOWN = 0x00A4;
		public const int WM_NCRBUTTONUP = 0x00A5;
		public const int WM_WINDOWPOSCHANGING = 0x0046;
		public const int WM_COMMAND = 0x0111;

        public const int WM_KILLFOCUS = 0x8;
        public const int WM_NCMOUSELEAVE = 0x02A2;
		public const int WM_NCMOUSEMOVE = 0xA0;

		public const int TPM_RIGHTBUTTON = 0x0002;
		public const int TPM_VERTICAL = 0x0040;

		public static readonly IntPtr HWND_BOTTOM = (IntPtr)1;
		public static readonly IntPtr HWND_TOPMOST = (IntPtr)(-1);

		private const string USER = "user32.dll";
		private const string GDI = "gdi32.dll";
		private const string DWMAPI = "dwmapi.dll";


		[DllImport(USER)]
		public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetShellWindow();

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

		[DllImport(DWMAPI, PreserveSig = false)]
		public static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, ref Size psize, IntPtr hdcSrc, IntPtr pprSrc, int crKey, IntPtr pblend, int dwFlags);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, int crKey, ref BlendFunction pblend, int dwFlags);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pprSrc, int crKey, ref BlendFunction pblend, int dwFlags);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport(USER, CallingConvention = CallingConvention.Winapi)]
		public static extern bool TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr tpmParams);

		[DllImport(GDI, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		[DllImport(GDI, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitmapInfo pbmi, uint pila, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

		[DllImport(GDI, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport(GDI, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		[DllImport(GDI, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport(DWMAPI, CallingConvention = CallingConvention.Winapi)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, WindowAttribute dwAttribute, ref bool pvAttribute, int cbAttribute);

		[DllImport(DWMAPI, PreserveSig = false)]
		public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

		[DllImport(DWMAPI)]
		public extern static int DwmIsCompositionEnabled(ref int en);

		[DllImport(USER)]
		public static extern bool TrackMouseEvent([In, Out] TRACKMOUSEEVENT lpEventTrack);

		public static void TrackNcMouseLeave(NativeWindow nativeWindow)
		{
			TRACKMOUSEEVENT tme = new TRACKMOUSEEVENT();
			tme.cbSize = (uint)Marshal.SizeOf(tme);
			tme.dwFlags = TME_LEAVE | TME_NONCLIENT;
			tme.hwndTrack = nativeWindow.Handle;
			TrackMouseEvent(tme);
		}

		public static CreateParams GetCreateParams(Point location)
		{
			CreateParams createParams = new CreateParams();
			createParams.Width = 4096;
			createParams.Height = 4096;
			createParams.X = location.X;
			createParams.Y = location.Y;
			createParams.ExStyle = WS_EX_TOOLWINDOW | WS_EX_LAYERED;
			return createParams;
		}

		public static BlendFunction GetBlendFunction(byte opacity)
		{
			BlendFunction blend = new BlendFunction();
			blend.BlendOp = AC_SRC_OVER;
			blend.BlendFlags = 0;
			blend.SourceConstantAlpha = opacity;
			blend.AlphaFormat = AC_SRC_ALPHA;
			return blend;
		}

        public static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, Size psize, IntPtr hdcSrc, ref Point pprSrc, int crKey, ref BlendFunction pblend, int dwFlags)
        {
            return UpdateLayeredWindow(hwnd, hdcDst, pptDst, ref psize, hdcSrc, ref pprSrc, crKey, ref pblend, dwFlags);
        }

        public static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, IntPtr pptDst, Size psize, IntPtr hdcSrc, IntPtr pprSrc, int crKey, IntPtr pblend, int dwFlags)
        {
            return UpdateLayeredWindow(hwnd, hdcDst, pptDst, ref psize, hdcSrc, pprSrc, crKey, pblend, dwFlags);
        }

        public static void PreventFadingToGlass(IntPtr handle)
		{
			try
			{
				bool value = true;
				DwmSetWindowAttribute(handle, WindowAttribute.DWMWA_EXCLUDED_FROM_PEEK, ref value, Marshal.SizeOf(value));
            }
			catch (DllNotFoundException)
			{
			}
			catch (EntryPointNotFoundException)
			{
			}
		}

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_INVALID_STATE = 5
        }

        [DllImport(USER)]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public static void BlurBehindWindow(IntPtr handle, Graphics graphics, Size size, DWM_BLURBEHIND bb, bool enable)
		{
            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor < 2)
            {
                Rectangle glassRect = new Rectangle(0, 0, size.Width, size.Height);
                using (Region rgn = new Region(glassRect))
                {
                    bb.dwFlags = (enable ? DWM_BB_ENABLE : 0) | DWM_BB_BLURREGION;
                    bb.fEnable = true;
                    bb.hRgnBlur = rgn.GetHrgn(graphics);
                    DwmEnableBlurBehindWindow(handle, ref bb);
                }
            }
            else
            {
                var accent = new AccentPolicy();
                accent.AccentState = enable ? AccentState.ACCENT_ENABLE_BLURBEHIND : AccentState.ACCENT_DISABLED;

                var accentStructSize = Marshal.SizeOf(accent);

                var accentPtr = Marshal.AllocHGlobal(accentStructSize);
                Marshal.StructureToPtr(accent, accentPtr, false);

                var data = new WindowCompositionAttributeData();
                data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
                data.SizeOfData = accentStructSize;
                data.Data = accentPtr;

                SetWindowCompositionAttribute(handle, ref data);

                Marshal.FreeHGlobal(accentPtr);
            }
        }

		public static IntPtr GetBuffer(Size size)
		{
			IntPtr handleScreenDC = NativeMethods.GetDC(IntPtr.Zero);
			IntPtr handleBitmapDC = NativeMethods.CreateCompatibleDC(handleScreenDC);
			NativeMethods.ReleaseDC(IntPtr.Zero, handleScreenDC);

			NativeMethods.BitmapInfo info = new NativeMethods.BitmapInfo();
			info.Size = Marshal.SizeOf(info);
			info.Width = size.Width;
			info.Height = -size.Height;
			info.BitCount = 32;
			info.Planes = 1;
			info.Compression = BI_RGB;

			IntPtr ptr;
			IntPtr hBmp = NativeMethods.CreateDIBSection(handleBitmapDC, ref info, 0, out ptr, IntPtr.Zero, 0);
			IntPtr hBmpOld = NativeMethods.SelectObject(handleBitmapDC, hBmp);
			NativeMethods.DeleteObject(hBmpOld);

			return handleBitmapDC;
		}

		public static Graphics GetGraphics(IntPtr handleBitmapDC, int textRenderingHint)
		{
			Graphics graphics = Graphics.FromHdc(handleBitmapDC);
			if (Environment.OSVersion.Version.Major > 5)
			{
				graphics.TextRenderingHint = (System.Drawing.Text.TextRenderingHint)textRenderingHint;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			}
			return graphics;
		}

		public static void MoveWindowToBottom(IntPtr handle)
		{
			SetWindowPos(handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOSENDCHANGING);
		}

		public static void MoveWindowToTopMost(IntPtr handle)
		{
			SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOSENDCHANGING);
		}

		public static class Macros
		{
			public static ushort LOWORD(IntPtr l)
			{
				return (ushort)((ulong)l & 0xFFFF);
			}

			public static UInt16 HIWORD(IntPtr l)
			{
				return (ushort)(((ulong)l >> 16) & 0xFFFF);
			}

			public static int GET_X_LPARAM(IntPtr lp)
			{
				return (short)LOWORD(lp);
			}

			public static int GET_Y_LPARAM(IntPtr lp)
			{
				return (short)HIWORD(lp);
			}
		}

        [DllImport(DWMAPI, EntryPoint = "#127")]
        private static extern void DwmGetColorizationParameters(out DWM_COLORIZATION_PARAMS parameters);

        public struct DWM_COLORIZATION_PARAMS
        {
            public uint ColorizationColor,
                ColorizationAfterglow,
                ColorizationColorBalance,
                ColorizationAfterglowBalance,
                ColorizationBlurBalance,
                ColorizationGlassReflectionIntensity,
                ColorizationOpaqueBlend;
        }

        public static Color GetWindowColorizationColor(bool opaque = true)
        {
            DWM_COLORIZATION_PARAMS colors;
            DwmGetColorizationParameters(out colors);

            return Color.FromArgb((byte)(opaque ? 255 : colors.ColorizationColor >> 24),
                (byte)(colors.ColorizationColor >> 16),
                (byte)(colors.ColorizationColor >> 8),
                (byte)colors.ColorizationColor);
        }
    }

	public enum HitResult
	{
		Transparent = -1,
		Nowhere = 0,
		Client = 1,
		Caption = 2,
		Left = 10,
		Right = 11,
		Top = 12,
		TopLeft = 13,
		TopRight = 14,
		Bottom = 15,
		BottomLeft = 16,
		BottomRight = 17,
		Border = 18
	}

	public class HitTestEventArgs : EventArgs
	{
		public HitTestEventArgs(Point location, HitResult hitResult)
		{
			Location = location;
			HitResult = hitResult;
		}

		public static void HitTest(object sender, HitTestEventArgs e, bool lockPositionAndSize, Size size)
		{
			if (lockPositionAndSize)
				return;
			if (e.Location.X < NativeMethods.LEFT_BORDER)
			{
				e.HitResult = HitResult.Left;
				return;
			}
			if (e.Location.X > size.Width - 1 - NativeMethods.RIGHT_BORDER)
			{
				e.HitResult = HitResult.Right;
				return;
			}

			if (e.Location.Y < NativeMethods.TOP_BORDER)
			{
				e.HitResult = HitResult.Top;
				return;
			}
			if (e.Location.Y > size.Height - 1 - NativeMethods.BOTTOM_BORDER)
			{
				e.HitResult = HitResult.Bottom;
				return;
			}
		}

		public Point Location { get; private set; }

		public HitResult HitResult { get; set; }
	}
}
