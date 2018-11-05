using System.Windows.Forms;

namespace Gadget.Config
{
    public partial class InternetUserControl : UserControl
    {
        public int UpdateInternetInterval
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
            private set
            {
                numericUpDown1.Value = value;
            }
        }

        public InternetUserControl(int updateInternetInterval)
        {
            InitializeComponent();

            UpdateInternetInterval = updateInternetInterval;
        }
    }
}
