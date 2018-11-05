using System;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Widgets.Time
{
	public partial class AlarmForm : Form
	{
		public int Hour
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

		public int Minute
        {
            get
            {
                return (int)numericUpDown2.Value;
            }
            private set
            {
                numericUpDown2.Value = value;
            }
        }

        public AlarmType AlarmType
        {
            get
            {
                return (AlarmType)comboBox1.SelectedIndex;
            }
            private set
            {
                comboBox1.SelectedIndex = (int)value;
            }
        }

        public TimeType TimeType
        {
            get
            {
                return (TimeType)comboBox2.SelectedIndex;
            }
            private set
            {
                comboBox2.SelectedIndex = (int)value;
            }
        }

        public bool AlarmRepeat
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

        public bool Enable
        {
            get
            {
                return btnDisable.Enabled;
            }
            private set
            {
                btnDisable.Enabled = value;
            }
        }
        
        public AlarmForm(int hour, int minute, AlarmType alarmType, TimeType timeType, bool alarmRepeat, bool enable)
		{
			InitializeComponent();

            comboBox1.Items.AddRange(Enum.GetValues(typeof(AlarmType)).Cast<AlarmType>().Select(i => i.ToString()).ToArray());
            comboBox2.Items.AddRange(Enum.GetValues(typeof(TimeType)).Cast<TimeType>().Select(i => i.ToString()).ToArray());

            Hour = hour;
			Minute = minute;
			AlarmType = alarmType;
			TimeType = timeType;
			Enable = enable;
			AlarmRepeat = alarmRepeat;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Enable = true;

			DialogResult = DialogResult.OK;
			Close();
		}
		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnDisable_Click(object sender, EventArgs e)
		{
			Enable = false;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
