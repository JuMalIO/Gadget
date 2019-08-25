using System;
using System.Windows.Forms;

namespace Gadget.Gadget
{
    public class ApplicationContext : System.Windows.Forms.ApplicationContext
    {
        private readonly NotificationIcon _notificationIcon;
        private readonly Gadget _gadget;

        public ApplicationContext()
        {
            Application.ApplicationExit += ApplicationExitHandler;

            _gadget = new Gadget();
            _notificationIcon = new NotificationIcon(_gadget);
        }

        private void ApplicationExitHandler(object sender, EventArgs e)
        {
            _notificationIcon.Dispose();
            _gadget.Dispose();
        }
    }
}
