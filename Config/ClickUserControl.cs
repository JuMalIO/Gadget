using Gadget.Gadget;
using Gadget.Widgets;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Config
{
    public partial class ClickUserControl : UserControl
    {
        public bool IsClickable
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

        public ClickType ClickType
        {
            get
            {
                return (ClickType)comboBox1.SelectedIndex;
            }
            private set
            {
                comboBox1.SelectedIndex = (int)value;
            }
        }

        public string ClickParameter
        {
            get
            {
                return textBox1.Text;
            }
            private set
            {
                textBox1.Text = value;
            }
        }

        public ClickUserControl(bool isClickable, ClickType clickType, string clickParameter)
        {
            InitializeComponent();

            comboBox1.Items.AddRange(Enum.GetValues(typeof(ClickType)).Cast<ClickType>().Where(x => x != ClickType.Disabled).Select(x => x.ToString()).ToArray());

            if (clickType == ClickType.Disabled)
            {
                label1.Visible = false;
                comboBox1.Visible = false;
                label2.Visible = false;
                textBox1.Visible = false;
                groupBox1.Size = new Size(groupBox1.Size.Width, groupBox1.Size.Height - 59);
                Size = new Size(Size.Width, Size.Height - 59);
            }

            IsClickable = isClickable;
            ClickType = clickType;
            ClickParameter = clickParameter;
        }
    }
}
