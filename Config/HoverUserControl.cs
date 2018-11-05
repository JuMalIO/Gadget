using System.Windows.Forms;

namespace Gadget.Config
{
    public partial class HoverUserControl : UserControl
    {
        public bool IsHoverable
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

        public HoverUserControl(bool isHoverable)
        {
            InitializeComponent();

            IsHoverable = isHoverable;
        }
    }
}
