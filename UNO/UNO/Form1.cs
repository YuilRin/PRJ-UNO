using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UNO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Player a= new Player();
            a.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Server a= new Server(); 
            a.Show();
            button2.Visible=false;
        }
    }
}
