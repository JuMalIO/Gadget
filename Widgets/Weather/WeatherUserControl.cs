using System.Windows.Forms;

namespace Gadget.Widgets.Weather
{
    public partial class WeatherUserControl : UserControl
    {
        public string Url
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

        public WeatherUserControl(string url)
        {
            InitializeComponent();

            Url = url;
        }
    }
}
