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


namespace UNO_V2
{
    public partial class playerV2 : Form
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
        public string IpServer = "127.0.0.1";
        private int plus = 0;
        bool isBegin = false;
        public playerV2(string namet)
        {
            InitializeComponent();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            LoadCardImages();
            NameBox.Text = namet;
            foreach (Control ctr in this.Controls)
            {
                if (ctr is PictureBox)
                {
                    PictureBox pic = (PictureBox)ctr;
                    pic.SizeMode =PictureBoxSizeMode.Zoom;
                }
            }
        }

        private bool isConnected = false;
        // Hàm load ảnh từ thư mục và thêm vào tập hợp cardImages
        private void LoadCardImages()
        {
            try
            {

                string currentDirectory = Path.GetDirectoryName(Application.ExecutablePath);

                // Xây dựng đường dẫn đầy đủ đến thư mục chứa các tệp ảnh
                string imageFolderPath = Path.Combine(currentDirectory, "Resources");
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
                    if(isBegin==false)
                    {
                        foreach (Control ctr in this.Controls)
                        {
                            if (ctr is Button)
                            {
                                ctr.Visible = true;
                            }

                        }
                        draw.Visible=true;
                        Sort.Visible=true;
                        back.Visible=true;
                        Next.Visible=true;
                    }
                    TopCard.Image = cardImages[cardName];
                   
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
        private void HandleCardReceived(string cards)
        {
            IDcard.Text=cards;
            if (cards == "RDP" || cards == "YDP" || cards == "BDP" || cards == "GDP"||cards== "DP")
            {
                DisplayCard("DP");
                if (cards=="BDP")
                    Blue.Visible = true;
                if (cards=="GDP")
                    Green.Visible = true;
                if (cards=="RDP")
                    Red.Visible = true;
                if (cards=="YDP")
                    Yellow.Visible = true;
            }
            else if (cards == "RDD" || cards == "YDD" || cards == "BDD" || cards == "GDD"||cards=="DD")
            {
                DisplayCard("DD");
                if (cards=="BDD")
                    Blue.Visible = true;
                if (cards=="GDD")
                    Green.Visible = true;
                if (cards=="RDD")
                    Red.Visible = true;
                if (cards=="YDD")
                    Yellow.Visible = true;
            }
            else
                DisplayCard(cards);

        }
        // Hàm xử lý khi khởi động
        private async void Player_Load(object sender, EventArgs e)
        {

            try
            {
                await ConnectToServer();
                await ReceiveMessagesAndUpdate();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }
        // Hàm xử lý kết nối server
        private async Task ConnectToServer()
        {
            try
            {
                ten=NameBox.Text;
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(IPAddress.Parse(IpServer), 8080); // Thay đổi địa chỉ IP và cổng nếu cần
                networkStream = new NetworkStream(client);
                reader = new StreamReader(networkStream);
                writer = new StreamWriter(networkStream) { AutoFlush = true };
                await writer.WriteLineAsync($"Login: {ten},{")"}");////khúc này có thể thêm mật khẩu cho mỗi player
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }
        // Hàm xử lý hàm xử lý giao tiếp từ server
        private async Task ReceiveMessagesAndUpdate()
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
                        if (clientId == 0)
                        {
                            MessageBox.Show("???. bạn đã trong phòng rồi mà. Phá game à");
                            this.Close();
                        }
                        UpdateClientIdLabel(clientId);
                    }
                    if (messagee.StartsWith("IDroom: "))
                    {
                        Room.Text = messagee.Split(':')[1].Trim();
                    }
                    // Kiểm tra nếu tin nhắn chứa id lượt của người chơi
                    if (messagee.StartsWith("IDplay: "))
                    {
                        foreach (Control control in this.Controls)
                            if (control is GroupBox)
                                control.BackColor = Color.White;
                        Idplay = int.Parse(messagee.Split(':')[1].Trim());

                        if (Idplay==1)
                        {
                            if (Idplay==clientId)
                                groupBoxp1.BackColor= Color.Red;
                            else
                                groupBoxp1.BackColor= Color.FromArgb(255, 255, 128);
                        }
                        if (Idplay==2)
                        {
                            if (Idplay==clientId)
                                groupBoxp2.BackColor= Color.Red;
                            else
                                groupBoxp2.BackColor= Color.FromArgb(255, 255, 128);
                        }
                        if (Idplay==3)
                        {
                            if (Idplay==clientId)
                                groupBoxp3.BackColor= Color.Red;
                            else
                                groupBoxp3.BackColor= Color.FromArgb(255, 255, 128);
                        }
                        if (Idplay==4)
                        {
                            if (Idplay==clientId)
                                groupBoxp1.BackColor= Color.Red;
                            else
                                groupBoxp4.BackColor= Color.FromArgb(255, 255, 128);
                        }
                    }
                    // Kiểm tra nếu tin nhắn chứa lá bài rút
                    if (messagee.StartsWith("Card:"))
                    {
                        string cardName = messagee.Split(':')[1].Trim();

                        LastCard.Image = cardImages[cardName];
                        card.Add(cardName);
                        currentIndex=0;
                        DisplayFirstSixCards();
                    }
                    else if (messagee.StartsWith("CardTop: "))
                    {
                        cardTop = messagee.Split(':')[1].Trim();

                        HandleCardReceived(cardTop);

                    }
                    if (messagee.StartsWith("Plus: "))
                    {
                        // Phân tích tin nhắn để lấy thông tin tên, loại và số của lá bài
                        string[] parts = messagee.Split(' ');
                        if (Idplay==clientId)
                        {
                            string card = parts[1];
                            plus = int.Parse(parts[2]);
                            PlusTable.Text=plus.ToString();
                        }

                    }
                    if (messagee.StartsWith("IDcards: "))
                    {
                        // số lượng lá bài của mỗi player
                        string[] IDcards = messagee.Split(':')[1].Trim().Split(',');
                        player1.Text= IDcards[0];
                        player2.Text= IDcards[1];
                        player3.Text= IDcards[2];
                        player4.Text= IDcards[3];
                        //end game
                        if (player1.Text=="0"
                          ||player2.Text=="0"
                          ||player3.Text=="0"
                          ||player4.Text=="0")
                            foreach (Control control in this.Controls)
                                control.Visible = false;

                        if (((player1.Text=="0")&&(clientId==1)
                          ||((player2.Text=="0")&&(clientId==2))
                          ||((player3.Text=="0")&&(clientId==3))
                          ||((player4.Text=="0")&&(clientId==4))))
                            IDcard.Text="you win";

                        else
                        if (IDcards[0] == "0" || IDcards[1] == "0" || IDcards[2] == "0" || IDcards[3] == "0")
                        {
                            IDcard.Text="you lost";
                            IDcard.Visible=true;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Disconnected from server.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving messages from server: " + ex.Message);
            }
        }

        //hàm yêu cầu rút bài từ server
        private void HandleDrawRequest(int count)
        {
            try
            {
                if (count == 0)
                {
                    writer.WriteLine("draw");
                    writer.Flush();
                }
                else
                    for (int i = 0; i < count; i++)
                    {
                        writer.WriteLine("draw");
                        writer.Flush();
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending 'draw' request to server: " + ex.Message);
            }
            if (plus!=0) plus--;
            PlusTable.Text=plus.ToString();
        }
        //ID của bạn
        private void UpdateClientIdLabel(int clientId)
        {
            labelClientId.Text = "ID: " + clientId.ToString();
        }

        private void Player_FormClosing(object sender, FormClosingEventArgs e)
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
                Begin.Visible=false;
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
            currentIndextb.Text=currentIndex.ToString();
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

        private void drawButton_Click(object sender, EventArgs e)
        {
            if (Idplay==clientId)
            {
                if (plus==0)
                    HandleDrawRequest(1);
                else
                {
                    HandleDrawRequest(plus);
                    plus=0;
                    PlusTable.Text=plus.ToString();
                }

            }
            else MessageBox.Show("Bạn bị *** à. Đây không phải lượt của bạn");
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
                {
                    card.RemoveAt(index);
                    if (selectedCard[0]=='D')
                        SendColorToServer(selectedCard);
                    else
                        SendCardToServer(selectedCard);
                }
                else
                {
                    if (clientId==Idplay)
                        MessageBox.Show("You cannot play this card.");
                    else
                        MessageBox.Show("It not your turn");
                    //+clientId+Idplay);
                }

                UpdatePictureBoxes();
            }
        }
        int B = 0, G = 0, R = 0, Y = 0;
        bool colorSelected = false;
        private async Task SendColorToServer(string selectedCard)
        {
            try
            {
                // Gửi thông tin lá bài và màu đã chọn lên server
                if (IsPlayable(selectedCard, cardTop))
                {
                    Blue.Visible = true;
                    Green.Visible = true;
                    Red.Visible = true;
                    Yellow.Visible = true;
                    B=0; G=0; R=0; Y=0;
                    string t = "";

                    await Task.Run(() =>
                    {
                        while (!colorSelected)
                        {
                            Thread.Sleep(100); // Đợi người dùng chọn màu
                        }
                    });
                    Blue.Visible = false;
                    Green.Visible = false;
                    Red.Visible = false;
                    Yellow.Visible = false;
                    if (B==1)
                        t="B"+selectedCard;
                    if (G==1)
                        t="G"+selectedCard;
                    if (R==1)
                        t="R"+selectedCard;
                    if (Y==1)
                        t="Y"+selectedCard;

                    // Reset trạng thái chọn màu
                    colorSelected = false;
                    await writer.WriteLineAsync($"PlayCard: {t}");
                    await writer.FlushAsync();
                    plus = 0;
                    PlusTable.Text = plus.ToString();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending card information to server: " + ex.Message);
            }
        }

        private void Yellow_Click(object sender, EventArgs e)
        {
            Y=1; colorSelected=true;
        }

        private void Green_Click(object sender, EventArgs e)
        {
            G=1; colorSelected=true;
        }

        private void Blue_Click(object sender, EventArgs e)
        {
            B=1; colorSelected=true;
        }

        private void Red_Click(object sender, EventArgs e)
        {
            R=1; colorSelected=true;
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
                    plus = 0;
                    PlusTable.Text = plus.ToString();
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
            string cards = cardTop;
            if (cardTop=="DD"||cardTop=="DP")
                return true;
            if (Idplay!=clientId)
                return false;

            if (selectedCard=="DP")
                return true;

            if (selectedCard=="DD")
                return (plus==0);

            if (cards == "RDP" || cards == "YDP" || cards == "BDP" || cards == "GDP"||cards== "DP")
            {
                return (((plus==0)&&(selectedColor == topColor)
                    || (selectedColor==topColor && selectedNumber == "P")));
            }
            else if (cards == "RDD" || cards == "YDD" || cards == "BDD" || cards == "GDD"||cards=="DD")
            {
                return (selectedColor == topColor);
            }
            else
            if (plus!=0)
                return ((selectedColor == topColor&&selectedNumber=="P")||(selectedNumber=="P")||selectedCard=="DP");
            // Check if the selected card matches the color or number of the top card, or if it's a special card (DD or DF)
            return (selectedColor == topColor || selectedNumber == topNumber || (selectedCard == "DD"&&topNumber!="P")
            || selectedCard == "DP" ||  cardTop == "DD" || cardTop == "DP");
        }

        private void Sort_Click(object sender, EventArgs e)
        {
            card.Sort();
            DisplayFirstSixCards();
        }
    }
}
