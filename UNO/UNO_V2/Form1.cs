namespace UNO_V2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void server2_Click(object sender, EventArgs e)
        {
            Server2 server = new Server2();
            server.StartServer();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            Login a = new Login();
            //playerV2 a = new playerV2();
            a.Show();
        }
    }
}
