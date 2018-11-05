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

            Gadget.Gadget gadget = new Gadget.Gadget();
            Application.Run();
        }
    }
}
