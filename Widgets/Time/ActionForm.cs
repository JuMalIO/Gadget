using System;
using System.Diagnostics;
using System.Media;
using System.Windows.Forms;

namespace Gadget.Widgets.Time
{
	public partial class ActionForm : Form
	{
		private SoundPlayer _soundPlayer = new SoundPlayer(@"c:\Windows\Media\chimes.wav");
        private AlarmType _alarmType;
        private int _timeLeft;

		public ActionForm(AlarmType alarmType)
		{
			InitializeComponent();

			_alarmType = alarmType;

			if (_alarmType == AlarmType.Alarm)
				_timeLeft = 61;
			else
				_timeLeft = 21;

			timer1_Tick(null, null);
			timer1.Start();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			timer1.Stop();
			DoAction();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			_soundPlayer.Stop();
			timer1.Stop();

			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void DoAction()
		{
            if (_alarmType == AlarmType.Alarm)
			{
				_soundPlayer.Stop();
			}
            else if (_alarmType == AlarmType.Shutdown)
            {
                Process.Start("shutdown", "/s /f /t 0");
            }
            else if (_alarmType == AlarmType.Restart)
            {
                Process.Start("shutdown", "/r /f /t 0");
            }
            else if (_alarmType == AlarmType.Hibernate)
            {
                Process.Start("shutdown", "/h /f");
            }

            DialogResult = DialogResult.OK;
			Close();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (_timeLeft < 0)
            {
                timer1.Stop();
                DoAction();
			}
			else
			{
				if (_alarmType == AlarmType.Alarm)
				{
					_soundPlayer.Play();
					label1.Text = "Alarm! Date and time now is: " + DateTime.Now.ToString("yyyy.MM.dd HH:mm");
				}
				else
				{
					label1.Text = "Your computer is going to " + _alarmType.ToString().ToLower() + " in: " + _timeLeft;
				}
				_timeLeft--;
			}
		}
	}
}
