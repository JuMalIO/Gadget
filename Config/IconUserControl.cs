using System.Windows.Forms;

namespace Gadget.Config
{
    public partial class IconUserControl : UserControl
    {
        public bool IsIconVisible
        {
            get
            {
                return checkBox1.Checked;
            }
            private set
            {
                checkBox1.Checked = value;
            }
        }

        public IconUserControl(bool isIconVisible)
        {
            InitializeComponent();

            IsIconVisible = isIconVisible;
        }
    }
}
