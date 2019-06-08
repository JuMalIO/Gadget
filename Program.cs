using System;
using System.Windows.Forms;

namespace Gadget
{
	static class Program
	{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var gadget = new Gadget.Gadget();
            Application.Run();
        }
    }
}
