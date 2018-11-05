using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Config
{
    public partial class TextUserControl : UserControl
    {
        public Color Color
        {
            get
            {
                return panel1.BackColor;
            }
            private set
            {
                panel1.BackColor = value;
            }
        }

        public string FontName
        {
            get
            {
                return comboBox1.Text;
            }
            private set
            {
                comboBox1.Text = value;
            }
        }

        public int FontSize
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

        public TextUserControl(Color color, string fontName, int fontSize)
        {
            InitializeComponent();

            comboBox1.Items.AddRange(FontFamily.Families.Select(x => x.Name).ToArray());

            Color = color;
            FontName = fontName;
            FontSize = fontSize;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            colorDialog.Color = panel1.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = colorDialog.Color;
            }
        }
    }
}
