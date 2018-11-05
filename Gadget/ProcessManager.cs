using Gadget.Utilities;
using Gadget.Widgets;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Gadget.Gadget
{
    public class ProcessManager
	{
		public static void Process(ClickType clickType, string parameter)
		{
			try
			{
				switch (clickType)
				{
					case ClickType.Process:
						{
                            Process p = new Process();
                            p.StartInfo.FileName = parameter;
                            //p.StartInfo.UseShellExecute = true;
                            //p.StartInfo.LoadUserProfile = false;
                            //p.StartInfo.CreateNoWindow = true;
                            p.Start();
                        }
                        break;
					case ClickType.Internet:
						{
							System.Diagnostics.Process.Start(parameter);
						}
                        break;
					case ClickType.ScreenOff:
						{
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                int WM_SYSCOMMAND = 0x0112;
                                int SC_MONITORPOWER = 0xF170;
                                int HWND_BROADCAST = 0xFFFF;
                                Thread.Sleep(2000);
                                NativeMethods.SendMessage(HWND_BROADCAST, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
                            }).Start();
						}
                        break;
                }
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
