using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing;
using System.Reflection;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Media;



namespace UNO_V2
{
    public partial class Login : Form
    {
        private Socket client;
        private NetworkStream networkStream;

        private StreamReader reader;
        private StreamWriter writer;
        public Login()
        {
            InitializeComponent();

        }

        private async void Login_Click(object sender, EventArgs e)
        {
            try
            {
                await ConnectToServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }
        private async Task ConnectToServer()
        {
            try
            {
                string t = "127.0.0.1";
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(IPAddress.Parse(t), 50000); // Thay đổi địa chỉ IP và cổng nếu cần
                networkStream = new NetworkStream(client);
                reader = new StreamReader(networkStream);
                writer = new StreamWriter(networkStream) { AutoFlush = true };
                writer.WriteLine("Name: "+NameTb);
                
                int i = 1;
                while (i==1)
                {
                    string messagee = await reader.ReadLineAsync();
                    if (messagee=="IsOnl")
                    {
                        MessageBox.Show("Đã có player cùng tên trong game");
                        this.Close();
                        i=0;
                    }
                    if (messagee=="LoginSuccessful")
                    {
                        playerV2 a = new playerV2(NameTb.Text);
                        a.Show();
                        
                        i=0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }


    }
   
}
