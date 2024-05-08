using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


namespace UNO
{
    public partial class Begin : Form
    {
        public Begin()
        {
            InitializeComponent();
        }
        public static Lobby lobby;
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // TODO: Adding checking for texbox's emptyness, legal values ...
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(textBoxIP.Text), 11000);
          //  ClientSocket.datatype = "CONNECT";
          //  ClientSocket.Connect(serverEP);
            lobby = new Lobby();
          ///  ClientSocket.SendMessage(textBoxName.Text);

            ThisPlayer.name = textBoxName.Text;

       //lobby.FormClosed += new FormClosedEventHandler(lobby_FormClosed);
            lobby.ShowStartButton();
            this.Hide();
            lobby.Show();
        }
    }
}
