using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Gadget.Widgets.Currency
{
    public partial class CurrencyUserControl : UserControl
    {
        public bool CurrencyShort
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

        public int Type
        {
            get
            {
                return comboBox3.SelectedIndex;
            }
            private set
            {
                comboBox3.SelectedIndex = value;
            }
        }

        public List<string> VisibleCurrency
        {
            get
            {
                return treeView.Nodes.Cast<TreeNode>().Where(i => i.Checked).Select(i => i.Name).ToList();
            }
            private set
            {
                foreach (TreeNode treeNode in treeView.Nodes)
                {
                    treeNode.Checked = value.Contains(treeNode.Name);
                }
            }
        }

        public CurrencyUserControl(List<CurrencyData> currencyDataList, List<string> visibleCurrency, bool currencyShort, int type)
        {
            InitializeComponent();

            foreach (var currencyData in currencyDataList)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = currencyData.Currency;
                treeNode.Name = currencyData.CurrencyShort;
                treeNode.Checked = currencyData.Visible;
                treeView.Nodes.Add(treeNode);
            }

            VisibleCurrency = visibleCurrency;
            CurrencyShort = currencyShort;
            Type = type;
        }
    }
}
