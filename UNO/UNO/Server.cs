using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO
{
    public partial class Server : Form
    {
        private Socket serverSocket;
        private Thread serverThread;
        private bool isServerRunning;
        private int nextClientId = 1;
        int plus = 0;
        

        private Dictionary<string, (string, int)> clientInfo = new Dictionary<string, (string, int)>();
        private Dictionary<Socket, int> clientIds = new Dictionary<Socket, int>();
        private Dictionary<string, bool> clientStatus = new Dictionary<string, bool>();

        private Queue<string> dataQueue = new Queue<string>();

        int playerCount = 0;
        private int[] Idplay = { 1, 2, 3, 4 };
        private int play=0;
        private int[] Idcards = { 0, 0, 0, 0 };

        public Server()
        {
            InitializeComponent();
        }

        private void StartServer()
        {
            if (isServerRunning)
                return;
            GetCard();
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
                serverThread = new Thread(ServerThread);
                serverThread.Start();
                isServerRunning = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }
        
        private void GetCard()
        {
           

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uno.txt");

            // Đọc dữ liệu từ tệp và đưa vào hàng đợi
            dataQueue = ReadFileAndEnqueue(filePath);

            // Xáo trộn các phần tử trong hàng đợi
            ShuffleQueue(dataQueue);
            UpdateAllTextBox();
        }


        // Hàm đọc dữ liệu từ tệp và đưa vào hàng đợi
        static Queue<string> ReadFileAndEnqueue(string filePath)
        {
            Queue<string> dataQueue = new Queue<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Loại bỏ ký tự xuống dòng và chia dữ liệu thành các phần tử
                    string[] elements = line.Split(',');
                    // Đưa các phần tử vào hàng đợi
                    foreach (string element in elements)
                    {
                        dataQueue.Enqueue(element);
                    }
                    
                }
            }
            return dataQueue;
        }


        // Hàm xáo trộn các phần tử trong hàng đợi
        static void ShuffleQueue(Queue<string> queue)
        {
            // Chuyển đổi hàng đợi thành danh sách
            List<string> dataList = new List<string>(queue);
            // Sử dụng thuật toán Fisher-Yates để xáo trộn danh sách
            Random random = new Random();
            int n = dataList.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = dataList[k];
                dataList[k] = dataList[n];
                dataList[n] = value;
            }
            // Xóa tất cả các phần tử khỏi hàng đợi
            queue.Clear();
            // Đưa các phần tử đã được xáo trộn vào lại hàng đợi
            foreach (string element in dataList)
            {
                queue.Enqueue(element);
            }
        }

        private void ServerThread()
        {
            serverSocket.Listen(4); // Số lượng tối đa của các kết nối đang chờ xử lý
            while (isServerRunning)
            {
                Socket clientSocket = serverSocket.Accept();
                Task.Run(() => HandleClientCommunication(clientSocket));
            }
        }

        private async Task HandleClientCommunication(Socket clientSocket)
        {
            using (NetworkStream stream = new NetworkStream(clientSocket))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string ten = ""; // Tên của client
                string mk = ""; // Mật khẩu
                int clientId = 0; // ID của client

                while (isServerRunning)
                {
                    string request = await reader.ReadLineAsync();

                    if (request == null)
                    {
                        SendIdplay();
                    }
                    else if (request.StartsWith("Login:"))
                    {
                        string[] loginInfo = request.Split(':')[1].Trim().Split(',');
                        if (loginInfo.Length >= 2)
                        {
                            ten = loginInfo[0].Trim();
                           
                                
                            mk = loginInfo[1].Trim();
                            if (clientInfo.ContainsKey(ten) && clientInfo[ten].Item1 == mk)
                            {
                                clientId = clientInfo[ten].Item2;
                                clientIds.Add(clientSocket, clientId);
                                SendClientId(clientSocket, 0);
                            }
                            else
                            {
                                clientId = nextClientId++;
                                clientInfo[ten] = (mk, clientId);
                                clientIds.Add(clientSocket, clientId);
                                clientStatus[ten] = true;
                            }
                            SendClientId(clientSocket, clientId);
                            Invoke((Action)(() => Member.AppendText($"Name: {ten}, pass: {mk} has joined the server." + Environment.NewLine)));
                        }
                        else
                        {
                        }
                    }
                    else if (request == "begin")
                    {
                        // Xử lý khi nhận được tin nhắn "begin"
                        for (int i = 0; i < 6; i++)
                            SendUnoCards(clientSocket);
                        playerCount++;
                        if (playerCount >= 4)
                        {
                            // Nếu đủ 4 người chơi, gửi lá đầu tiên cho mỗi người chơi
                            SendUnoCardsTop("");
                            Idcards[0]=6;
                            Idcards[1]=6;
                            Idcards[2]=6;
                            Idcards[3]=6;
                            SendIdplay();
                        }
                    }
                    else if (request == "draw")
                    {
                        
                        Idcards[play]++;
                        if (plus!=0) plus=0;
                        SendIdplay();
                        SendUnoCards(clientSocket);
                        
                    }
                    else if (request.StartsWith("Exit:"))
                    {
                        string[] loginInfo = request.Split(':')[1].Trim().Split(',');
                        if (loginInfo.Length >= 2)
                        {
                            ten = loginInfo[0].Trim();
                            mk = loginInfo[1].Trim();
                            clientStatus[ten] = false;
                            Invoke((Action)(() => Member.AppendText($"Name: {ten}, pass: {mk} has left the server." + Environment.NewLine)));
                        }
                    }
                    else if (request.StartsWith("PlayCard:"))
                    {
                        Idcards[Idplay[play]-1]--;
                        string cards = request.Split(':')[1].Trim();
                        if (cards == "RDP" || cards == "YDP" || cards == "BDP" || cards == "GDP"||cards== "DP")
                        {
                            SendUnoCardsTop(cards);
                            plus+=4;
                            
                            play++;

                            if (play >= 4)
                                play -= 4;
                            SendIdplay();
                            SendPlus(cards);
                        }
                        else if (cards == "RDD" || cards == "YDD" || cards == "BDD" || cards == "GDD"||cards=="DD")
                        {
                           SendUnoCardsTop(cards);
                            
                            play++;
                            if (play >= 4)
                                play -= 4;
                           SendIdplay();
                            plus=0;

                        }
                        else if (cards == "RP" || cards == "YP" || cards == "BP" || cards == "GP")
                        {
                            SendUnoCardsTop(cards);
                            
                            play++;
                            if (play >= 4)
                                play -= 4;
                           SendIdplay();
                            plus+=2;
                            SendPlus(cards);
                        }
                        else
                        {
                            
                            SendUnoCardsTop(cards);
                            plus=0;
                            play++;
                            if (play >= 4)
                                play -= 4;
                             SendIdplay();


                            if (cards == "BC" || cards == "GC" || cards == "YC" || cards == "RC")
                            {
                                play++;
                                if (play >= 4)
                                    play -= 4;
                                SendIdplay();
                            }
                            else if (cards == "BD" || cards == "GD" || cards == "YD" || cards == "RD")
                            {
                                play--;
                                if (play < 0)
                                    play = 3;
                                int y = play-1;
                                if (y < 0)
                                    y=3;
                                int temp = Idplay[(play + 1) % 4];
                                Idplay[(play + 1) % 4] = Idplay[y];
                                Idplay[y] = temp;
                                play++;
                                //1 2 3 4
                                //  2
                                //3 2 1 4
                                SendIdplay();
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        private void SendPlus(string card) 
        {
            foreach (Socket clientSocket in clientIds.Keys)
            {
                try
                {
                    NetworkStream stream = new NetworkStream(clientSocket);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    writer.WriteLine("Plus: " + card+" "+plus.ToString());
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending IDplay message to client: " + ex.Message);
                    // Xử lý ngoại lệ nếu cần
                }
            }
        }
        private void SendIdplay()
        {
            foreach (Socket clientSocket in clientIds.Keys)
            {
                try
                {
                    NetworkStream stream = new NetworkStream(clientSocket);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    writer.WriteLine("IDplay: " + Idplay[play]);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending IDplay message to client: " + ex.Message);
                    // Xử lý ngoại lệ nếu cần
                }
                try
                {
                    NetworkStream stream = new NetworkStream(clientSocket);
                    StreamWriter writer = new StreamWriter(stream);
                    writer.AutoFlush = true;

                    writer.WriteLine("IDcards: " + Idcards[0]+", "+ Idcards[1]+", "+ Idcards[2]+", "+ Idcards[3]);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending IDplay message to client: " + ex.Message);
                    // Xử lý ngoại lệ nếu cần
                }
            }
        }


        private void UpdateAllTextBox()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dataQueue)
            {
                sb.AppendLine(item);
            }
            ALL.Text = sb.ToString();
        }

        private void SendUnoCards(Socket clientSocket)
        {
            NetworkStream stream = new NetworkStream(clientSocket);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            if (dataQueue.Count > 0)
            {
                string card = dataQueue.Dequeue();
                string message = "Card: " + card;
                writer.WriteLine(message);
                writer.Flush();
            }
            else
            {
                string message = "Card: No more Uno cards available.";
                writer.WriteLine(message);
                writer.Flush();
            }
            // UpdateAllTextBox();
        }

        private void SendUnoCardsTop(string cards)
        {
            if (cards == "")
            {
                string card = dataQueue.Peek(); // Lấy lá bài đầu tiên trong hàng đợi mà không xóa nó
                string message = "CardTop: " + card;

                foreach (Socket clientSocket in clientIds.Keys)
                {
                    try
                    {
                        NetworkStream stream = new NetworkStream(clientSocket);
                        StreamWriter writer = new StreamWriter(stream);
                        writer.AutoFlush = true;

                        writer.WriteLine(message);
                        writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending CardTop message to client: " + ex.Message);
                        // Xử lý ngoại lệ nếu cần
                    }
                }
                dataQueue.Dequeue();
            }
            else
            {
                string message = "CardTop: " + cards;

                foreach (Socket clientSocket in clientIds.Keys)
                {
                    try
                    {
                        NetworkStream stream = new NetworkStream(clientSocket);
                        StreamWriter writer = new StreamWriter(stream);
                        writer.AutoFlush = true;

                        writer.WriteLine(message);
                        writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending CardTop message to client: " + ex.Message);
                    }
                }
            }
        }




     

        private void SendClientId(Socket clientSocket, int clientId)
        {
            NetworkStream stream = new NetworkStream(clientSocket);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("Your ID is: " + clientId);
            writer.Flush();
        }


        private void ChatRoomServer_Load(object sender, EventArgs e)
        {
            StartServer();
        }

        private void ChatRoomServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (!isServerRunning)
                return;
            if (serverThread != null && serverThread.IsAlive)
                serverThread.Abort();
            if (serverSocket != null && serverSocket.IsBound)
                serverSocket.Close();
            isServerRunning = false;
        }
    }
}
