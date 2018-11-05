using Gadget.Config;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Gadget
{
    public class NotificationIcon
    {
        NotifyIcon _notifyIcon = new NotifyIcon();

        public NotificationIcon(Gadget gadget)
        {
            _notifyIcon.Text = "Gadget";
            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _notifyIcon.Visible = true;

            var contextMenu = new ContextMenu();

            var lockItem = new MenuItem("Lock position and size");
            lockItem.Checked = gadget.LockPositionAndSize;
            lockItem.Click += delegate (object sender, EventArgs e)
            {
                lockItem.Checked = !lockItem.Checked;
                gadget.LockPositionAndSize = lockItem.Checked;

                gadget.SaveConfig();
            };
            contextMenu.MenuItems.Add(lockItem);

            var alwaysOnTopItem = new MenuItem("Always on top");
            alwaysOnTopItem.Checked = gadget.AlwaysOnTop;
            alwaysOnTopItem.Click += delegate (object sender, EventArgs e)
            {
                alwaysOnTopItem.Checked = !alwaysOnTopItem.Checked;
                gadget.AlwaysOnTop = alwaysOnTopItem.Checked;

                gadget.SaveConfig();
            };
            contextMenu.MenuItems.Add(alwaysOnTopItem);

            var settingsMenu = new MenuItem("Settings");
            settingsMenu.Click += delegate (object sender, EventArgs e)
            {
                var settingsForm = new SettingsForm(gadget);
                settingsForm.ShowDialog();
            };
            contextMenu.MenuItems.Add(settingsMenu);

            var aboutMenu = new MenuItem("About");
            aboutMenu.Click += delegate (object sender, EventArgs e)
            {
                MessageBox.Show("Gadget App by Hotter v0.5", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            contextMenu.MenuItems.Add(aboutMenu);

            var exitMenu = new MenuItem("Exit");
            exitMenu.Click += delegate (object sender, EventArgs e)
            {
                Application.Exit();
            };
            contextMenu.MenuItems.Add(exitMenu);

            _notifyIcon.ContextMenu = contextMenu;
        }
    }
}
