using Gadget.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Config
{
	public partial class PropertiesForm : Form
    {
        private IWidget _widget;

        public PropertiesForm(IWidget widget, UserControl[] additionalUserControls = null)
        {
            InitializeComponent();

            _widget = widget;

            var userControls = new List<UserControl>();

            if (_widget is IWidgetWithText)
            {
                var widgetWith = (IWidgetWithText)_widget;
                userControls.Add(new TextUserControl(widgetWith.Color, widgetWith.FontName, widgetWith.FontSize));
            }

            if (_widget is IWidgetWithIcon)
            {
                var widgetWith = (IWidgetWithIcon)_widget;
                userControls.Add(new IconUserControl(widgetWith.IsIconVisible));
            }

            if (_widget is IWidgetWithClick)
            {
                var widgetWith = (IWidgetWithClick)_widget;
                userControls.Add(new ClickUserControl(widgetWith.IsClickable, widgetWith.ClickType, widgetWith.ClickParameter));
            }

            if (_widget is IWidgetWithHover)
            {
                var widgetWith = (IWidgetWithHover)_widget;
                userControls.Add(new HoverUserControl(widgetWith.IsHoverable));
            }

            if (_widget is IWidgetWithInternet)
            {
                var widgetWith = (IWidgetWithInternet)_widget;
                userControls.Add(new InternetUserControl(widgetWith.UpdateInternetInterval));
            }

            if (additionalUserControls != null)
            {
                userControls.AddRange(additionalUserControls);
            }

            AddUserControls(userControls.ToArray());
        }

        public T GetControl<T>()
        {
            return Controls.OfType<T>().First();
        }

        private void AddUserControls(UserControl[] userControls)
        {
            var y = 9;
            var x = 9;

            foreach (var userControl in userControls)
            {
                userControl.Location = new Point(x, y);
                y += userControl.Size.Height;
                Controls.Add(userControl);
            }

            Size = new Size(Size.Width, Size.Height + y);
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_widget is IWidgetWithText)
            {
                var widgetWith = (IWidgetWithText)_widget;
                var userControl = GetControl<TextUserControl>();
                widgetWith.Color = userControl.Color;
                widgetWith.FontName = userControl.FontName;
                widgetWith.FontSize = userControl.FontSize;
            }

            if (_widget is IWidgetWithIcon)
            {
                var widgetWith = (IWidgetWithIcon)_widget;
                var userControl = GetControl<IconUserControl>();
                widgetWith.IsIconVisible = userControl.IsIconVisible;
            }

            if (_widget is IWidgetWithClick)
            {
                var widgetWith = (IWidgetWithClick)_widget;
                var userControl = GetControl<ClickUserControl>();
                widgetWith.IsClickable = userControl.IsClickable;
                widgetWith.ClickType = userControl.ClickType;
                widgetWith.ClickParameter = userControl.ClickParameter;
            }

            if (_widget is IWidgetWithHover)
            {
                var widgetWith = (IWidgetWithHover)_widget;
                var userControl = GetControl<HoverUserControl>();
                widgetWith.IsHoverable = userControl.IsHoverable;
            }

            if (_widget is IWidgetWithInternet)
            {
                var widgetWith = (IWidgetWithInternet)_widget;
                var userControl = GetControl<InternetUserControl>();
                widgetWith.UpdateInternetInterval = userControl.UpdateInternetInterval;
            }

            DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
