namespace Gadget.Widgets.Date
{
	partial class CalendarForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblYear = new System.Windows.Forms.Label();
            this.pYear = new System.Windows.Forms.Panel();
            this.pbCalendar = new System.Windows.Forms.PictureBox();
            this.pYear.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCalendar)).BeginInit();
            this.SuspendLayout();
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear.Location = new System.Drawing.Point(331, 0);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(60, 26);
            this.lblYear.TabIndex = 1;
            this.lblYear.Text = "2013";
            this.lblYear.Click += new System.EventHandler(this.lblYear_Click);
            // 
            // pYear
            // 
            this.pYear.BackColor = System.Drawing.Color.White;
            this.pYear.Controls.Add(this.lblYear);
            this.pYear.Dock = System.Windows.Forms.DockStyle.Top;
            this.pYear.Location = new System.Drawing.Point(0, 0);
            this.pYear.Name = "pYear";
            this.pYear.Size = new System.Drawing.Size(726, 28);
            this.pYear.TabIndex = 2;
            // 
            // pbCalendar
            // 
            this.pbCalendar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCalendar.BackColor = System.Drawing.Color.White;
            this.pbCalendar.Location = new System.Drawing.Point(-2, 25);
            this.pbCalendar.Name = "pbCalendar";
            this.pbCalendar.Size = new System.Drawing.Size(728, 466);
            this.pbCalendar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbCalendar.TabIndex = 3;
            this.pbCalendar.TabStop = false;
            this.pbCalendar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbCalendar_MouseClick);
            // 
            // CalendarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 491);
            this.Controls.Add(this.pbCalendar);
            this.Controls.Add(this.pYear);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalendarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calendar";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalendarForm_FormClosing);
            this.pYear.ResumeLayout(false);
            this.pYear.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCalendar)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label lblYear;
		private System.Windows.Forms.Panel pYear;
		private System.Windows.Forms.PictureBox pbCalendar;
    }
}