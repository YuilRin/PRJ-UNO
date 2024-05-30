﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace UNO_V2
{
    public class Room
    {
        public string RoomName { get; set; }
        public int beg = 0;
        public Dictionary<Socket, int> ClientIds = new Dictionary<Socket, int>();
        public Dictionary<int, string> PlayerNames { get; set; }
        public Queue<string> DataQueue { get; private set; }
        public Queue<string> DataQueue2 { get; }
        public int PlayerCount = 0;
        public int[] Idplay { get; }
        public int Play { get; set; }
        public int[] Idcards { get; }
        public int Plus { get; set; }

        public Room(string roomName)
        {
            RoomName = roomName;
            ClientIds = new Dictionary<Socket, int>();
            PlayerNames = new Dictionary<int, string>();
            DataQueue = new Queue<string>();
            DataQueue2 = new Queue<string>();
            Idplay = new int[] { 1, 2, 3, 4 };
            Idcards = new int[] { 0, 0, 0, 0 };
            InitializeCardQueue();
        }

        private void InitializeCardQueue()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uno.txt");

            // Read data from file and enqueue
            DataQueue = ReadFileAndEnqueue(filePath);

            // Shuffle the queue elements
            ShuffleQueue(DataQueue);
        }

        // Method to read data from file and enqueue
        private static Queue<string> ReadFileAndEnqueue(string filePath)
        {
            Queue<string> dataQueue = new Queue<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Remove newline character and split data into elements
                    string[] elements = line.Split(',');
                    // Enqueue elements
                    foreach (string element in elements)
                    {
                        dataQueue.Enqueue(element);
                    }
                }
            }
            return dataQueue;
        }

        // Method to shuffle queue elements
        private static void ShuffleQueue(Queue<string> queue)
        {
            // Convert queue to list
            List<string> dataList = new List<string>(queue);
            // Use Fisher-Yates algorithm to shuffle list
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
            // Clear the queue
            queue.Clear();
            // Enqueue shuffled elements back to the queue
            foreach (string element in dataList)
            {
                queue.Enqueue(element);
            }
        }
    }

    public class Server2
    {
        private Dictionary<string, Room> rooms;
        private Socket serverSocket;
        private Thread serverThread;
        private bool isServerRunning;
        private int nextClientId;
        private int nextRoomId;
        private Dictionary<string, (string password, int clientId)> clientInfo;
        private Dictionary<string, bool> clientStatus;

        public Server2()
        {
            rooms = new Dictionary<string, Room>();
            nextClientId = 1; // Initial client ID
            nextRoomId = 1;   // Initial room ID
            clientInfo = new Dictionary<string, (string password, int clientId)>();
            clientStatus = new Dictionary<string, bool>();
        }

        public void StartServer()
        {
            // Create a socket object
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to a specific IP address and port
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));

            // Start listening for incoming connections
            serverSocket.Listen(4);

            isServerRunning = true;
            serverThread = new Thread(ServerThread);
            serverThread.Start();
        }

        private void ServerThread()
        {
            while (isServerRunning)
            {
                try
                {
                    Socket clientSocket = serverSocket.Accept();
                    Task.Run(() => HandleClientCommunication(clientSocket));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error accepting client: " + ex.Message);
                }
            }
        }

        private async Task HandleClientCommunication(Socket clientSocket)
        {
            try
            {
                using (NetworkStream stream = new NetworkStream(clientSocket))
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string playerName = "";
                    string password = "";
                    int clientId = 0;

                    while (isServerRunning)
                    {
                        string request = await reader.ReadLineAsync();
                        if (request == null)
                        {
                           // SendIdplay();
                        }
                        else if (request.StartsWith("Name:"))
                        {
                            playerName = request.Substring(5).Trim();
                            bool isNameTaken = rooms.Values.Any(room => room.PlayerNames.Values.Contains(playerName));

                            if (isNameTaken)
                            {
                                await writer.WriteLineAsync("IsOnl");
                                await writer.FlushAsync();
                            }
                            else
                            {
                                //clientId = nextClientId++;
                                //AddPlayerToRoom(playerName, clientId, clientSocket);

                                await writer.WriteLineAsync("LoginSuccessful");
                                await writer.FlushAsync();
                            }
                        }
                        else if (request.StartsWith("Login:"))
                        {
                           
                            string[] loginInfo = request.Split(':')[1].Trim().Split(',');
                            if (loginInfo.Length >= 2)
                            {
                                playerName = loginInfo[0].Trim();
                                password = loginInfo[1].Trim();
                                

                                if (clientInfo.ContainsKey(playerName) && clientInfo[playerName].password == password)
                                {
                                    clientId = clientInfo[playerName].clientId;
                                    AddClientToRoom(clientSocket, clientId);
                                    SendClientId(writer, 0);
                                }
                                else
                                {
                                    if(clientId==4)
                                        nextClientId = 0;
                                    clientId = nextClientId++;
                                    clientInfo[playerName] = (password, clientId);
                                    clientStatus[playerName] = true;
                                    AddClientToRoom(clientSocket, clientId);
                                    SendClientId(writer, clientId);
                                }

                                Console.WriteLine($"Name: {playerName}, pass: {password} has joined the server.");
                            }
                        }
                        else if (request == "begin")
                        {
                            // Handle "begin" request
                            Room room = FindRoomByClientSocket(clientSocket);
                            if (room != null)
                            {
                                for (int i = 0; i < 6; i++)
                                    await SendUnoCards(writer, room);
                                if (room.beg==0&&room.PlayerCount == 4)
                                {
                                    room.beg=1;
                                    SendUnoCardsTop(room, "");
                                    for (int i = 0; i < 4; i++)
                                        room.Idcards[i] += 6;
                                    SendIdplay(room);
                                }

                                // MessageBox.Show(room.RoomName+room.PlayerCount);

                            }
                        }
                        else if (request == "draw")
                        {
                            // Handle "draw" request
                            Room room = FindRoomByClientSocket(clientSocket);
                            if (room != null)
                            {
                                room.Idcards[room.Idplay[room.Play] - 1]++;
                                if (room.Plus != 0) room.Plus = 0;
                                SendIdplay( room);
                                await SendUnoCards(writer, room);
                            }
                        }
                        else if (request.StartsWith("Exit:"))
                        {
                            string[] loginInfo = request.Split(':')[1].Trim().Split(',');
                            if (loginInfo.Length >= 2)
                            {
                                playerName = loginInfo[0].Trim();
                                password = loginInfo[1].Trim();
                                clientStatus[playerName] = false;
                                Console.WriteLine($"Name: {playerName}, pass: {password} has left the server.");
                            }
                        }
                        else if (request.StartsWith("PlayCard:"))
                        {
                            Room room = FindRoomByClientSocket(clientSocket);
                            if (room != null)
                            {
                                room.Idcards[room.Idplay[room.Play] - 1]--;
                                SendIdplay(room);

                                string cards = request.Split(':')[1].Trim();
                                await HandlePlayCard(writer, room, cards);
                            }
                        }
                        else
                        {
                            // Handle other requests
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling client communication: " + ex.Message);
            }
            finally
            {
                clientSocket.Close();
            }
        }
        private void AddClientToRoom(Socket clientSocket, int clientId)
        {
            Room room = FindAvailableRoom();
            room.ClientIds[clientSocket] = clientId;
            room.PlayerNames[clientId] = clientId.ToString();
            room.PlayerCount++;
        }
        private void AddPlayerToRoom(string playerName, int clientId, Socket clientSocket)
        {
            Room availableRoom = FindAvailableRoom();
            availableRoom.PlayerNames[clientId] = playerName;
            availableRoom.ClientIds[clientSocket] = clientId;
            availableRoom.PlayerCount++;
           
        }

        

        private Room FindAvailableRoom()
        {
            Room availableRoom = rooms.Values.FirstOrDefault(room => room.PlayerCount < 4);
            if (availableRoom == null)
            {
                string roomName = "Room" + nextRoomId++;
                availableRoom = new Room(roomName);
                rooms.Add(roomName, availableRoom);
            }
            return availableRoom;
        }

        private Room FindRoomByClientSocket(Socket clientSocket)
        {
            return rooms.Values.FirstOrDefault(room => room.ClientIds.ContainsKey(clientSocket));
        }

        private async Task SendClientId(StreamWriter writer, int clientId)
        {
            await writer.WriteLineAsync("Your ID is:" + clientId);
            
            await writer.FlushAsync();
        }

        private void SendIdplay(Room room)
        {
            foreach (Socket clientSocket in room.ClientIds.Keys)
            {
                try
                {
                    using (NetworkStream stream = new NetworkStream(clientSocket))
                    using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                    {
                        // Send current player's ID
                        writer.WriteLine("IDplay: " + room.Idplay[room.Play]);
                        writer.Flush();

                        // Send current cards count
                        writer.WriteLine("IDcards: " + string.Join(", ", room.Idcards));
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending IDplay or IDcards message to client: " + ex.Message);
                    // Handle the exception if needed
                }
            }
        }



        private async Task SendUnoCards(StreamWriter writer, Room room)
        {
            if (room.DataQueue.Count == 0)
            {
                RefillAndShuffleQueue(room);
            }

            if (room.DataQueue.Count > 0)
            {
                string card = room.DataQueue.Dequeue();
                string message = "Card: " + card;
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            else
            {
                string message = "Card: No more Uno cards available.";
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
        }

        private void RefillAndShuffleQueue(Room room)
        {
            while (room.DataQueue2.Count > 0)
            {
                room.DataQueue.Enqueue(room.DataQueue2.Dequeue());
            }
            ShuffleQueue(room.DataQueue);
        }

        // Assuming ShuffleQueue remains the same
        private static void ShuffleQueue(Queue<string> queue)
        {
            // Convert queue to list
            List<string> dataList = new List<string>(queue);
            // Use Fisher-Yates algorithm to shuffle list
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
            // Clear the queue
            queue.Clear();
            // Enqueue shuffled elements back to the queue
            foreach (string element in dataList)
            {
                queue.Enqueue(element);
            }
        }

        private void SendUnoCardsTop(Room room, string cards)
        {
            
            if (string.IsNullOrEmpty(cards))
            {
                if (room.DataQueue.Count > 0)
                {
                    string card = room.DataQueue.Peek(); // Peek the top card without removing it
                    string message = "CardTop: " + card;

                    foreach (Socket clientSocket in room.ClientIds.Keys)
                    {
                        try
                        {
                            NetworkStream stream = new NetworkStream(clientSocket);
                            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                            writer.WriteLine(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error sending CardTop message to client: " + ex.Message);
                            // Handle the exception if needed
                        }
                    }
                    room.DataQueue.Dequeue(); // Remove the top card after sending it
                }
                else
                {
                    Console.WriteLine("Error: DataQueue is empty.");
                }
            }
            else
            {
                string message = "CardTop: " + cards;

                foreach (Socket clientSocket in room.ClientIds.Keys)
                {
                    try
                    {
                        NetworkStream stream = new NetworkStream(clientSocket);
                        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                        writer.WriteLine(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending CardTop message to client: " + ex.Message);
                    }
                }
            }
        }


        private async Task HandlePlayCard(StreamWriter writer, Room room, string card)
        {
            room.DataQueue2.Enqueue(card);
            SendUnoCardsTop(room, card);

            if (card == "RDP" || card == "YDP" || card == "BDP" || card == "GDP" || card == "DP")
            {
                room.Plus += 4;
                room.Play = (room.Play + 1) % 4;
                SendIdplay(room);
                SendPlus(room, card);
            }
            else if (card == "RDD" || card == "YDD" || card == "BDD" || card == "GDD" || card == "DD")
            {
                room.Plus = 0;
                room.Play = (room.Play + 1) % 4;
                SendIdplay(room);
            }
            else if (card == "RP" || card == "YP" || card == "BP" || card == "GP")
            {
                room.Plus += 2;
                room.Play = (room.Play + 1) % 4;
                SendIdplay(room);
                SendPlus( room, card);
            }
            else
            {
                room.Plus = 0;
                room.Play = (room.Play + 1) % 4;
                SendIdplay(room);

                if (card == "BC" || card == "GC" || card == "YC" || card == "RC")
                {
                    room.Play = (room.Play + 1) % 4;
                    SendIdplay(room);
                }
                else if (card == "BD" || card == "GD" || card == "YD" || card == "RD")
                {
                    room.Play = (room.Play - 1 + 4) % 4;
                    int y = (room.Play - 1 + 4) % 4;
                    int temp = room.Idplay[(room.Play + 1) % 4];
                    room.Idplay[(room.Play + 1) % 4] = room.Idplay[y];
                    room.Idplay[y] = temp;
                    room.Play = (room.Play + 1) % 4;
                    SendIdplay(room);
                }
            }
        }

        private void SendPlus(Room room, string card)
        {

            foreach (Socket clientSocket in room.ClientIds.Keys)
            {
                try
                {
                    NetworkStream stream = new NetworkStream(clientSocket);
                    StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                    // Placeholder: Implement actual logic to send Plus
                    writer.WriteLine("Plus: " + card+" "+room.Plus.ToString());
                    //MessageBox.Show("Plus: " + card+" "+room.Plus.ToString());
                    writer.FlushAsync();
                }
                catch { }
            }
            
        }
    }
}