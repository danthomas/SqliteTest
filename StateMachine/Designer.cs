using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StateMachine
{
    public partial class Designer : Form
    {
        private readonly Parser _parser;

        public Designer()
        {
            _parser = new Parser();

            InitializeComponent();

            foreach (StateMachine<Parser>.When_ when in _parser.Whens)
            {
                treeView1.Nodes.Add(new WhenTreeNode(when));
            }
        }

        private void Designer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                _parser.Parse(richTextBox1.Text);

                richTextBox2.Text = "";

                foreach (Token token in _parser.Tokens)
                {
                    int i = richTextBox2.Text.Length;
                    richTextBox2.Text += token.Text;
                    richTextBox2.Select(i, token.Text.Length);
                    richTextBox2.SelectionBackColor = token.TokenType == TokenType.Text ? Color.White : Color.LightSkyBlue;
                }
            }
        }
    }

    public class WhenTreeNode : TreeNode
    {
        public StateMachine<Parser>.When_ When { get; set; }

        public WhenTreeNode(StateMachine<Parser>.When_ when)
        {
            When = when;
            Text = "When";
        }

        public void RefreshText()
        {
            Text = "When " + (When.IsTrue ? " true" : "false");
        }
    }
}
