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


namespace UNO
{
    public partial class Player : Form
    {
        private Socket client;
        private NetworkStream networkStream;

        private StreamReader reader;
        private StreamWriter writer;
        private int clientId;
        private string ten;
        private string mk;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private Dictionary<string, Image> cardImages = new Dictionary<string, Image>();
        private string cardTop;
        private List<string> card = new List<string>();
        private int Idplay;
        public Player()
        {
            InitializeComponent();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            LoadCardImages();
            IpServer.Text="127.0.0.1";
            foreach (Control control in this.Controls)
            {
                control.Visible = false;
            }
            name.Visible=true;          txtFullName.Visible=true;
            phone.Visible=true;         txtPass.Visible=true;
            Login.Visible=true;
        }


        private bool isConnected = false;
        // Hàm load ảnh từ thư mục và thêm vào tập hợp cardImages
        private void LoadCardImages()
        {
            try
            {
             
                string currentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

                // Xây dựng đường dẫn đầy đủ đến thư mục chứa các tệp ảnh
                string imageFolderPath = Path.Combine(currentDirectory, "anh");

                DirectoryInfo directoryInfo = new DirectoryInfo(imageFolderPath);
                foreach (var file in directoryInfo.GetFiles())
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
                    if (!cardImages.ContainsKey(fileNameWithoutExtension))
                    {
                        cardImages.Add(fileNameWithoutExtension, Image.FromFile(file.FullName));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading card images: " + ex.Message);
            }
        }

        // Hàm hiển thị thẻ bài trên PictureBox
        private void DisplayCard(string cardName)
        {
            try
            {
                if (cardImages.ContainsKey(cardName))
                {
                    pictureBox7.Image = cardImages[cardName];
                }
                else
                {
                    MessageBox.Show("No image found for card: " + cardName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying card image: " + ex.Message);
            }
        }

        // Hàm xử lý khi nhận được thẻ từ server
        private async Task HandleCardReceived(string cardName)
        {
            // Hiển thị thẻ bài trên PictureBox
            DisplayCard(cardName);
        }

        private async void Chat_Load(object sender, EventArgs e)
        {

            try
            {
                await ConnectToServer();
                await ReceiveMessagesAndUpdateChat();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text;
            string pass = txtPass.Text;
            foreach (Control control in this.Controls)
            {
                control.Visible = true;
            }

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ họ tên và số điện thoại.");
            }
            else
            {
                try
                {
                    ten = fullName;
                    mk = pass;
                    txtFullName.Visible = false;
                    txtPass.Visible = false;
                    name.Visible = false;
                    phone.Visible = false;
                   
                    textBox1.Text = ten;
                    textBox2.Text = mk;
                    Login.Visible = false;


                    // Gửi thông tin đăng nhập lên server
                    await writer.WriteLineAsync($"Login: {ten}, {mk}");
                    await writer.FlushAsync();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending login information to server: " + ex.Message);
                }
            }
        }

        private async Task ConnectToServer()
        {
            try
            {
                string t = IpServer.Text;
                IpServer.ReadOnly= false;
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(IPAddress.Parse(t), 8080); // Thay đổi địa chỉ IP và cổng nếu cần
                networkStream = new NetworkStream(client);
                reader = new StreamReader(networkStream);
                writer = new StreamWriter(networkStream) { AutoFlush = true };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }
       
        private async Task ReceiveMessagesAndUpdateChat()
        {
            try
            {
                while (true)
                {
                    string messagee = await reader.ReadLineAsync();
                  
                    if (messagee == null)
                    {
                        // Đóng kết nối và thoát vòng lặp nếu không còn tin nhắn từ server
                        MessageBox.Show("Disconnected from server.");
                        break;
                    }


                    // Kiểm tra nếu tin nhắn chứa danh sách client đang online

                    if (messagee.StartsWith("Your ID is:"))
                    {
                        clientId = int.Parse(messagee.Split(':')[1].Trim());
                        UpdateClientIdLabel(clientId);
                    }
                    else if (messagee.StartsWith("IDplay: "))
                    {
                        Idplay = int.Parse(messagee.Split(':')[1].Trim());
                        playid.Text=Idplay.ToString();
                    }

                    else if (messagee.StartsWith("Card:"))
                    {
                        string cardName = messagee.Split(':')[1].Trim();
                        //textBox3.AppendText(cardName + Environment.NewLine);
                        pictureBox8.Image = cardImages[cardName];
                    
                    // await HandleCardReceived(cardName);
                    card.Add(cardName);
                        card.Sort();
                    
                        DisplayFirstSixCards();
                    }
                   else if (messagee.StartsWith("CardTop: "))
                    {
                         cardTop = messagee.Split(':')[1].Trim();
                       
                        await HandleCardReceived(cardTop);
                      
                    }
                    else if (messagee.StartsWith("Plus: "))
                    {
                        // Phân tích tin nhắn để lấy thông tin tên, loại và số của lá bài
                        string[] parts = messagee.Split(' ');
                        if(Idplay==clientId)
                        {
                               string card = parts[1];
                        plus = int.Parse(parts[2]);
                        PlusTable.Text=plus.ToString();
                        }    
                                             
                    }
                    else if(messagee.StartsWith("IDcards: "))
                    {
                        string[] IDcards = messagee.Split(':')[1].Trim().Split(',');
                        player1.Text= IDcards[0]; 
                        player2.Text= IDcards[1];
                        player3.Text= IDcards[2];
                        player4.Text= IDcards[3];   
                    }
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Disconnected from server.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error receiving messages from server: " + ex.Message);
            }
        }
        int plus = 0;
        private async Task HandleDrawRequest(int count)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    // Gửi yêu cầu "draw" đến server
                    writer.WriteLine("draw");
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending 'draw' request to server: " + ex.Message);
            }
           if(plus!=0) plus--;
           PlusTable.Text=plus.ToString();
        }
        private void UpdateChat(string message)
        {
            // Hiển thị tin nhắn từ server trên textboxChat
          
        }

        private void UpdateClientIdLabel(int clientId)
        {
            // Cập nhật giao diện người dùng với ID mới
            labelClientId.Text = "Your ID: " + clientId.ToString();
        }

       

        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (writer != null)
            {
                writer.WriteLineAsync($"Exit: {ten}, {mk}");
                writer.Flush();
            }
            // Đóng kết nối khi đóng form
            if (client != null)
                client.Close();
            cancellationTokenSource.Cancel();
        }

        private void begin_Click(object sender, EventArgs e)
        {
            try
            {
                // Gửi yêu cầu "begin" đến server
                writer.WriteLine("begin");
                
                writer.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending 'begin' request to server: " + ex.Message);
            }
            
        }
        private int currentIndex = 0;
        private void DisplayFirstSixCards()
        {
            for (int i = 0; i < 6; i++)
            {
                if (i < card.Count)
                {
                    PictureBox pictureBox = Controls.Find("pictureBox" + (i + 1), true)[0] as PictureBox;
                    if (pictureBox != null)
                    {
                        DisplayCard(card[i], pictureBox);
                    }
                }
            }
        }

        // Hàm hiển thị lá bài trên PictureBox
        private void DisplayCard(string cardName, PictureBox pictureBox)
        {
            try
            {
                if (cardImages.ContainsKey(cardName))
                {
                    pictureBox.Image = cardImages[cardName];
                }
                else
                {
                    MessageBox.Show("No image found for card: " + cardName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying card image: " + ex.Message);
            }
        }

        // Xử lý khi bấm nút Next
        private void NextButton_Click(object sender, EventArgs e)
        {
            currentIndex += 6;
            if (currentIndex >= card.Count)
            {
                // Trường hợp không còn đủ 6 lá bài để hiển thị
                currentIndex -= 6;
                return;
            }
            else
            UpdatePictureBoxes();
        }
        /// <summary>
        /// 0 1 2 3 4 5
        /// 6 7 8
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        // Xử lý khi bấm nút Back
        private void BackButton_Click(object sender, EventArgs e)
        {
            currentIndex -= 6;
            if (currentIndex < 0)
            {
                // Trường hợp không còn đủ 6 lá bài để hiển thị
                currentIndex = 0;
            }
            else
            UpdatePictureBoxes();
        }

        // Hàm cập nhật hình ảnh trên PictureBox
        private void UpdatePictureBoxes()
        {
            for (int i = 0; i < 6; i++)
            {
                PictureBox pictureBox = Controls.Find("pictureBox" +(i + 1), true)[0] as PictureBox;
                if (pictureBox != null)
                {
                    if (currentIndex + i < card.Count)
                    {
                        DisplayCard(card[currentIndex + i], pictureBox);
                    }
                    else
                    {
                        // Trường hợp không còn đủ 6 lá bài để hiển thị
                        pictureBox.Image = null;
                    }
                }
            }
        }

        private async void drawButton_Click(object sender, EventArgs e)
        {
            await HandleDrawRequest(1);
        }
        private void CardButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                // Xác định vị trí của button trong danh sách
                int index = int.Parse(button.Name.Replace("button", "")) - 1+currentIndex;
                string selectedCard = card[index];
                if (IsPlayable(selectedCard, cardTop))
                    // Xác định lá bài tương ứng
                {
                    
                    // Xóa lá bài khỏi danh sách
                    card.RemoveAt(index);

                    // Gửi lá bài lên server
                    SendCardToServer(selectedCard);
                }
                else
                {
                    MessageBox.Show("You cannot play this card."+clientId+Idplay);
                }

                UpdatePictureBoxes();
            }
        }
        private async Task SendCardToServer(string cardName)
        {
            try
            {

                // Gửi thông tin lá bài và màu đã chọn lên server
                if (IsPlayable(cardName, cardTop))
                {
                    await writer.WriteLineAsync($"PlayCard: {cardName}");
                    await writer.FlushAsync();
                    plus=0;
                    PlusTable.Text=plus.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending card information to server: " + ex.Message);
            }
        }

        private bool IsPlayable(string selectedCard, string cardTop)
        {
            // Extract the color and number from the selected card and card on top
            string selectedColor = selectedCard.Substring(0, 1);
            string selectedNumber = selectedCard.Substring(1);
            string topColor = cardTop.Substring(0, 1);
            string topNumber = cardTop.Substring(1);
            if (plus!=0)
                return ((selectedColor == topColor&&selectedNumber=="P")||(selectedNumber=="P")||selectedCard=="DP");
            // Check if the selected card matches the color or number of the top card, or if it's a special card (DD or DF)
            return (selectedColor == topColor || selectedNumber == topNumber || (selectedCard == "DD"&&topNumber!="P") || selectedCard == "DP" ||
            cardTop == "DD" || cardTop == "DP")&&(clientId==Idplay);
        }
    }
}
