using Gadget.Config;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Gadget.Gadget
{
    public class NotificationIcon : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        public NotificationIcon(Gadget gadget)
        {
            _notifyIcon = new NotifyIcon
            {
                Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title,
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Visible = true,
                ContextMenu = GetContextMenu(gadget)
            };
        }

        private ContextMenu GetContextMenu(Gadget gadget)
        {
            var lockPositionAndSizeMenuItem = new MenuItem("Lock position and size");
            lockPositionAndSizeMenuItem.Checked = gadget.LockPositionAndSize;
            lockPositionAndSizeMenuItem.Click += delegate (object sender, EventArgs e)
            {
                lockPositionAndSizeMenuItem.Checked = !lockPositionAndSizeMenuItem.Checked;
                gadget.LockPositionAndSize = lockPositionAndSizeMenuItem.Checked;

                gadget.SaveConfig();
            };

            var alwaysOnTopMenuItem = new MenuItem("Always on top");
            alwaysOnTopMenuItem.Checked = gadget.AlwaysOnTop;
            alwaysOnTopMenuItem.Click += delegate (object sender, EventArgs e)
            {
                alwaysOnTopMenuItem.Checked = !alwaysOnTopMenuItem.Checked;
                gadget.AlwaysOnTop = alwaysOnTopMenuItem.Checked;

                gadget.SaveConfig();
            };

            var settingsMenuItem = new MenuItem("Settings");
            settingsMenuItem.Click += delegate (object sender, EventArgs e)
            {
                var settingsForm = new SettingsForm(gadget);
                settingsForm.ShowDialog();
            };

            var aboutMenuItem = new MenuItem("About");
            aboutMenuItem.Click += delegate (object sender, EventArgs e)
            {
                MessageBox.Show("Gadget App by Hotter v0.5", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Click += delegate (object sender, EventArgs e)
            {
                Application.Exit();
            };

            return new ContextMenu(new MenuItem[]
            {
                lockPositionAndSizeMenuItem,
                alwaysOnTopMenuItem,
                settingsMenuItem,
                aboutMenuItem,
                exitMenuItem
            });
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}
