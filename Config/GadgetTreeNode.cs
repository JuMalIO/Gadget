using Gadget.Widgets;
using System.Windows.Forms;

namespace Gadget.Config
{
    public class GadgetTreeNode : TreeNode
    {
        public IWidget Widget { get; set; }

        public GadgetTreeNode(IWidget widget)
        {
            Text = widget.Name;
            Checked = widget.IsVisible;
            Widget = widget;
        }

        public GadgetTreeNode(GadgetTreeNode gadgetTreeNode)
        {
            Text = gadgetTreeNode.Text;
            Checked = gadgetTreeNode.Checked;
            Widget = gadgetTreeNode.Widget;
        }
    }
}
