using Gadget.Extensions;
using Gadget.Utilities;
using Gadget.Widgets;
using OpenHardwareMonitor.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gadget.Config
{
	public partial class SettingsForm : Form
	{
		private StartupManager _startupManager = new StartupManager();
		private Gadget.Gadget _gadget;
        private List<IWidget> _widgets;

        public SettingsForm(Gadget.Gadget gadget)
		{
			InitializeComponent();

			_gadget = gadget;
            _widgets = gadget.GetWidgets();

            checkBox3.Checked = _startupManager.Startup;
            trackBar1.Value = _gadget.Opacity;
            numericUpDown1.Value = _gadget.UpdateInterval;
            cbText.SelectedIndex = _gadget.TextRenderingHint;
            cbBlur.Checked = _gadget.Blur;
            cbBorderTop.Checked = _gadget.BackgroundBorder[0];
            cbBorderRight.Checked = _gadget.BackgroundBorder[1];
            cbBorderBottom.Checked = _gadget.BackgroundBorder[2];
            cbBorderLeft.Checked = _gadget.BackgroundBorder[3];
            tbBackgroundOpacity.Value = _gadget.BackgroundOpacity;
            pBackgroundColor.BackColor = _gadget.BackgroundColor;

            foreach (var gadgetData in _widgets)
            {
                treeView.Nodes.Add(new GadgetTreeNode(gadgetData));
            }

            treeView.ExpandAll();
		}

		private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e == null || e.Node == null)
				return;

			treeView.SelectedNode = e.Node;

			if (e.Button == MouseButtons.Right && e.Node is GadgetTreeNode)
			{
				treeContextMenu.MenuItems.Clear();
                
				var item1 = new MenuItem("Rename");
				item1.Click += delegate(object obj, EventArgs args)
				{
					e.Node.BeginEdit();
				};
				treeContextMenu.MenuItems.Add(item1);

				var item2 = new MenuItem("Properties");
				item2.Click += delegate(object obj, EventArgs args)
				{
                    if (((GadgetTreeNode)e.Node).Widget.ShowProperties())
                    {
                        _gadget.Redraw();
                    }
				};
				treeContextMenu.MenuItems.Add(item2);

				treeContextMenu.Show(treeView, new Point(e.X, e.Y));
			}
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e == null || e.Node == null)
				return;

			treeView.SelectedNode = e.Node;

			if (e.Node is GadgetTreeNode)
			{
                if (((GadgetTreeNode)e.Node).Widget.ShowProperties())
                {
                    _gadget.Redraw();
                }
			}
		}

		private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
            //if not renamable (currently we alow to rename everything)
            //if (e.Node is GadgetTreeNode && !(((GadgetTreeNode)e.Node).Widget.IsRenameable))
            //{
            //	e.CancelEdit = true;
            //}
        }

        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeView.SelectedNode = (TreeNode)e.Item;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            var name = typeof(GadgetTreeNode).FullName;
            if (e.Data.GetDataPresent(name, false))
            {
                var movingNode = (GadgetTreeNode)e.Data.GetData(name);
                var point = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                var InsertCollection = treeView.Nodes;
                var DestinationNode = ((TreeView)sender).GetNodeAt(point);

                int NodeIndex;
                if (DestinationNode != null)
                {
                    int OffsetY = point.Y - DestinationNode.Bounds.Top;
                    if (OffsetY < (DestinationNode.Bounds.Height / 2))
                    {
                        NodeIndex = DestinationNode.Index;
                    }
                    else
                    {
                        if (InsertCollection.Count > DestinationNode.Index)
                        {
                            NodeIndex = DestinationNode.Index + 1;
                        }
                        else
                        {
                            NodeIndex = DestinationNode.Index;
                        }
                    }
                }
                else
                {
                    NodeIndex = InsertCollection.Count;
                }

                if (InsertCollection != null)
                {
                    var gadgetTreeNode = new GadgetTreeNode(movingNode);
                    InsertCollection.Insert(NodeIndex, gadgetTreeNode);
                    treeView.SelectedNode = InsertCollection[NodeIndex];
                    movingNode.Remove();
                }
            }
        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void btnOK_Click(object sender, EventArgs e)
		{
			ApplyChanges();
			Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			ApplyChanges();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to reset all configuration?", "Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_startupManager.Startup = false;
                _gadget.ResetConfig();

                ApplyChanges();
                Close();
			}
		}

		private void ApplyChanges()
		{
			int i = 0;

			foreach (TreeNode node in treeView.Nodes)
			{
				if (node is GadgetTreeNode)
				{
					var gadgetTreeNode = (GadgetTreeNode)node;
					gadgetTreeNode.Widget.IsVisible = gadgetTreeNode.Checked;
					gadgetTreeNode.Widget.Name = gadgetTreeNode.Text;
					gadgetTreeNode.Widget.Position = i;
					i++;
				}
			}

			_startupManager.Startup = checkBox3.Checked;

            _widgets.SortByPosition();

            _gadget.UpdateInterval = (int)numericUpDown1.Value;
            _gadget.Opacity = (byte)trackBar1.Value;

            _gadget.Blur = cbBlur.Checked;
            _gadget.TextRenderingHint = cbText.SelectedIndex;
            _gadget.BackgroundBorder = new bool[] { cbBorderTop.Checked, cbBorderRight.Checked, cbBorderBottom.Checked, cbBorderLeft.Checked };
            _gadget.BackgroundOpacity = (byte)tbBackgroundOpacity.Value;
            _gadget.BackgroundColor = pBackgroundColor.BackColor;
            _gadget.UpdateBackgroundImage();

            _gadget.Redraw();
		}

        private void btnColor_Click(object sender, EventArgs e)
        {
            pBackgroundColor.BackColor = NativeMethods.GetWindowColorizationColor();
        }

        private void pBackgroundColor_Click(object sender, EventArgs e)
		{
            ColorDialog colorDialog = new ColorDialog();
			colorDialog.Color = pBackgroundColor.BackColor;
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				pBackgroundColor.BackColor = colorDialog.Color;
			}
		}

		private void Settings_FormClosing(object sender, FormClosingEventArgs e)
		{
            _gadget.SaveConfig();

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
    }
}
