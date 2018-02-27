using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Go
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            GameWindow GW = new GameWindow();
            GW.computerThinkingTimeLimit = (double)thinkingTimeUpDown.Value;
            GW.boardWidth = FiveBoard.Checked ? 5 : (NineBoard.Checked ? 9 : 19);
            GW.starter();
            GW.Show();
            GW.noComputer = checkBox1.Checked;
            if (checkBox1.Checked)
            {
                GW.gameStateList.Enabled = false;
            }
            this.Hide();
            GW.FormClosed += ExitClicked;
        }

        private void ExitClicked(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
