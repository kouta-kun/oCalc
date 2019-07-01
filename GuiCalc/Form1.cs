using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oCalc;

namespace GuiCalc
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                List<DataGridViewRow> list = dataGridView1.ToList().Where(x => x.Cells[0].Value != null).ToList();
                var variables = list.ToDictionary(x => ((string)x.Cells[0].Value)[0], x => ParseExpression.Parse((string)x.Cells[1].Value, null));
                var bindings = variables.ToDictionary(x => x.Key, x => (IExpression<double>)new Variable(x.Key, variables));
                var expr = ParseExpression.Parse(textBox1.Text, bindings);
                listBox1.Items.Add(textBox1.Text + ": " + expr.Evaluate());
                textBox1.Clear();
            }
        }

        private TreeNode NodeFromExpr(IExpression<double> expr)
        {
            if (expr is BinaryOp b)
            {
                var pr = new TreeNode("Binary expression");
                var tn = new TreeNode("Pre-manipulation: " + Enum.GetName(typeof(Op), b.Operation));
                pr.Nodes.Add(tn);
                tn.Nodes.Add(NodeFromExpr(b.lhs));
                tn.Nodes.Add(NodeFromExpr(b.rhs));
                b.Evaluate();
                tn = new TreeNode("Post-manipulation: " + Enum.GetName(typeof(Op), b.Operation));
                pr.Nodes.Add(tn);
                tn.Nodes.Add(NodeFromExpr(b.lhs));
                tn.Nodes.Add(NodeFromExpr(b.rhs));
                return pr;
            }
            else
            {
                var tn = new TreeNode(expr.GetType().Name + ": " + expr.Evaluate());
                return tn;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            List<DataGridViewRow> list = dataGridView1.ToList().Where(x => x.Cells[0].Value != null).ToList();
            var variables = list.ToDictionary(x => ((string)x.Cells[0].Value)[0], x => ParseExpression.Parse((string)x.Cells[1].Value, null));
            var bindings = variables.ToDictionary(x => x.Key, x => (IExpression<double>)new Variable(x.Key, variables));
            var expr = ParseExpression.Parse((string)listBox1.SelectedItem.ToString().Split(':')[0], bindings);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(NodeFromExpr(expr));
        }
    }
    public static class Extend
    {
        public static List<DataGridViewRow> ToList(this DataGridView dt)
        {
            List<DataGridViewRow> drl = new List<DataGridViewRow>();
            foreach (DataGridViewRow dr in dt.Rows)
            {
                drl.Add(dr);
            }
            return drl;
        }
    }
}
