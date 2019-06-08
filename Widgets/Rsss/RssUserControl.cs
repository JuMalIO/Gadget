using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Widgets.Rss
{
    public partial class RssUserControl : UserControl
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

        public int MaxTitles
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

        public List<string> RssLinks
        {
            get
            {
                return textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
            private set
            {
                textBox1.Text = string.Join(Environment.NewLine, value);
            }
        }

        public RssUserControl(Color color, int maxTitles, List<string> rssLinks)
        {
            InitializeComponent();

            Color = color;
            MaxTitles = maxTitles;
            RssLinks = rssLinks;
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
