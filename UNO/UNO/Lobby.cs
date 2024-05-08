using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO
{
    public partial class Lobby : Form
    {
        public Lobby lobby;
        public List<System.Windows.Forms.Label> PlayerName = new List<System.Windows.Forms.Label>();
        public List<PictureBox> PlayerIcon = new List<PictureBox>();
        public int connectedPlayer = 0;
        public Lobby()
        {
            InitializeComponent();
            // No need for using safe call delegate, ignore cross thread calls
            CheckForIllegalCrossThreadCalls = false;
            //
            lobby = this;
            btnStart.Visible = false;

            PlayerName.Add(labelP1);
            PlayerName.Add(labelP2);
            PlayerName.Add(labelP3);
            PlayerName.Add(labelP4);

            PlayerIcon.Add(pictureBoxP1);
            PlayerIcon.Add(pictureBoxP2);
            PlayerIcon.Add(pictureBoxP3);
            PlayerIcon.Add(pictureBoxP4);
        }

        public void ShowStartButton()
        {
            btnStart.Visible = true;
        }

        // Just a test - to be removed later !
        public void Tempdisplay(string msg)
        {
            richTextBox1.Text += msg + '\n';
        }

        public void DisplayConnectedPlayer(string name)
        {
            connectedPlayer++;

            switch (connectedPlayer)
            {
                case 1:
                    labelP1.Text = name;
                    break;
                case 2:
                    labelP2.Text = name;
                    break;
                case 3:
                    labelP3.Text = name;
                    break;
                case 4:
                    labelP4.Text = name;
                    break;
                default:
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
       //     ClientSocket.datatype = "START";
        //    ClientSocket.SendMessage("");
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
