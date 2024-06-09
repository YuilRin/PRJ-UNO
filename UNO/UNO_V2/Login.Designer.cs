namespace UNO_V2
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LoginTb = new Button();
            NameTb = new TextBox();
            lbName = new Label();
            LbIp = new Label();
            IpTb = new TextBox();
            SuspendLayout();
            // 
            // LoginTb
            // 
            LoginTb.Location = new Point(65, 255);
            LoginTb.Name = "LoginTb";
            LoginTb.Size = new Size(135, 40);
            LoginTb.TabIndex = 0;
            LoginTb.Text = "Login";
            LoginTb.UseVisualStyleBackColor = true;
            LoginTb.Click += Login_Click;
            // 
            // NameTb
            // 
            NameTb.Location = new Point(77, 130);
            NameTb.Name = "NameTb";
            NameTb.Size = new Size(117, 27);
            NameTb.TabIndex = 1;
            // 
            // lbName
            // 
            lbName.AutoSize = true;
            lbName.Location = new Point(22, 133);
            lbName.Name = "lbName";
            lbName.Size = new Size(49, 20);
            lbName.TabIndex = 2;
            lbName.Text = "Name";
            // 
            // LbIp
            // 
            LbIp.AutoSize = true;
            LbIp.Location = new Point(22, 197);
            LbIp.Name = "LbIp";
            LbIp.Size = new Size(22, 20);
            LbIp.TabIndex = 2;
            LbIp.Text = "Ip";
            // 
            // IpTb
            // 
            IpTb.Location = new Point(77, 190);
            IpTb.Name = "IpTb";
            IpTb.Size = new Size(117, 27);
            IpTb.TabIndex = 1;
            IpTb.Text = "127.0.0.1";
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(281, 349);
            Controls.Add(LbIp);
            Controls.Add(lbName);
            Controls.Add(IpTb);
            Controls.Add(NameTb);
            Controls.Add(LoginTb);
            Name = "Login";
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button LoginTb;
        private TextBox NameTb;
        private Label lbName;
        private Label LbIp;
        private TextBox IpTb;
    }
}