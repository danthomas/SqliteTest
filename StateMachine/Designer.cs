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

                richTextBox2.Text = String.Join("", _parser.Tokens.Select(item => item.Text));

                int start = 0;
                foreach (Token token in _parser.Tokens)
                {
                    richTextBox2.SelectionStart = start;
                    richTextBox2.SelectionLength = token.Text.Length;
                    if (token.TokenType == TokenType.Text)
                    {
                        richTextBox2.SelectionBackColor = Color.LightYellow;
                    }
                    else if (token.TokenType == TokenType.Statement)
                    {
                        richTextBox2.SelectionBackColor = Color.LightSkyBlue;
                    }
                    else if (token.TokenType == TokenType.Block)
                    {
                        richTextBox2.SelectionBackColor = Color.LightSalmon;
                    }
                    start += token.Text.Length;
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
