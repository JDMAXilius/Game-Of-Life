using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPortfolio
{
    public partial class Sed_Dialog : Form
    {
        public Sed_Dialog()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        public decimal RandomSeed
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Random rng = new Random();
            RandomSeed = rng.Next(int.MinValue, int.MaxValue);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
